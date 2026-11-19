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
                GetFromShelfWithOrder(order);
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
                    CallDeliveryNotEnoughFromShelf();
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
        }
    }


void CallDeliveryNotEnoughFromShelf()
{
    // 일단 다 선반에 넣고 생각하자.
    PutItemToShelfAll();
    // 아직 손에 있는게 있을 수 있다.
    var beforeCarried = robot.GetCarried().Count();
    robot.GetList().Clear();
    // 다 꺼내본다. 4개 꺼내야 하는데 덜 꺼내지면 그 만큼 부족하다는 것이다.
    CountingNotEnoughFromShelf(Product.Tomato, beforeCarried);
    if (robot.GetList().Count() < 3)
    {
        CountingNotEnoughFromShelf(Product.Pumpkin, beforeCarried);
    }
    CallDelivery();
    robot.GetList().Clear();
}

void CountingNotEnoughFromShelf(Product product, int beforeCarried)
{
    GotoShelf(product);
    for (var i = beforeCarried; i < 4; i++)
    {
        if (robot.Pick(product) == false)
        {
            if (robot.GetCapacity() == robot.GetCarried().Count())
            {
                // 로봇이 더 이상 못 집는 경우.
                // 이 제품은 일단 주문하지 말자.
                // 다시 돌려놓고 끝낸다.
                PutItemToShelfWithProduct(product);
                return;
            }
            else
            {
                // 선반에 물건이 부족한 경우.
                // i = 0 이라는 뜻은 선반에 아무것도 없었다는 뜻?
                for (var j = i + 1; j < 4; j++)
                {
                    if (robot.GetList().Count() < 3)
                    {
                        robot.GetList().Add(product);
                    }
                    if (robot.GetList().Count() >= 3)
                    {
                        PutItemToShelfWithProduct(product);
                        return;
                    }
                }
            }
        }
    }
    PutItemToShelfWithProduct(product);
}

/// <summary>내가 가지고 있는 것을 선반에 넣는다.</summary>
void PutItemToShelfAll()
{
    PutItemToShelfWithProduct(Product.Tomato);
    PutItemToShelfWithProduct(Product.Pumpkin);
}

void PutItemToShelfWithProduct(Product product)
{
    GotoShelf(product);
    foreach(var item in robot.GetCarried())
    {
        if (item == product)
        {
            robot.PutOnShelf(product);
        }
    }
}

int PickItemFromDelivery()
{
    var tomato = PickItemFromDeliveryWithProduct(Product.Tomato);
    var pumpkin = PickItemFromDeliveryWithProduct(Product.Pumpkin);
    return tomato + pumpkin;
}
int PickItemFromDeliveryWithProduct(Product product)
{
    GotoDelivery();
    int pickCount = 0;
    for (var i = 0; i < 3; i++)
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
            else if (robot.GetCapacity() == robot.GetCarried().Count())
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

int GetOrderRemaining(Order order)
{
    var tomato = order.GetRemaining(Product.Tomato);
    var pumpkin = order.GetRemaining(Product.Pumpkin);
    return tomato + pumpkin;
}

void GetFromShelfWithOrder(Order order)
{
    GetFromShelfWithProduct(order, Product.Tomato);
    GetFromShelfWithProduct(order, Product.Pumpkin);
}

/// <summary>선반에서 주문한 수량의 일부라도 가져올 수 있는지 본다.</summary>
void GetFromShelfWithProduct(Order order, Product product)
{
    if (order.GetRemaining(product) > 0)
    {
        var result = GetFromShelfWithCount(product, order.GetRemaining(product));
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
int GetFromShelfWithCount(Product product, int count)
{
    GotoShelf(product);
    for (var i = 0; i < count; i++)
    {
        if (robot.Pick(product) == false)
        {
            if (robot.GetCapacity() == robot.GetCarried().Count())
            {
                return 1; // 로봇이 더 이상 들 수 없다.
            }
            else
            {
                return 2; // 선반이 비었다.
            }
        }
    }
    return 0; // 원하는 만큼 얻었다!
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
    return false;
}

/// <summary>주문내역 중 로봇이 가지고 있는게 있는지 본다.</summary>
bool HasSomeItemWithProduct(Order order, Product product)
{
    var myCount = 0;
    foreach (var item in robot.GetCarried())
    {
        if (item == product)
        {
            myCount += 1;
        }
    }

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

/// <summary>주문 내역을 로봇이 전부 가지고 있는지 본다.</summary>
bool HasItemWithOrder(Order order)
{
    if (HasItemProduct(order, Product.Tomato))
    {
        if (HasItemProduct(order, Product.Pumpkin))
        {
            return true;
        }
    }
    return false;
}

/// <summary>주문 내역을 로봇이 전부 가지고 있는지 본다.</summary>
bool HasItemProduct(Order order, Product product)
{
    var myCount = 0;
    foreach (var item in robot.GetCarried())
    {
        if (item == product)
        {
            myCount += 1;
        }
    }

    return myCount >= order.GetRemaining(product);
}

Order TakeOrder()
{
    GotoRegister();
    var order = robot.TakeOrder();
    return order;
}

void PutInBagWithOrder(Order order)
{
    GotoBag();
    PutInBagWithProduct(order, Product.Tomato);
    PutInBagWithProduct(order, Product.Pumpkin);
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

bool PickGoal(Product product, int goal)
{
    while (true)
    {
        var hasProduct = 0;
        foreach (var carriedItem in robot.GetCarried())
        {
            if (carriedItem == product)
            {
                hasProduct += 1;
            }
        }
        if (hasProduct == goal)
        {
            return true;
        }
        else
        {
            if (robot.Pick(product) == false)
            {
                return false;
            }
        }
    }
}

void GotoDelivery()
{
    Goto(3, 3);
    robot.Turn(Direction.Up);
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

void Goto(int x, int y)
{
    while (robot.GetX() < x)
    {
        robot.Move(Direction.Right);
    }
    while (robot.GetX() > x)
    {
        robot.Move(Direction.Left);
    }
    while (robot.GetY() < y)
    {
        robot.Move(Direction.Up);
    }
    while (robot.GetY() > y)
    {
        robot.Move(Direction.Down);
    }
}