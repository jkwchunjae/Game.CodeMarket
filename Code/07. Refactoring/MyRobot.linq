<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load "../basecode"
#load "./StoreMap"
#load "./PathManager"
#load "./StaticData"

class MyRobot
{
    protected IStaticData _static;
    protected IStoreMap _map;
    protected IPathManager _path;
    protected MyCell MyCell => new MyCell(robot.GetX(), robot.GetY());
    public MyRobot(IStaticData staticData, IStoreMap map, IPathManager pathManager)
    {
        _static = staticData;
        _map = map;
        _path = pathManager;
    }
    protected void Goto(MyCell target)
    {
        // 이미 그 위치면 움직일 필요 없다.
        // 내 근처이면 그냥 움직인다.
        // 멀리 있으면 경로를 계산해서 움직인다.
    }
}

class CasherRobot: MyRobot
{
    
}

class ShelfData
{
    public Shelf Shelf { get; private set; }
    public List<MyCell> Cells { get; private set; }
    public ShelfData(MyCell cell, Shelf shelf)
    {
        Cells = new();
        Cells.Add(cell);
        Shelf = shelf;
    }
}
class DeliveryRobot : MyRobot
{
    private List<ShelfData> _shelfs = new();

    public DeliveryRobot(IStoreMap map) : base(map)
    {
    }
    
    /// <summary>한 바퀴 돌면서 선반 객체를 가져온다.</summary>
    public void CollectShelfs()
    {
        _shelfs.Clear();
        var cells = _map.GetDeliveryCells();
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
                if (prevCell != null && prevShelfData != null && IsDuplicatedShelf(prevCell, cellData))
                {
                    // 앞과 중복이면 넘어간다.
                    prevCell = cellData;
                    prevShelfData.Cells.Add(cell);
                    continue;
                }
                // 선반을 바라본다.
                robot.Move(cellData.Direction);
                var shelf = robot.GetShelf();
                var shelfCapacity = shelf.GetCapacity();
                var entity = _static.GetEntityFromProduct(cellData.Product);
                var shelfData = new ShelfData(cell, shelf);
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
}