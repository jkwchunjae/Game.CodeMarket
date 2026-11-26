<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load "../basecode"
#load "./StoreMap"
#load "./PathManager"
#load "./StaticData"
#load "./RobotBase"

class ServingRobotInit
{
    public int[] MyShelfGroups { get; set; }
}

class ServingRobot : RobotBase
{
    private readonly int[] _myShelfGroups;
    private readonly HashSet<Product> _myProducts;

    // 주문 중에서 내가 선반에 가서 담아야 하는 상품 수
    //private readonly Dictionary<Product, int> _remainProducts = new Dictionary<Product, int>();
    
    public ServingRobot(ServingRobotInit init, IStaticData staticData, IStoreMap map, IPathManager pathManager)
        : base(staticData, map, pathManager)
    {
        _myShelfGroups = init.MyShelfGroups;
        _myProducts = init.MyShelfGroups
            .SelectMany(index => map.GetShelfGroup(index).TargetProducts)
            .ToHashSet();
    }
    /// <summary>MainLoop</summary>
    public void Run()
    {
        // 계산대를 모두 돈다. (내가 들고 있던게 있으면 바로 담는다.)
        //   목록을 다 합친다.
        // 선반에 가서 다 담는다. (역순으로 돈다.)
        // 가방에 담아서 보낸다.
        while (true)
        {
            // 계산대를 모두 돈다.
            var totalOrderProducts = TakeAllOrders();

            if (totalOrderProducts.Any())
            {
                // 선반에 가서 다 담는다. (역순으로 간다)
                foreach (var groupIndex in _myShelfGroups.Reverse())
                {
                    PickFromShelf(groupIndex, totalOrderProducts);
                }

                // 3. 가방에 담아서 보낸다.
                foreach (var registerCell in _map.Registers)
                {
                    MoveAndPutInBag(registerCell);
                }
            }
        }
    }
    
    #region 계산대에서 주문을 받는다.
    Dictionary<Product, int> TakeAllOrders()
    {
        var totalOrderProducts = new Dictionary<Product, int>();
        foreach (var register in _map.Registers)
        {
            var currentOrderProducts = MoveAndTakeOrder(register);
            foreach (var (product, count) in currentOrderProducts)
            {
                totalOrderProducts[product] = totalOrderProducts.TryGetValue(product, out var totalCount) ? totalCount + count : count;
            }
        }
        return totalOrderProducts;
    }
    Dictionary<Product, int> MoveAndTakeOrder(MyCell registerCell)
    {
        var orderProducts = new Dictionary<Product, int>();
        GotoCasher(registerCell);
        var order = TakeOrder();
        if (order != null)
        {
            GotoFrontOfBag(registerCell);
            PutInBag(order);
            foreach (var item in order.GetProducts())
            {
                if (_myProducts.Contains(item))
                {
                    var remainCount = order.GetRemaining(item);
                    if (remainCount > 0)
                    {
                        orderProducts[item] = remainCount + (orderProducts.TryGetValue(item, out var count) ? count : 0);
                    }
                }
            }
        }
        return orderProducts;
    }
    #endregion
    
    #region 선반에 가서 담는다. PickFromShelf
    void PickFromShelf(int shelfGroupIndex, Dictionary<Product, int> totalOrderProducts)
    {
        var shelfGroup = _map.GetShelfGroup(shelfGroupIndex);
        // 이 그룹에서 꺼낼 물건이 있을 때만 들어간다.
        Product[] orderProducts = totalOrderProducts.Keys.ToArray();
        if (shelfGroup.ContainsProducts(orderProducts))
        {
            Goto(shelfGroup.BottomEnter);
            foreach (var pair in shelfGroup.TraverseForServing())
            {
                Goto(pair.Cell);
                if (RobotIsFull())
                    return;

                if (totalOrderProducts.TryGetValue(pair.Product, out var remainCount))
                {
                    robot.Turn(pair.Direction);
                    while (true)
                    {
                        if (RobotIsFull())
                            break;
                        if (HasEnoughProducts(pair.Product, remainCount))
                        {
                            break;
                        }
                        else
                        {
                            if (robot.Pick(pair.Product) == false)
                                break;
                        }
                    }
                }
            }
        }
    }
    
    /// <summary>로봇이 count만큼 들고 있는가?</summary>
    bool HasEnoughProducts(Product product, int count)
    {
        return robot.GetCarried().Count(x => x == product) >= count;
    }

    bool RobotIsFull()
    {
        return robot.GetCarried().Count() >= robot.GetCapacity();
    }
    #endregion

    #region PutInBag: 봉투에 주문한 상품을 담는다.
    /// <summary>봉투에 주문한 상품을 담는다.</summary>
    void MoveAndPutInBag(MyCell registerCell)
    {
        // 계산대로 이동
        GotoCasher(registerCell);
        // 주문을 한 번더 확인
        var order = TakeOrder();
        if (order != null)
        {
            // 장바구니 앞으로 이동
            GotoFrontOfBag(registerCell);
            
            // 물건을 담는다.
            PutInBag(order);
            
            // 나머지는 일단 버리자.
            order.RemoveRemaining();
        }
    }
    
    Dictionary<Product, int> PutInBag(Order order)
    {
        var putItems = new Dictionary<Product, int>();
        foreach (var item in order.GetProducts())
        {
            while (order.GetRemaining(item) > 0 && robot.Has(item))
            {
                robot.PutInBag(item);
                putItems[item] = putItems.TryGetValue(item, out var count) ? count + 1 : 1;
            }
        }
        return putItems;
    }
    #endregion

    #region TakeOrder
    private Order? TakeOrder()
    {
        var order = robot.TakeOrder();
        var customer = robot.GetCustomer();
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
    #endregion
    
    #region Goto
    void GotoCasher(MyCell registerCell)
    {
        Goto(new MyCell(registerCell.X, registerCell.Y + 1));
        robot.Turn(Direction.Down);
    }
    void GotoFrontOfBag(MyCell registerCell)
    {
        Goto(new MyCell(registerCell.X + 1, registerCell.Y + 1));
        robot.Turn(Direction.Down);
    }
    #endregion
}
