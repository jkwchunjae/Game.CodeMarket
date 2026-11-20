<Query Kind="Statements" />

#load "../basecode"

//Main();

void Main()
{
    // 주문을 받는다.
    //    배송 
    //    주문이 있으면
    //      내 손에 있는 것을 먼저 넣어준다.
    //      선반에서 꺼내서 준다.
    //    주문이 없으면
    //       선반을 채워넣는다.
    //       배송 요청을 한다.

    // 배송
    // 선반에 남는 자리

    while (true)
    {
        // 주문을 받는다.
        var order = TakeOrder();
        if (order != null) // 주문이 있으면
        {

            // 남는 것이 있을 때 까지 계속 뭔가 한다.
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
                if (robot.GetCarried().Count() > 0)
                {
                    // 손에 있는 것을 선반에 넣는다.
                    PutItemToShelfAll();
                }
                // 배송지에 가서 한 번만 확인해본다.
                if (PickItemFromDelivery() == 0)
                {
                    CallDeliveryNotEnoughFromShelfWithOrder(order);
                }
                else
                {
                    if (HasSomeItemOrder(order))
                    {
                        // 일단 손에 있는 것을 먼저 넣어준다.
                        PutInBagWithOrder(order);
                    }
                    PutItemToShelfAll();
                }
            }
            order.RemoveRemaining();
        }
        else
        {
            // 배송지에 가서 한 번만 확인해본다.
            if (PickItemFromDelivery() == 0)
            {
                CallDeliveryNotEnoughFromShelf();
            }
            else
            {
                PutItemToShelfAll();
            }
        }
    }
}

/// <summary>선반에 부족한게 있으면 배송. 주문에 맥주가 있으면 배송</summary>
void CallDeliveryNotEnoughFromShelfWithOrder(Order order)
{
    if (store.IsTruckAway())
    {
        robot.GetList().Clear();

        //if (order.GetRemaining(Product.Beer) > 0)
        //{
        //    for (var i = 0; i < order.GetRemaining(Product.Beer); i++)
        //    {
        //        robot.GetList().Add(Product.Beer);
        //    }
        //}

        CountingNotEnoughFromShelf(Product.Tomato);
        CountingNotEnoughFromShelf(Product.Pumpkin);
        CountingNotEnoughFromShelf(Product.Bread);
        CountingNotEnoughFromShelf(Product.Soda);
        CountingNotEnoughFromShelf(Product.Milk);
        CountingNotEnoughFromShelf(Product.Beer);

        CallDelivery();
        robot.GetList().Clear();
    }
}
/// <summary>선반에 부족한게 있으면 배송.</summary>
void CallDeliveryNotEnoughFromShelf()
{
    if (store.IsTruckAway())
    {
        robot.GetList().Clear();

        CountingNotEnoughFromShelf(Product.Tomato);
        CountingNotEnoughFromShelf(Product.Pumpkin);
        CountingNotEnoughFromShelf(Product.Bread);
        CountingNotEnoughFromShelf(Product.Beer);
        CountingNotEnoughFromShelf(Product.Soda);
        CountingNotEnoughFromShelf(Product.Milk);

        CallDelivery();
        robot.GetList().Clear();
    }
}

void CountingNotEnoughFromShelf(Product product)
{
    var truckCapacity = store.GetTruckCapacity();
    if (robot.GetList().Count() >= truckCapacity)
        return;

    GotoShelf(product);
    var shelf = robot.GetShelf();
    for (var i = shelf.GetCount(); i < shelf.GetCapacity(); i++)
    {
        robot.GetList().Add(product);
        if (robot.GetList().Count() >= truckCapacity)
            return;
    }
}

/// <summary>내가 가지고 있는 것을 선반에 넣는다.</summary>
void PutItemToShelfAll()
{
    if (robot.GetCarried().Count() > 0)
    {
        PutItemToShelfWithProduct(Product.Tomato);
        PutItemToShelfWithProduct(Product.Pumpkin);
        PutItemToShelfWithProduct(Product.Bread);
        PutItemToShelfWithProduct(Product.Beer);
        PutItemToShelfWithProduct(Product.Soda);
        PutItemToShelfWithProduct(Product.Milk);
    }
}

void PutItemToShelfWithProduct(Product product)
{
    if (CarriedCount(product) > 0)
    {
        GotoShelf(product);
        foreach (var item in robot.GetCarried())
        {
            if (item == product)
            {
                robot.PutOnShelf(product);
            }
        }
    }
}

/// <summary> 트럭에서 상품을 꺼낸다. </summary>
int PickItemFromDelivery()
{
    if (store.IsTruckHere())
    {
        GotoDelivery();
        while (true)
        {
            if (store.IsTruckHere())
            {
                if (robot.IsAtTruck())
                    break;
            }
            else
            {
                return 0;
            }
        }

        var tomato = PickItemFromDeliveryWithProduct(Product.Tomato);
        var pumpkin = PickItemFromDeliveryWithProduct(Product.Pumpkin);
        var bread = PickItemFromDeliveryWithProduct(Product.Bread);
        var soda = PickItemFromDeliveryWithProduct(Product.Soda);
        var milk = PickItemFromDeliveryWithProduct(Product.Milk);
        var beer = PickItemFromDeliveryWithProduct(Product.Beer);
        return tomato + pumpkin + bread + soda + milk + beer;
    }
    else
    {
        return 0;
    }
}
int PickItemFromDeliveryWithProduct(Product product)
{
    GotoDelivery();
    int pickCount = 0;
    for (var i = 0; i < store.GetTruckCapacity(); i++)
    {
        if (robot.Pick(product))
        {
            pickCount += 1;
        }
        else
        {
            if (i == 0)
            {
                // 아직 배송차량이 안 왔다.
                return pickCount;
            }
            // 내 손이 꽉차서 더 못 가져오는 경우
            else if (RobotIsFull())
            {
                return pickCount;
            }
            // 더 이상 받아올게 없다.
            else
            {
            }
        }
    }
    return pickCount;
}

