<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load "../basecode"
#load "./StoreMap"
#load "./PathManager"
#load "./StaticData"
#load "./RobotBase"

class ServingRobot : RobotBase
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
            new MyCell(3, 7),
            new MyCell(2, 7),
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
