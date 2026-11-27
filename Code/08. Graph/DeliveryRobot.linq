<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load "../basecode"
#load "./StoreMap"
#load "./PathManager"
#load "./StaticData"
#load "./RobotBase"

class ShelfData
{
    public Product Product { get; private set; }
    public int Capacity { get; private set; }
    public MyCell ShelfPosition { get; private set; }
    public List<MyCell> Cells { get; private set; }
    public ShelfData(MyCell shelfPosition, MyCell cell, Product product, int capacity)
    {
        ShelfPosition = shelfPosition;
        Cells = new List<MyCell>();
        Cells.Add(cell);
        Product = product;
        Capacity = capacity;
    }
}
class DeliveryRobotInit
{
    public int StartIndex { get; set; }
    public int[] MyShelfGroups { get; set; }
    public int GarageX { get; set; }
}
class DeliveryRobot : RobotBase
{
    private List<ShelfData> _shelfs = new();
    private readonly int _startIndex;
    private readonly int[] _myShelfGroups;
    private readonly MyCell _garageCell;
    private readonly Product[] _myProducts;
    private readonly Dictionary<int, Product[]> _myProductsByIndex;

    public DeliveryRobot(DeliveryRobotInit init, IStaticData staticData, IStoreMap map, IPathManager pathManager)
        : base(staticData, map, pathManager)
    {
        _startIndex = init.StartIndex;
        _myShelfGroups = init.MyShelfGroups;
        _garageCell = new MyCell(init.GarageX, store.GetGarageY());
        _myProducts = _myShelfGroups
            .Select(index => map.GetShelfGroup(index))
            .SelectMany(group => group.TargetProducts)
            .ToArray();
        _myProductsByIndex = _myShelfGroups
            .ToDictionary(index => index, index => map.GetShelfGroup(index).TargetProducts);
    }
    /// <summary>MainLoop</summary>
    public void Run()
    {
        // 한 바퀴 돌면서 선박 객체를 가져온다.
        // 일단 시작 할 때 한 번만 하고, 그 이후에 필요하면 또 하자.
        CollectShelfs();
        while (true)
        {
            // 배송 요청
            if (CallDelivery())
            {
                // 차고 앞으로 이동
                GotoGarage();
                // 하차
                var targetShelfIndexes = PickFromTruck();
                robot.Move(Direction.Down);
                // 선반에 넣는다.
                PutToShelf(targetShelfIndexes);
            }
            else
            {
                GotoGarage();
                if (store.IsTruckHere() && robot.IsAtTruck())
                {
                    var targetShelfIndexes = PickFromTruck();
                    robot.Move(Direction.Down);
                    PutToShelf(targetShelfIndexes);
                    GotoGarage();
                }
            }
        }
    }

    /// <summary>차고 앞으로 간다.</summary>
    private void GotoGarage()
    {
        Goto(_garageCell);
        robot.Turn(store.GetGarageSide());
    }

    #region CollectShelf: 선반 정보를 모은다.
    /// <summary>한 바퀴 돌면서 선반 객체를 가져온다.</summary>
    public void CollectShelfs()
    {
        _shelfs.Clear();
        // 1. 차고로 이동한다.
        // 2. 첫번째 선반그룹 TopEnter에서 순회한다.
        // 3. 순회 후 다음 선반그룹 TopEnter로 가서 순회한다.
        // 4. 마지막 선반그룹 순회 후 다시 차고로 이동한다.

        // 1. 차고로 이동한다.
        GotoGarage();
        
        var shelfGroups = _map.GetAllShelfGroup(_startIndex);
        foreach (var shelfGroup in shelfGroups)
        {
            // 2. 첫번째 선반그룹 TopEnter에서 순회한다.
            Goto(shelfGroup.TopEnter);
            CellProductPair prevPair = null;
            ShelfData prevShelf = null;
            foreach (var pair in shelfGroup.TraverseForDelivery())
            {
                Goto(pair.Cell);
                if (prevPair != null && prevPair.X == pair.X && prevPair.Product == pair.Product && prevShelf != null)
                {
                    // 2칸 짜리 선반은 한 칸만 처리한다.
                    prevShelf.Cells.Add(pair.Cell);
                    continue;
                }
                robot.Turn(pair.Direction);
                var shelf = robot.GetShelf();
                var shelfCapacity = shelf.GetCapacity();
                var entity = _static.GetEntityFromProduct(pair.Product);
                var shelfData = new ShelfData(pair.ShelfPosition, pair.Cell, pair.Product, shelf.GetCapacity());
                _shelfs.Add(shelfData);
                
                prevPair = pair;
                prevShelf = shelfData;
            }
            // 3. 순회 후 다음 선반그룹 TopEnter로 가서 순회한다.
            // 따로 뭘 할 필요는 없다. 다음 루프 Goto(shelfGroup.TopEnter);가 해준다.
        }
        
        GotoGarage();
    }
    #endregion

