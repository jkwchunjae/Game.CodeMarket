<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load "../basecode"
#load "./StoreMap"
#load "./StaticData"
#load "./Node"

void Main()
{
    store = new TestStore();
    var graph = Graph.Create(new TestStoreMap());
    graph.ToGridString().DumpTell();
}

class TestStoreMap : IStoreMap
{
    public MyCell GarageCell => throw new NotImplementedException();

    public MyCell[] Registers => throw new NotImplementedException();

    public ShelfGroup[] GetAllShelfGroup(int startIndex)
    {
        throw new NotImplementedException();
    }

    public CellProductPair? GetCellProductPair(MyCell cell)
    {
        throw new NotImplementedException();
    }

    public CellProductPair? GetCellProductPairWithProduct(Product product)
    {
        throw new NotImplementedException();
    }

    public MyCell[] GetFrontOfShelfCells()
    {
        return new MyCell[]
        {
            new(2, 13), new(3, 13), new(6, 13), new(7, 13), new(10, 13), new(11, 13),
            new(2, 12), new(3, 12), new(6, 12), new(7, 12), new(10, 12), new(11, 12),
            new(2, 11), new(3, 11), new(6, 11), new(7, 11), new(10, 11), new(11, 11),
            new(2, 10), new(3, 10), new(6, 10), new(7, 10), new(10, 10), new(11, 10),
            new(2, 9), new(3, 9), new(6, 9), new(7, 9), new(10, 9), new(11, 9),
            new(2, 8), new(3, 8), new(6, 8), new(7, 8), new(10, 8), new(11, 8),
        };
        throw new NotImplementedException();
    }

    public ShelfGroup GetShelfGroup(int index)
    {
        throw new NotImplementedException();
    }
}

class C(int x, int y) : Cell
{
    public int GetX() => x;
    public int GetY() => y;
}

class TestStore : Store
{
    public List<Cell> FindAll(Entity entity)
    {
        var y = 3;
        return entity switch
        {
            Entity.Register => [new C(2, y), new C(4, y), new C(6, y), new C(8, y), new C(10, y)],
            Entity.Bag => [new C(3, y), new C(5, y), new C(7, y), new C(9, y), new C(11, y)],
            _ => [],
        };
    }
    public int GetWidth() => 16;
    public int GetHeight() => 17;
    public int GetGarageX() => 2;
    public int GetGarageY() => GetHeight() - 1;
    public bool IsFree(int x, int y) => true;

    public bool CallDelivery(List<Product> goods) => throw new NotImplementedException();
    public Cell Find(Entity entity) => throw new NotImplementedException();
    public int GetBuyPrice(Product product) => throw new NotImplementedException();
    public Cell GetCashierCell(int x, int y) => throw new NotImplementedException();
    public int GetDay() => throw new NotImplementedException();
    public Entity? GetEntity(int x, int y) => throw new NotImplementedException();
    public Direction GetGarageSide() => throw new NotImplementedException();
    public int GetHour() => throw new NotImplementedException();
    public int GetMinute() => throw new NotImplementedException();
    public int GetMoney() => throw new NotImplementedException();
    public Cell GetPackerCell(int x, int y) => throw new NotImplementedException();
    public List<Direction> GetPickupDirections(int x, int y, Product product) => throw new NotImplementedException();
    public int GetRegisterNumber(int x, int y) => throw new NotImplementedException();
    public Direction? GetRegisterSide(int x, int y) => throw new NotImplementedException();
    public int GetRemaining(Product product) => throw new NotImplementedException();
    public Season GetSeason() => throw new NotImplementedException();
    public int GetStock(int x, int y) => throw new NotImplementedException();
    public int GetTruckCapacity() => throw new NotImplementedException();
    public Customer GetWaitingCustomer() => throw new NotImplementedException();
    public bool IsTruckAway() => throw new NotImplementedException();
    public bool IsTruckHere() => throw new NotImplementedException();
    public bool SendTruckAway() => throw new NotImplementedException();
}

void TestNode()
{
    var storeMap = new StoreMap(new StaticData());
    var graph = Graph.Create(storeMap);
    Console.Error.WriteLine(graph.ToGridString());
}