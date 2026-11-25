<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load "../basecode"
#load "./StoreMap"
#load "./StaticData"
#load "./PathManager"
#load "./Node"
#load "./DeliveryRobot"

void Main()
{
    var staticData = new StaticData();
    var storeMap = new TestStoreMap();
    var pathManager = new PathManager(storeMap);
    
    
    store = new TestStore();
    var deliveryRobot = new DeliveryRobot(staticData, storeMap, pathManager);
    
    deliveryRobot.Run();
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
            new(1, 6), new(2, 6), new(5, 6), new(6, 6),
            new(1, 5), new(2, 5), new(5, 5), new(6, 5),
            new(1, 4), new(2, 4), new(5, 4), new(6, 4),
            new(1, 3), new(2, 3), new(5, 3), new(6, 3),
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

class TestRobot : Robot
{
    public bool CanMove(Direction direction) => throw new NotImplementedException();
    public int GetCapacity() => throw new NotImplementedException();
    public List<Product> GetCarried() => throw new NotImplementedException();
    public Customer GetCustomer() => throw new NotImplementedException();
    public List<Product> GetDelivery() => throw new NotImplementedException();
    public Direction GetFacing() => throw new NotImplementedException();
    public List<Product> GetList() => throw new NotImplementedException();
    public int GetLoad() => throw new NotImplementedException();
    public int GetNumber() => throw new NotImplementedException();
    public Shelf GetShelf() => throw new NotImplementedException();
    public int GetX() => throw new NotImplementedException();
    public int GetY() => throw new NotImplementedException();
    public bool Has(Product product) => throw new NotImplementedException();
    public bool IsAtBag() => throw new NotImplementedException();
    public bool IsAtPickup() => throw new NotImplementedException();
    public bool IsAtRegister() => throw new NotImplementedException();
    public bool IsAtTruck() => throw new NotImplementedException();
    public bool IsFacing(Entity entity) => throw new NotImplementedException();
    public Entity Look(Direction direction) => throw new NotImplementedException();
    public bool Move(Direction direction) => throw new NotImplementedException();
    public bool Pick(Product product) => throw new NotImplementedException();
    public bool PutInBag(Product product) => throw new NotImplementedException();
    public bool PutOnShelf(Product product) => throw new NotImplementedException();
    public Order TakeOrder() => throw new NotImplementedException();
    public void Turn(Direction direction) => throw new NotImplementedException();
}