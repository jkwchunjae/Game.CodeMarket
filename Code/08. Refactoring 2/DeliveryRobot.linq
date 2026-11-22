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
class DeliveryRobot : RobotBase
{
    private List<ShelfData> _shelfs = new();

    public DeliveryRobot(IStaticData staticData, IStoreMap map, IPathManager pathManager)
        : base(staticData, map, pathManager)
    {
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
                robot.Move(Direction.Right);
                robot.Move(Direction.Left);
            }
        }
    }

    /// <summary>한 바퀴 돌면서 선반 객체를 가져온다.</summary>
    public void CollectShelfs()
    {
        _shelfs.Clear();
        var cells = _map.GetShelfCells();
        if (_path.ContainsInCell(cells, MyCell) == false)
        {
            // 경로에서 완전히 벗어나있으면 일단 시작 위치로 이동한다.
            Goto(cells[0]);
        }
        CellProductPair? prevCell = null;
        ShelfData? prevShelfData = null;
        foreach (var cell in cells)
        {
            // 일단 이동!
            Goto(cell);
            if (_map.IsFrontOfShelf(cell))
            {
                // 선반 앞에 도착했다.
                var cellData = _map.GetCellProductPair(cell);
                if (prevCell != null && prevShelfData != null && IsDuplicatedShelf(prevCell, cellData!))
                {
                    // 앞과 중복이면 넘어간다.
                    prevCell = cellData;
                    prevShelfData.Cells.Add(cell);
                    continue;
                }
                // 선반을 바라본다.
                robot.Turn(cellData!.Direction);
                var shelf = robot.GetShelf();
                var shelfCapacity = shelf.GetCapacity();
                var entity = _static.GetEntityFromProduct(cellData.Product);
                var shelfData = new ShelfData(cellData.ShelfPosition, cell, cellData.Product, shelf.GetCapacity());
                _shelfs.Add(shelfData);
                prevCell = cellData;
                prevShelfData = shelfData;
            }
        }
    }
    private bool IsDuplicatedShelf(CellProductPair cell1, CellProductPair cell2)
    {
        // 인접해 있고, 같은 entity면 중복이라고 하자.
        return cell1.Product == cell2.Product &&
            Math.Abs(cell1.Cell.X - cell2.Cell.X) + Math.Abs(cell1.Y - cell2.Y) == 1;
    }
    
    /// <summary>차고 앞으로 간다.</summary>
    private void GotoGarage()
    {
        var target = new MyCell(store.GetGarageX(), store.GetGarageY());
        Goto(target);
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
                robot.Move(Direction.Right);
                robot.Move(Direction.Left);
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
        var targetProducts = carriedItems.Distinct();
        // 선반의 위치를 찾는다.
        var targetShelfs = _shelfs
            .Where(shelf => targetProducts.Contains(shelf.Product))
            .ToArray();
        var targetCells = targetShelfs.Select(x => x.Cells[0]).ToArray();
        var paths = _path.GetPathTraverse(MyCell, targetCells);
        foreach (var cell in paths)
        {
            Goto(cell);
            if (targetCells.Contains(cell))
            {
                var cellProductPair = _map.GetCellProductPair(cell);
                var product = cellProductPair.Product;
                robot.Turn(cellProductPair.Direction);
                
                while (robot.Has(product))
                {
                    // 선반이 꽉 차서 더 이상 못 넣는 경우 무한루프에 빠진다.
                    // 그런데 이런 일은 잘 없을 것 같다.
                    // 배송로봇이 2명이 될 때 생각해보자.
                    robot.PutOnShelf(product);
                }
            }
        }
    }
}