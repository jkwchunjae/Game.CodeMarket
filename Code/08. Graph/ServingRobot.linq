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
                robot.Move(Direction.Down);

                // 3. 가방에 담아서 보낸다.
                //foreach (var registerCell in _map.Registers)
                //{
                //    MoveAndPutInBag(registerCell);
                //}
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
        var newOrders = AddMoreProduct(totalOrderProducts);
        return newOrders;
    }
    Dictionary<Product, int> MoveAndTakeOrder(MyCell registerCell)
    {
        var orderProducts = new Dictionary<Product, int>();
        GotoCasher(registerCell);
        var order = TakeOrder();
        while (order == null)
        {
            // 주문받기 성공할 때 까지 계속 고객을 기다린다.
            order = TakeOrder();
        }
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
    /// <summary>주문 받은 것 보다 더 들고 있어보자. 첫 줄 5개씩, 두번째 줄 3개씩</summary>
    Dictionary<Product, int> AddMoreProduct(Dictionary<Product, int> totalOrders)
    {
        Dictionary<Product, int> moreProducts = new Dictionary<Product, int>();
        
        foreach (var shelfGroupIndex in _myShelfGroups)
        {
            if (shelfGroupIndex == 3)
                continue; // 여기는 명품관.
            var shelfGroup = _map.GetShelfGroup(shelfGroupIndex);
            var topShelfProducts = new List<Product>(); // 앞쪽 배치된 상품 기록. 3, 4 번째에서 중복 담기를 막기 위해.
            for (var i = 0; i < 2; i++)
            {
                var y = shelfGroup.BottomEnter.Y + i;
                var leftX = shelfGroup.BottomEnter.X;
                var rightX = shelfGroup.TopEnter.X;
                foreach (var x in new[] { leftX, rightX })
                {
                    var pair = shelfGroup.FindPair(x, y);
                    if (pair != null)
                    {
                        if (pair.Product.GetSeason() == null)
                        {
                            moreProducts[pair.Product] = (i == 0 ? 5 : 3);
                        }
                        else if (pair.Product.IsInSeason())
                        {
                            moreProducts[pair.Product] = moreProducts.TryGetValue(pair.Product, out var count) ? count + 3 : 3;
                        }
                    }
                }
            }
            for (var i = 2; i < 4; i++)
            {
                var y = shelfGroup.BottomEnter.Y + i;
                var leftX = shelfGroup.BottomEnter.X;
                var rightX = shelfGroup.TopEnter.X;
                foreach (var x in new[] { leftX, rightX })
                {
                    var pair = shelfGroup.FindPair(x, y);
                    if (pair != null && topShelfProducts.Contains(pair.Product) == false)
                    {
                        if (pair.Product.GetSeason() == null && pair.Product.IsInSeason())
                        {
                            // 시즈널 상품은 3개 더 남는다.
                            moreProducts[pair.Product] = moreProducts.TryGetValue(pair.Product, out var count) ? count + 3 : 3;
                        }
                    }
                }
            }
        }
        
        var newOrders = new Dictionary<Product, int>(totalOrders);
        foreach (var (product, count) in moreProducts)
        {
            if (newOrders.TryGetValue(product, out var orderCount))
            {
                newOrders[product] = Math.Max(orderCount, count);
            }
            else
            {
                newOrders[product] = count;
            }
        }
        return newOrders;
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
            Goto(shelfGroup.BottomEnter, true);
            var traverse = shelfGroup.TraverseForServing(totalOrderProducts);
            foreach (var pair in traverse)
            {
                Goto(pair.Cell);
                if (RobotIsFull())
                    return;

                if (totalOrderProducts.TryGetValue(pair.Product, out var remainCount))
                {
                    if (HasEnoughProducts(pair.Product, remainCount))
                        continue;
                    if (pair.ShelfIsEmpty())
                        continue;

                    robot.Turn(pair.Direction);
                    while (true)
                    {
                        if (RobotIsFull())
                            break;
                        if (HasEnoughProducts(pair.Product, remainCount))
                            break;
                        if (pair.ShelfIsEmpty())
                            break;
                        if (robot.Pick(pair.Product) == false)
                        {
                            Console.Error.WriteLine($"cell: {pair.Cell.ToStr()}, shelf: {pair.ShelfPosition.ToStr()}");
                            break;
                        }
                    }
                }
            }
        }
    }
    
    bool NeedMoreShelf(Dictionary<Product, int> totalOrderProducts, CellProductPair[] leftShelfs)
    {
        var myCarried = robot.GetCarried()
            .GroupBy(p => p)
            .ToDictionary(p => p.Key, p => p.Count());
            
        #region leftOrders: 더 담아야 할 상품 수 계산
        var leftOrders = new Dictionary<Product, int>();
        foreach (var (product, orderCount) in totalOrderProducts)
        {
            if (myCarried.TryGetValue(product, out var hasCount))
            {
                if (hasCount >= orderCount)
                {
                    // 이 상품은 다 담았다.
                }
                else
                {
                    var needCount = orderCount - hasCount;
                    leftOrders[product] = needCount;
                }
            }
        }
        #endregion
        
        // 남은 선반 중 더 담아야 할 상품이 하나라도 있으면 true !
        foreach (var pair in leftShelfs)
        {
            if (leftOrders.TryGetValue(pair.Product, out var needCount))
            {
                return true;
            }
        }
        return false;
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
        RemoveItem(order, item =>
        {
            return robot.Has(item) == false && item.IsInSeason() == false;
        });
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
