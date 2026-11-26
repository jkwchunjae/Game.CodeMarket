<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load "../basecode"
#load "./StoreMap"
#load "./StaticData"
#load "./PathManager"
#load "./Node"
#load "./ServingRobot"
#load "./RobotBase"

void Main()
{
    Product.Tomato = new ProductImpl();
    store = new TestStore();
    robot = new TestRobot();
    
    var staticData = new StaticData();
    var storeMap = new TestStoreMap();
    var pathManager = new PathManager(storeMap);
    var init = new ServingRobotInit()
    {
        MyShelfGroups = [0],
    };
    
    var servingRobot = new ServingRobot(init, staticData, storeMap, pathManager);
    
    servingRobot.Run();
}

class TestStoreMap : IStoreMap
{
    public MyCell GarageCell => new(2, store.GetHeight() - 1);
    public MyCell[] Registers => [new(2, 1)];

    public ShelfGroup[] GetAllShelfGroup(int startIndex)
    {
        return [
            new ShelfGroup([
                new CellProductPair(new(2, 8), new(3, 8), Product.Tomato, Direction.Left),
                new CellProductPair(new(2, 7), new(3, 7), Product.Tomato, Direction.Left),
                new CellProductPair(new(2, 6), new(3, 6), Product.Tomato, Direction.Left),
                new CellProductPair(new(2, 5), new(3, 5), Product.Tomato, Direction.Left),
                new CellProductPair(new(2, 4), new(3, 4), Product.Tomato, Direction.Left),

                new CellProductPair(new(5, 8), new(4, 8), Product.Tomato, Direction.Right),
                new CellProductPair(new(5, 7), new(4, 7), Product.Tomato, Direction.Right),
                new CellProductPair(new(5, 6), new(4, 6), Product.Tomato, Direction.Right),
                new CellProductPair(new(5, 5), new(4, 5), Product.Tomato, Direction.Right),
                new CellProductPair(new(5, 4), new(4, 4), Product.Tomato, Direction.Right),
            ]),
        ];
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
            new(2, 8), new(3, 8),
            new(2, 7), new(3, 7),
            new(2, 6), new(3, 6),
            new(2, 5), new(3, 5),
            new(2, 4), new(3, 4),
        };
        throw new NotImplementedException();
    }

    public ShelfGroup GetShelfGroup(int index)
    {
        return GetAllShelfGroup(0)[index];
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
            Entity.Register => [new C(2, 1)],
            Entity.Bag => [new C(3, 1)],
            _ => [],
        };
    }
    public int GetWidth() => 6;
    public int GetHeight() => 11;
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
    public int GetHour() => 10;
    public int GetMinute() => 20;
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
    int x = 2;
    int y = 2;
    public bool CanMove(Direction direction) => throw new NotImplementedException();
    public int GetCapacity() => throw new NotImplementedException();
    public List<Product> GetCarried() => throw new NotImplementedException();
    public Customer GetCustomer() => new TestCustomer();
    public List<Product> GetDelivery() => throw new NotImplementedException();
    public Direction GetFacing() => throw new NotImplementedException();
    public List<Product> GetList() => throw new NotImplementedException();
    public int GetLoad() => throw new NotImplementedException();
    public int GetNumber() => throw new NotImplementedException();
    public Shelf GetShelf() => throw new NotImplementedException();
    public int GetX() => x;
    public int GetY() => y;
    public bool Has(Product product) => throw new NotImplementedException();
    public bool IsAtBag() => throw new NotImplementedException();
    public bool IsAtPickup() => throw new NotImplementedException();
    public bool IsAtRegister() => throw new NotImplementedException();
    public bool IsAtTruck() => throw new NotImplementedException();
    public bool IsFacing(Entity entity) => throw new NotImplementedException();
    public Entity Look(Direction direction) => throw new NotImplementedException();
    public bool Move(Direction direction)
    {
        if (direction == Direction.Up) y++;
        if (direction == Direction.Down) y--;
        if (direction == Direction.Left) x--;
        if (direction == Direction.Right) x++;
        return true;
    }
    public bool Pick(Product product) => throw new NotImplementedException();
    public bool PutInBag(Product product) => throw new NotImplementedException();
    public bool PutOnShelf(Product product) => throw new NotImplementedException();
    public Order TakeOrder() => new TestOrder();
    public void Turn(Direction direction) {}
}

class TestCustomer : Customer
{
    public bool IsAdult() => true;
}

class TestOrder : Order
{
    public List<Product> GetProducts()
    {
        return [Product.Tomato, Product.Tomato];
    }
    public int GetRemaining(Product product)
    {
        return 2;
    }
    public int GetCount(Product product) => throw new NotImplementedException();
    public int GetTotal() => throw new NotImplementedException();
    public bool IsDone() => throw new NotImplementedException();
    public bool Remove(Product product) => throw new NotImplementedException();
    public bool RemoveRemaining() => throw new NotImplementedException();
}

class ProductImpl : Product
{
    public string GetName() => throw new NotImplementedException();
    public int GetPrice() => throw new NotImplementedException();
    public Entity GetRack() => throw new NotImplementedException();
    public Season? GetSeason() => throw new NotImplementedException();
    public bool IsAdultsOnly() => throw new NotImplementedException();
    public bool IsAlcohol() => throw new NotImplementedException();
    public bool IsElite() => throw new NotImplementedException();
    public bool IsInfinite() => throw new NotImplementedException();
    public bool IsInSeason() => true;
    public bool IsPremium() => throw new NotImplementedException();
    public bool IsVending() => throw new NotImplementedException();
}