    #region CallDelivery: 배송 요청
    record ShelfOrder(Product Product, int Stock, int Capacity, double Random);
    /// <summary>배송 요청한다.</summary>
    private bool CallDelivery()
    {
        if (store.IsTruckAway() == false)
            return false;

        // 빈 공간이 많은 순으로 주문한다.
        // 빈 공간이 같다면 랜덤을 사용해서 순서를 섞는다.
        var orderShelfs = _shelfs
            .Select(x => new ShelfOrder(x.Product, store.GetStock(x.ShelfPosition.X, x.ShelfPosition.Y), x.Capacity, Random.Shared.Next()))
            // 많이 비어있는 것 먼저 처리한다. 완전 비어 있으면 우선순위를 높인다.
            .OrderByDescending(x => (x.Stock == 0 ? 999 : (x.Capacity - x.Stock)))
            .ThenBy(x => x.Random)
            .ToArray();

        if (store.IsTruckAway() == false)
            return false;

        robot.GetList().Clear();
        foreach (var shelfOrder in orderShelfs)
        {
            // 한 상품당 최대 3개만 신청하자. (한 상품이 너무 독점하지 않도록 방지)
            //var deliveryCount = Math.Min(5, shelfOrder.Capacity - shelfOrder.Stock);
            var deliveryCount = shelfOrder.Capacity - shelfOrder.Stock;
            if (robot.Has(shelfOrder.Product))
            {
                // 내가 들고있는건 주문할 필요 없다.
                var myCount = robot.GetCarried().Count(x => x == shelfOrder.Product);
                deliveryCount = Math.Max(0, deliveryCount - myCount);
            }
            AddProductIfPossibleWithCount(shelfOrder.Product, deliveryCount);

            if (store.IsTruckAway() == false)
                return false;
        }
        // 빵이 주문내역에 있으면 
        if (robot.GetList().Count(x => x == Product.Bread) > 0)
        {
            AddProductIfPossibleWithCount(Product.Bread, 1);
        }
        if (store.GetSeason() == Season.Spring)
        {
            AddProductIfPossibleWithCount(Product.Fertilizer, 2);
        }
        if (robot.GetList().Any() && store.IsTruckAway())
        {
            store.CallDelivery(robot.GetList());
            return true;
        }
        else
        {
            return false;
        }
    }
    void AddProductIfPossibleWithCount(Product product, int count)
    {
        for (var i = 0; i < count; i++)
        {
            if (product.GetSeason() == null || product.GetSeason() == store.GetSeason())
            {
                if (robot.GetList().Count() < store.GetTruckCapacity())
                {
                    robot.GetList().Add(product);
                }
            }
        }
    }
    #endregion
    
