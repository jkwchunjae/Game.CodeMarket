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
            new(1, 6), new(2, 6), new(5, 6), new(6, 6),  new(9, 6), new(10, 6),
            new(1, 5), new(2, 5), new(5, 5), new(6, 5),  new(9, 5), new(10, 5),
            new(1, 4), new(2, 4), new(5, 4), new(6, 4),  new(9, 4), new(10, 4),
            new(1, 3), new(2, 3), new(5, 3), new(6, 3),  new(9, 3), new(10, 3),
        };
        throw new NotImplementedException();
    }

    public MyCell[] GetShelfCells()
    {
        throw new NotImplementedException();
    }

    public bool IsFrontOfShelf(MyCell cell)
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
        return entity switch
        {
            Entity.Register => [new C(2, 0), new C(5, 0)],
            Entity.Bag => [new C(3, 0), new C(6, 0)],
            _ => [],
        };
    }
    public int GetWidth() => 12;
    public int GetHeight() => 11;
    public int GetGarageX() => 4;
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

