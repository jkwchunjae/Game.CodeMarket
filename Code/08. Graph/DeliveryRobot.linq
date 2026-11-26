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
}
class DeliveryRobot : RobotBase
{
    private List<ShelfData> _shelfs = new();
    private readonly int _startIndex;
    private readonly int[] _myShelfGroups;
    private readonly Product[] _myProducts;

    public DeliveryRobot(DeliveryRobotInit init, IStaticData staticData, IStoreMap map, IPathManager pathManager)
        : base(staticData, map, pathManager)
    {
        _startIndex = init.StartIndex;
        _myShelfGroups = init.MyShelfGroups;
        _myProducts = _myShelfGroups
            .Select(index => map.GetShelfGroup(index))
            .SelectMany(group => group.TargetProducts)
            .ToArray();
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
                PickFromTruck();
                // 선반에 넣는다.
                PutToShelf();
            }
            else
            {
                GotoGarage();
                if (store.IsTruckHere() && robot.IsAtTruck())
                {
                    PickFromTruck();
                    PutToShelf();
                }
                while (robot.CanMove(Direction.Right))
                {
                    robot.Move(Direction.Right);
                }
                while (robot.CanMove(Direction.Left))
                {
                    robot.Move(Direction.Left);
                }
            }
        }
    }

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
            foreach (var pair in shelfGroup.TraverseForDelivery())
            {
                Goto(pair.Cell);
                robot.Turn(pair.Direction);
                var shelf = robot.GetShelf();
                var shelfCapacity = shelf.GetCapacity();
                var entity = _static.GetEntityFromProduct(pair.Product);
                var shelfData = new ShelfData(pair.ShelfPosition, pair.Cell, pair.Product, shelf.GetCapacity());
                _shelfs.Add(shelfData);
            }
            // 3. 순회 후 다음 선반그룹 TopEnter로 가서 순회한다.
            // 따로 뭘 할 필요는 없다. 다음 루프 Goto(shelfGroup.TopEnter);가 해준다.
        }
        
        GotoGarage();
    }
    
    /// <summary>차고 앞으로 간다.</summary>
    private void GotoGarage()
    {
        Goto(_map.GarageCell);
        robot.Turn(store.GetGarageSide());
    }

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

        robot.GetList().Clear();
        foreach (var shelfOrder in orderShelfs)
        {
            // 한 상품당 최대 3개만 신청하자. (한 상품이 너무 독점하지 않도록 방지)
            var deliveryCount = Math.Min(3, shelfOrder.Capacity - shelfOrder.Stock);
            if (robot.Has(shelfOrder.Product))
            {
                // 내가 들고있는건 주문할 필요 없다.
                var myCount = robot.GetCarried().Count(x => x == shelfOrder.Product);
                deliveryCount = Math.Max(0, deliveryCount - myCount);
            }
            for (var i = 0; i < deliveryCount; i++)
            {
                if (robot.GetList().Count() < store.GetTruckCapacity())
                {
                    robot.GetList().Add(shelfOrder.Product);
                }
            }
        }
        if (robot.GetList().Any())
        {
            store.CallDelivery(robot.GetList());
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>트럭에서 물건을 내린다. 다 꺼냈으면 true 남았으면 false</summary>
    private bool PickFromTruck()
    {
        while (true)
        {
            if (store.IsTruckHere())
            {
                GotoGarage();
                if (robot.IsAtTruck())
                {
                    break;
                }
                else
                {
                    robot.Move(Direction.Right);
                    robot.Move(Direction.Left);
                }
            }
            else
            {
                while (robot.CanMove(Direction.Right))
                {
                    robot.Move(Direction.Right);
                }
                while (robot.CanMove(Direction.Left))
                {
                    robot.Move(Direction.Left);
                }
            }
        }
        var allProducts = robot.GetDelivery();
        foreach (var item in allProducts)
        {
            if (robot.Pick(item) == false)
            {
                return false;
            }
        }
        return true;
    }
    /// <summary>선반에 물건을 넣는다.</summary>
    private void PutToShelf()
    {
        var carriedItems = robot.GetCarried();
        var targetProducts = carriedItems.Distinct().ToArray();
        
        // 내가 담당해야 할 선반그룹 중에서 방문해야 할 선반 그룹을 고른다.
        var targetGroups = _myShelfGroups
            .Select(index => _map.GetShelfGroup(index))
            .Where(group => group.ContainsProducts(targetProducts))
            .ToArray();
            
        foreach (var group in targetGroups)
        {
            Goto(group.TopEnter);
            foreach (var pair in group.TraverseForDelivery())
            {
                Goto(pair.Cell);
                robot.Turn(pair.Direction);

                while (robot.Has(pair.Product))
                {
                    if (robot.PutOnShelf(pair.Product) == false)
                    {
                        // 로봇 손엔 있는데 선반에 꽉 찬 경우
                        break;
                    }
                }
            }
        }
    }
}