    #region PickFromTruck: 트럭에서 물건을 내린다.
    /// <summary>트럭에서 물건을 내린다. 트럭에서 꺼낸 물건이 포함된 선반 목록</summary>
    private int[] PickFromTruck()
    {
        #region waiting truck
        while (true)
        {
            GotoGarage();
            if (store.IsTruckHere())
            {
                if (robot.IsAtTruck())
                {
                    break;
                }
            }
        }
        #endregion
        
        var allProducts = robot.GetDelivery();
        // 다 들 수 없으면 선반 단위로 나눠서 들자.
        var myProductCount = allProducts.Count(p => _myProducts.Contains(p));
        var myRemainCount = robot.GetCapacity() - robot.GetLoad();
        var canPickAll = myRemainCount > myProductCount;
        
        if (canPickAll || myProductCount == 0)
        {
            // 전부 다 들 수 있는 경우.
            HashSet<Product> pickItems = new HashSet<Product>();
            foreach (var item in allProducts)
            {
                if (_myProducts.Contains(item))
                {
                    if (robot.Pick(item) == false)
                    {
                        return _myShelfGroups;
                    }
                    pickItems.Add(item);
                }
            }
            foreach (var index in _myShelfGroups)
            {
                // 내가 집은 상품이 모두 이 선반 그룹이면 이 값을 반환한다.
                var group = _map.GetShelfGroup(index);
                if (pickItems.All(p => group.TargetProducts.Contains(p)))
                {
                    return new int[] { index };
                }
            }
            return _myShelfGroups;
        }
        else
        {
            // 다 못 드는 경우. 선반 별로 들자.
            foreach (var shelfIndex in _myShelfGroups)
            {
                var group = _map.GetShelfGroup(shelfIndex);
                var shelfProducts = group.TargetProducts;
                var shelfProductCount = allProducts.Count(p => shelfProducts.Contains(p));
                if (shelfProductCount > 0)
                {
                    foreach (var item in allProducts)
                    {
                        if (shelfProducts.Contains(item))
                        {
                            if (robot.Pick(item) == false)
                            {
                                return new int[] { shelfIndex };
                            }
                        }
                    }
                    return new int[] { shelfIndex };
                }
            }
            throw new Exception("배달 로봇이 트럭에서 상품을 꺼내는 중 문제가 발생했습니다.");
        }
    }
    #endregion
    
    #region PutToShelf: 선반에 물건을 넣는다.
    /// <summary>선반에 물건을 넣는다.</summary>
    private void PutToShelf(int[] targetShelfIndexes /* 내가 돌아야 하는 선반 그룹 */)
    {
        var carriedItems = robot.GetCarried();
        var targetProducts = carriedItems.Distinct().ToArray();
        
        // 내가 담당해야 할 선반그룹 중에서 방문해야 할 선반 그룹을 고른다.
        var targetGroups = targetShelfIndexes
            .Select(index => _map.GetShelfGroup(index))
            //.Where(group => group.ContainsProducts(targetProducts))
            .ToArray();
            
        foreach (var group in targetGroups)
        {
            Goto(group.TopEnter);
            foreach (var pair in group.TraverseForDelivery())
            {
                Goto(pair.Cell);
                var currentShelfStock = store.GetStock(pair.ShelfPosition.X, pair.ShelfPosition.Y);
                var currentShelfCapacity = _shelfs.FirstOrDefault(x => x.ShelfPosition.Equal(pair.ShelfPosition))?.Capacity;
                var isFull = currentShelfStock >= currentShelfCapacity;
                if (isFull)
                    continue;

                while (robot.Has(pair.Product))
                {
                    // 선반이 꽉 찬 경우
                    if (store.GetStock(pair.ShelfPosition.X, pair.ShelfPosition.Y) >= currentShelfCapacity)
                        break;
                    
                    if (robot.GetFacing() != pair.Direction)
                        robot.Turn(pair.Direction);
                    
                    // 그 외 실패.
                    if (robot.PutOnShelf(pair.Product) == false)
                    {
                        // 로봇 손엔 있는데 선반에 꽉 찬 경우
                        break;
                    }
                }
                
                // 중간중간 배송 요청을 한다. (실패해도 그만이고, 요청하면 좋은거고)
                CallDelivery();
            }
        }
    }
    #endregion
}