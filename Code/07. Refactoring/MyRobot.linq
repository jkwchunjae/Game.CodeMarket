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
    /// <summary>목적지로 간다. 못 가면 false</summary>
    protected bool Goto(MyCell target)
    {
        // 이미 그 위치면 움직일 필요 없다.
        // 내 근처이면 그냥 움직인다.
        // 멀리 있으면 경로를 계산해서 움직인다.
        //if (robot.GetX() == target.X && robot.GetY() == target.Y)
        //{
        //    return true;
        //}
        //else if (robot.GetX() + 1 == target.X && robot.GetY() == target.Y)
        //{
        //    return robot.Move(Direction.Right);
        //}
        //else if (robot.GetX() - 1 == target.X && robot.GetY() == target.Y)
        //{
        //    return robot.Move(Direction.Left);
        //}
        //else if (robot.GetX() == target.X && robot.GetY() + 1 == target.Y)
        //{
        //    return robot.Move(Direction.Up);
        //}
        //else if (robot.GetX() == target.X && robot.GetY() - 1 == target.Y)
        //{
        //    return robot.Move(Direction.Down);
        //}
        //else
        //{
        //    while (Goto(target) == false)
        //    {
        //        // 멀다! 요청하자.
        //        var paths = _path.GetPath(MyCell, target);
        //        foreach (var cell in paths)
        //        {
        //            if (Goto(cell) == false)
        //            {
        //                break;
        //            }
        //        }
        //    }
        //    return true;
        //}
        var x = target.X;
        var y = target.Y;
        while (true)
        {
            if (robot.GetX() == x)
            {
                if (robot.GetY() == y)
                {
                    // 도착할 때 까지 계속 움직여본다.
                    return true;
                }
            }
            if (robot.GetX() == 3)
            {
                while (robot.GetY() < y)
                {
                    if (robot.CanMove(Direction.Up))
                    {
                        if (robot.Move(Direction.Up) == false)
                            break;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (robot.GetX() == 2)
            {
                while (robot.GetY() > y)
                {
                    if (robot.CanMove(Direction.Down))
                    {
                        if (robot.Move(Direction.Down) == false)
                            break;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            while (robot.GetX() < x)
            {
                if (robot.CanMove(Direction.Right))
                {
                    if (robot.Move(Direction.Right) == false)
                        break;
                }
                else
                {
                    break;
                }
            }
            while (robot.GetX() > x)
            {
                if (robot.CanMove(Direction.Left))
                {
                    if (robot.Move(Direction.Left) == false)
                        break;
                }
                else
                {
                    break;
                }
            }
        }
    }
}


class ServingRobot : MyRobot
{
    public ServingRobot(IStaticData staticData, IStoreMap map, IPathManager pathManager)
    : base(staticData, map, pathManager)
    {
    }
    /// <summary>MainLoop</summary>
    public void Run()
    {
        while (true)
        {
            var order = TakeOrder();
            if (order != null)
            {
                while (GetOrderRemaining(order) > 0)
                {
                    if (HasSomeItemOrder(order))
                    {
                        // 일단 손에 있는 것을 먼저 넣어준다.
                        PutInBagWithOrder(order);
                    }
                    // 선반에서 가져온다.
                    PickFromShelfWithOrder(order);
                    if (robot.GetCarried().Count() > 0)
                    {
                        // 손에 있는 것을 장바구니에 담는다.
                        PutInBagWithOrder(order);
                    }
                }
                order.RemoveRemaining();
            }
            else
            {
                // 다른 손님을 기다린다.
                robot.Move(Direction.Right);
                robot.Move(Direction.Right);
                robot.Move(Direction.Left);
                robot.Move(Direction.Left);
            }
        }
    }


    /// <summary> 선반에서 주문에 필요한 만큼 담는다. </summary>
    void PickFromShelfWithOrder(Order order)
    {
        Goto(new MyCell(3, 2));
        MyCell[] cells = new MyCell[]
        {
            new MyCell(3, 3),
            new MyCell(3, 4),
            new MyCell(3, 5),
            new MyCell(3, 6),
            new MyCell(2, 6),
            new MyCell(2, 5),
            new MyCell(2, 4),
            new MyCell(2, 3),
        };
        foreach (var cell in cells)
        {
            Goto(cell);
            var pair = _map.GetCellProductPair(cell);
            if (pair != null)
            {
                PickFromShelfWithProduct(order, pair.Product);
            }
        }
    }

    /// <summary>선반에서 주문한 수량의 일부라도 가져올 수 있는지 본다.</summary>
    void PickFromShelfWithProduct(Order order, Product product)
    {
        if (order.GetRemaining(product) > 0)
        {
            var result = PickFromShelfWithCount(product, order.GetRemaining(product));
            if (result == 1) // 손이 가득 찼다.
            {
                return; // 일단 들 수 있는 만큼 들었다.
            }
            else if (result == 2) // 선반이 비었다.
            {
                // 그냥 지나간다.
                return;
            }
        }
    }

    /// <summary>선반에 남은 수량 확인. 필요한 만큼 못 얻었으면 false</summary>
    int PickFromShelfWithCount(Product product, int count)
    {
        GotoShelf(product);
        var shelf = robot.GetShelf();
        if (shelf.IsEmpty())
        {
            // 비어있다.
            return 2;
        }
        else
        {
            while (true)
            {
                if (CarriedCount(product) >= count)
                {
                    return 0; // 원하는 만큼 이미 가지고 있다.
                }
                if (robot.Pick(product) == false)
                {
                    if (RobotIsFull())
                    {
                        return 1; // 로봇이 더 이상 들 수 없다.
                    }
                    else
                    {
                        return 2; // 선반이 비었다.
                    }
                }
            }
        }
    }

    bool RobotIsFull()
    {
        return robot.GetCapacity() >= robot.GetCarried().Count();
    }

    /// <summary>장바구니에 가서 손에 든 것을 내려 놓는다.</summary>
    void PutInBagWithOrder(Order order)
    {
        GotoBag();
        foreach (var product in _static.AvailableProducts())
        {
            PutInBagWithProduct(order, product);
        }
    }

    bool PutInBagWithProduct(Order order, Product product)
    {
        while (order.GetRemaining(product) > 0)
        {
            if (robot.PutInBag(product) == false)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>주문내역 중 로봇이 가지고 있는게 있는지 본다.</summary>
    bool HasSomeItemOrder(Order order)
    {
        return _static.AvailableProducts()
            .Any(product => HasSomeItemWithProduct(order, product));
    }
    /// <summary>주문내역 중 로봇이 가지고 있는게 있는지 본다.</summary>
    bool HasSomeItemWithProduct(Order order, Product product)
    {
        var myCount = CarriedCount(product);

        if (order.GetRemaining(product) > 0)
        {
            if (myCount > 0)
            {
                return true;
            }
        }
        // product를 주문하지 않았으면 false이다.
        return false;
    }
    int CarriedCount(Product product)
    {
        var myCount = 0;
        foreach (var item in robot.GetCarried())
        {
            if (item == product)
            {
                myCount += 1;
            }
        }
        return myCount;
    }


    int GetOrderRemaining(Order order)
    {
        return _static.AvailableProducts()
            .Sum(product => order.GetRemaining(product));
    }
    private Order? TakeOrder()
    {
        GotoRegister();
        var order = robot.TakeOrder();
        var customer = GetCustomer();
        if (order != null)
        {
            RemoveAlcoholWhenNight(order);
            RemoveWhenNotSeason(order);
            if (customer != null)
            {
                RemoveAdultOnlyProduct(order, customer);
            }
        }
        return order;
    }
    Customer GetCustomer()
    {
        GotoRegister();
        var customer = robot.GetCustomer();

        return customer;
    }

    /// <summary>미성년자는 맥주, 담배를 살 수 없다.</summary>
    void RemoveAdultOnlyProduct(Order order, Customer customer)
    {
        if (IsKids(customer))
        {
            RemoveItem(order, item => item.IsAdultsOnly());
        }
    }
    bool IsKids(Customer customer)
    {
        if (customer != null)
        {
            if (customer.IsAdult() == false)
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>밤에는 주류를 판매하면 안된다.</summary>
    void RemoveAlcoholWhenNight(Order order)
    {
        if (IsAlcoholAllowedTime() == false)
        {
            RemoveItem(order, item => item.IsAlcohol());
        }
    }
    bool IsAlcoholAllowedTime()
    {
        var hour = store.GetHour();
        if (hour >= 6)
        {
            if (hour < 22)
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>특정 계절에만 주문할 수 있는 상품이 있다. 계절에 맞지 않으면 주문에서 뺀다.</summary>
    void RemoveWhenNotSeason(Order order)
    {
        RemoveItem(order, item => item.IsInSeason() == false);
    }
    void RemoveItem(Order order, Func<Product, bool> condition)
    {
        var products = order.GetProducts();
        foreach (var item in products)
        {
            if (condition(item))
            {
                order.Remove(item);
            }
        }
    }

    private void GotoRegister()
    {
        Goto(new MyCell(2, 2));
        robot.Turn(Direction.Down);
    }
    private void GotoBag()
    {
        Goto(new MyCell(3, 2));
        robot.Turn(Direction.Down);
    }
    private void GotoShelf(Product product)
    {
        var cellProductPair = _map.GetCellProductPairWithProduct(product);
        if (cellProductPair != null)
        {
            var target = cellProductPair.Cell;
            Goto(target);
            robot.Turn(cellProductPair.Direction);
        }
    }
}

class ShelfData
{
    public Product Product { get; private set; }
    public int Capacity { get; private set;}
    public List<MyCell> Cells { get; private set; }
    public ShelfData(MyCell cell, Product product, int capacity)
    {
        Cells = new List<MyCell>();
        Cells.Add(cell);
        Product = product;
        Capacity = capacity;
    }
}
class DeliveryRobot : MyRobot
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
            CallDelivery();
            // 차고 앞으로 이동
            GotoGarage();
            // 하차
            PickFromTruck();
            // 선반에 넣는다.
            PutToShelf();
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
                robot.Move(cellData!.Direction);
                var shelf = robot.GetShelf();
                var shelfCapacity = shelf.GetCapacity();
                var entity = _static.GetEntityFromProduct(cellData.Product);
                var shelfData = new ShelfData(cell, cellData.Product, shelf.GetCapacity());
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
    record ShelfOrder(Product Product, int EmptyCount, double Random);
    /// <summary>배송 요청한다.</summary>
    private void CallDelivery()
    {
        if (store.IsTruckAway() == false)
            return;

        // 빈 공간이 많은 순으로 주문한다.
        // 빈 공간이 같다면 랜덤을 사용해서 순서를 섞는다.
        var orderShelfs = _shelfs
            .Select(x => new ShelfOrder(x.Product, x.Capacity - store.GetStock(x.Cells.First().X, x.Cells.First().Y), Random.Shared.Next()))
            .OrderByDescending(x => x.EmptyCount)
            .ThenBy(x => x.Random)
            .ToArray();

        robot.GetList().Clear();
        foreach (var shelf in orderShelfs)
        {
            robot.GetList().Add(shelf.Product);
            if (robot.GetList().Count() >= store.GetTruckCapacity())
            {
                break;
            }
        }
        store.CallDelivery(robot.GetList());
    }
    /// <summary>트럭에서 물건을 내린다. 다 꺼냈으면 true 남았으면 false</summary>
    private bool PickFromTruck()
    {
        var tryCount = 0;
        while (true)
        {
            tryCount += 1;
            if (store.IsTruckHere())
            {
                if (robot.IsAtTruck())
                {
                    break;
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
        var targetProducts = carriedItems.Distinct();
        // 선반의 위치를 찾는다.
        var targetShelfs = _shelfs
            .Where(shelf => targetProducts.Contains(shelf.Shelf.GetProduct()))
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