/// <summary>상품을 배송한다.</summary>
bool CallDelivery()
{
    if (store.CallDelivery(robot.GetList()))
    {
        return true;
    }
    else
    {
        return false;
    }
}

/// <summary> 주문에서 남은 수량을 확인한다. </summary>
int GetOrderRemaining(Order order)
{
    var tomato = order.GetRemaining(Product.Tomato);
    var pumpkin = order.GetRemaining(Product.Pumpkin);
    var bread = order.GetRemaining(Product.Bread);
    var soda = order.GetRemaining(Product.Soda);
    var milk = order.GetRemaining(Product.Milk);
    var beer = order.GetRemaining(Product.Beer);
    return tomato + pumpkin + bread + soda + milk + beer;
}

/// <summary> 선반에서 주문에 필요한 만큼 담는다. </summary>
void PickFromShelfWithOrder(Order order)
{
    PickFromShelfWithProduct(order, Product.Tomato);
    PickFromShelfWithProduct(order, Product.Pumpkin);
    PickFromShelfWithProduct(order, Product.Bread);
    PickFromShelfWithProduct(order, Product.Beer);
    PickFromShelfWithProduct(order, Product.Soda);
    PickFromShelfWithProduct(order, Product.Milk);
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

/// <summary>주문내역 중 로봇이 가지고 있는게 있는지 본다.</summary>
bool HasSomeItemOrder(Order order)
{
    if (HasSomeItemWithProduct(order, Product.Tomato))
    {
        return true;
    }
    if (HasSomeItemWithProduct(order, Product.Pumpkin))
    {
        return true;
    }
    if (HasSomeItemWithProduct(order, Product.Bread))
    {
        return true;
    }
    if (HasSomeItemWithProduct(order, Product.Soda))
    {
        return true;
    }
    if (HasSomeItemWithProduct(order, Product.Milk))
    {
        return true;
    }
    if (HasSomeItemWithProduct(order, Product.Beer))
    {
        return true;
    }
    return false;
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

bool RobotIsFull()
{
    return robot.GetCapacity() >= robot.GetCarried().Count();
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
/// <summary>주문을 받고, 올바른 내용물만 남긴다. 시간, 계절, 성인</summary>
Order TakeOrder()
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

/// <summary>장바구니에 가서 손에 든 것을 내려 놓는다.</summary>
void PutInBagWithOrder(Order order)
{
    GotoBag();
    PutInBagWithProduct(order, Product.Tomato);
    PutInBagWithProduct(order, Product.Pumpkin);
    PutInBagWithProduct(order, Product.Bread);
    PutInBagWithProduct(order, Product.Soda);
    PutInBagWithProduct(order, Product.Milk);
    PutInBagWithProduct(order, Product.Beer);
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

void GotoDelivery()
{
    Goto(store.GetGarageX(), store.GetGarageY());
    robot.Turn(store.GetGarageSide());
}

void GotoRegister()
{
    Goto(2, 2);
    robot.Turn(Direction.Down);
}

void GotoBag()
{
    Goto(3, 2);
    robot.Turn(Direction.Down);
}

void GotoShelf(Product product)
{
    if (product == Product.Tomato)
    {
        GotoTomatoShelf();
    }
    else if (product == Product.Pumpkin)
    {
        GotoPumpkinShelf();
    }
    else if (product == Product.Bread)
    {
        GotoBreadShelf();
    }
    else if (product == Product.Soda)
    {
        GotoSodaShelf();
    }
    else if (product == Product.Milk)
    {
        GotoMilkShelf();
    }
    else if (product == Product.Beer)
    {
        GotoBeerShelf();
    }
}

void GotoTomatoShelf()
{
    Goto(1, 3);
    robot.Turn(Direction.Left);
}

void GotoPumpkinShelf()
{
    Goto(1, 2);
    robot.Turn(Direction.Left);
}

void GotoBreadShelf()
{
    Goto(1, 1);
    robot.Turn(Direction.Left);
}

void GotoSodaShelf()
{
    Goto(3, 3);
    robot.Turn(Direction.Right);
}

void GotoMilkShelf()
{
    Goto(3, 2);
    robot.Turn(Direction.Right);
}

void GotoBeerShelf()
{
    Goto(3, 4);
    robot.Turn(Direction.Right);
}

void Goto(int x, int y)
{
    while (true)
    {
        if (robot.GetX() == x)
        {
            if (robot.GetY() == y)
            {
                // 도착할 때 까지 계속 움직여본다.
                return;
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
}