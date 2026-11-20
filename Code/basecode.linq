<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

public interface Robot
{
    /// <summary>다음 타일로 한 칸 이동합니다. 길이 막혀 있으면 false입니다.</summary>
    bool Move(Direction direction);

    /// <summary>타일을 떠나지 않고 제자리에서 해당 방향으로 회전합니다.</summary>
    void Turn(Direction direction);

    /// <summary>로봇이 있는 타일의 y 좌표를 반환합니다.</summary>
    int GetY();

    /// <summary>로봇이 있는 타일의 x 좌표를 반환합니다.</summary>
    int GetX();

    /// <summary>로봇이 바라보는 방향을 반환합니다.</summary>
    Direction GetFacing();

    /// <summary>이 코드를 실행하는 로봇의 번호를 반환합니다. 첫 번째는 1, 두 번째는 2입니다.</summary>
    int GetNumber();

    /// <summary>해당 방향으로 이동할 수 있는지 나타냅니다. 타일이 존재하고 비어 있으며 손님이나 다른 로봇이 없어야 합니다.</summary>
    bool CanMove(Direction direction);

    /// <summary>해당 방향의 다음 타일에 있는 물체를 반환합니다. 비어 있으면 null입니다.</summary>
    Entity Look(Direction direction);

    /// <summary>로봇이 바라보는 바로 앞 타일에 이 개체가 있는지 나타냅니다.</summary>
    bool IsFacing(Entity entity);

    /// <summary>앞 진열대 또는 차고 문 앞의 트럭에서 상품 하나를 집습니다. 상품이 없거나 손이 가득 찼으면 false입니다.</summary>
    bool Pick(Product product);

    /// <summary>로봇이 현재 들고 있는 상품 수를 반환합니다.</summary>
    int GetLoad();

    /// <summary>로봇이 한 번에 들 수 있는 상품 수를 반환합니다. 업그레이드에서 늘릴 수 있습니다.</summary>
    int GetCapacity();

    /// <summary>로봇이 이 상품을 들고 있는지 나타냅니다.</summary>
    bool Has(Product product);

    /// <summary>현재 손에 든 상품을 집은 순서대로 상품 하나당 요소 하나인 리스트로 반환합니다. 손이 비었으면 빈 리스트이며, 사본을 변경해도 실제 손의 상품은 바뀌지 않습니다.</summary>
    List<Product> GetCarried();

    /// <summary>로봇 고유의 메모장 리스트를 반환합니다. 실행 간에 유지되고 게임과 함께 저장됩니다. 로봇 패널에서 확인하고 리스트 버튼으로 편집합니다.</summary>
    List<Product> GetList();

    /// <summary>로봇이 진열대를 보며 상품을 집을 위치에 서 있는지 나타냅니다. 이때만 집거나 놓을 수 있습니다. 탁자는 인접 타일, 선반과 냉장고는 앞쪽입니다.</summary>
    bool IsAtPickup();

    /// <summary>로봇 바로 앞의 진열대인 탁자, 선반 또는 냉장고를 반환합니다. 상품을 놓는 물체가 없으면 null입니다.</summary>
    Shelf GetShelf();

    /// <summary>손에 든 상품을 앞 진열대에 놓습니다. 들고 있지 않거나 진열대가 가득 찼으면 false입니다. 앞에 진열대가 없거나 다른 상품용이면 오류로 프로그램이 멈춥니다.</summary>
    bool PutOnShelf(Product product);

    /// <summary>문이 열린 트럭이 차고에 있고 로봇이 차고 문 앞에서 문을 바라보는지 나타냅니다. 이때 Pick으로 상품을 꺼냅니다.</summary>
    bool IsAtTruck();

    /// <summary>트럭에 남은 상품을 하나당 요소 하나인 리스트로 반환합니다. 로봇이 트럭 앞에 없으면 빈 리스트입니다. IsAtTruck으로 확인할 수 있습니다.</summary>
    List<Product> GetDelivery();

    /// <summary>로봇이 계산대 뒤 화면 앞에서 화면을 바라보는지 나타냅니다. 그곳에서 주문을 받습니다.</summary>
    bool IsAtRegister();

    /// <summary>로봇이 서 있는 계산대 화면 앞에서 기다리는 손님을 반환합니다. 화면 앞이 아니거나 손님이 없으면 null입니다.</summary>
    Customer GetCustomer();

    /// <summary>이 계산대 손님의 주문을 받습니다. 로봇이 화면 앞에 없거나 손님이 없으면 null입니다.</summary>
    Order TakeOrder();

    /// <summary>로봇이 계산대 뒤 봉투 앞에서 봉투를 바라보는지 나타냅니다. 그곳에서 상품을 봉투에 넣습니다.</summary>
    bool IsAtBag();

    /// <summary>손에 든 상품을 손님의 봉투에 넣습니다. 봉투 앞이 아니거나 상품을 안 들었거나 손님에게 필요 없으면 false입니다.</summary>
    bool PutInBag(Product product);
}

public interface Store
{
    /// <summary>매장 너비인 x 방향의 타일 수를 반환합니다.</summary>
    int GetWidth();

    /// <summary>매장 깊이인 앞 벽부터 뒤 벽까지 y 방향의 타일 수를 반환합니다.</summary>
    int GetHeight();

    /// <summary>타일 (x, y)가 매장 안에 있고 아무것도 없어 비어 있는지 나타냅니다.</summary>
    bool IsFree(int x, int y);

    /// <summary>타일 (x, y)에 있는 물체를 반환합니다. 비어 있거나 매장 밖이면 오류 없이 null입니다. x는 0부터 GetWidth() - 1, y는 0부터 GetHeight() - 1까지로 유지하세요.</summary>
    Entity GetEntity(int x, int y);

    /// <summary>타일 (x, y)의 계산대 화면이나 봉투에서 주문을 받을 위치인 손님 반대쪽 화면 뒤 타일을 반환합니다. 그곳에서 GetRegisterSide(x, y) 방향을 보세요. 계산대가 없거나 화면 뒤에 물체가 있으면 null입니다.</summary>
    Cell GetCashierCell(int x, int y);

    /// <summary>타일 (x, y)의 계산대 봉투에 상품을 넣을 위치인 봉투 뒤 타일을 반환합니다. 그곳에서 GetRegisterSide(x, y) 방향을 보세요. 계산대가 없거나 봉투 뒤에 물체가 있으면 null입니다.</summary>
    Cell GetPackerCell(int x, int y);

    /// <summary>타일 (x, y)의 계산대 뒤에서 볼 방향을 반환합니다. 화면과 봉투 모두 손님이 있는 계산대 쪽을 봅니다. 계산대가 없으면 null입니다.</summary>
    Direction? GetRegisterSide(int x, int y);

    /// <summary>타일 (x, y)의 계산대 화면이나 봉투가 속한 계산대 번호를 반환합니다. 패널에 표시된 1, 2, 3, 4 중 하나이며, 설치 시 가장 작은 빈 번호를 받고 옮겨도 유지됩니다. 계산대가 없으면 0입니다.</summary>
    int GetRegisterNumber(int x, int y);

    /// <summary>타일 (x, y)의 물체에 있는 상품 수를 반환합니다. 상품이 없거나 매장 밖이면 0입니다.</summary>
    int GetStock(int x, int y);

    /// <summary>이 개체가 놓인 첫 타일을 반환합니다. y 순서로 행을 살피고 각 행은 왼쪽부터 찾습니다. 없으면 null입니다.</summary>
    Cell Find(Entity entity);

    /// <summary>이 개체가 놓인 모든 타일을 y 순서로, 각 행에서는 왼쪽부터 찾아 반환합니다. 빵 진열대처럼 두 칸짜리는 두 타일 모두 포함합니다. 계산대는 화면이 REGISTER, 봉투가 BAG입니다.</summary>
    List<Cell> FindAll(Entity entity);

    /// <summary>타일 (x, y)에서 이 상품을 집을 수 있는 방향을 반환합니다. 보통 하나이며 같은 상품 진열대 사이에서는 두 방향입니다. 집을 수 없으면 빈 리스트입니다.</summary>
    List<Direction> GetPickupDirections(int x, int y, Product product);

    /// <summary>게임 시각의 시를 0부터 23까지로 반환합니다. 게임 하루는 실제 10분입니다.</summary>
    int GetHour();

    /// <summary>게임 시각의 분을 0부터 59까지로 반환합니다.</summary>
    int GetMinute();

    /// <summary>현재 날짜를 1부터 시작하는 값으로 반환합니다.</summary>
    int GetDay();

    /// <summary>현재 계절을 반환합니다. Summer, Autumn, Winter, Spring은 각각 10일이며 계절 상품은 해당 계절에만 주문할 수 있습니다.</summary>
    Season GetSeason();

    /// <summary>계산대에서 아직 주문을 접수하지 않은 손님 중 가장 먼저 떠날 손님을 반환합니다. 없으면 null입니다.</summary>
    Customer GetWaitingCustomer();

    /// <summary>매장의 보유 금액을 반환합니다.</summary>
    int GetMoney();

    /// <summary>상품 하나의 주문 비용을 반환합니다. 판매가의 절반이며 매입 할인으로 줄일 수 있습니다.</summary>
    int GetBuyPrice(Product product);

    /// <summary>트럭 적재량을 반환합니다. 한 번의 배송에는 이 수량까지만 주문할 수 있습니다.</summary>
    int GetTruckCapacity();

    /// <summary>배송을 요청하고 즉시 결제합니다. 리스트의 요소 하나당 상품 하나를 싣고 옵니다. 제철이 아닌 계절 상품은 오지 않고 하나당 $10의 벌금이 부과됩니다. 트럭이 사용 중이거나 리스트가 비었거나 돈이 부족하면 false이며 로봇 위에 이유가 표시됩니다. 적재량을 넘기면 오류이므로 주문을 나누세요.</summary>
    bool CallDelivery(List<Product> goods);

    /// <summary>요청한 배송에서 이 상품을 아직 트럭에서 내리지 않은 수량을 반환합니다. 이동 중과 차고에 있는 동안 유효하며, 모두 내렸거나 배송 요청이 없으면 0입니다.</summary>
    int GetRemaining(Product product);

    /// <summary>트럭이 차고에 있는지 나타냅니다. 막 도착하면 아직 문을 여는 중이므로 로봇의 IsAtTruck으로 상품을 꺼낼 수 있는지 확인하세요.</summary>
    bool IsTruckHere();

    /// <summary>트럭이 이동 중도 아니고 차고에도 없어 새로 호출할 수 있는지 나타냅니다.</summary>
    bool IsTruckAway();

    /// <summary>차고 문 바로 앞 타일의 x 좌표를 반환합니다. 이 타일에서 문을 바라보면 트럭 상품을 꺼낼 수 있습니다. 문 앞 두 타일 중 하나가 막혔으면 다른 타일을 사용합니다.</summary>
    int GetGarageX();

    /// <summary>GetGarageX와 같은 차고 문 앞 타일의 y 좌표를 반환합니다.</summary>
    int GetGarageY();

    /// <summary>차고에서 문 쪽을 바라볼 방향을 반환합니다.</summary>
    Direction GetGarageSide();

    /// <summary>다 내리지 않은 트럭을 보냅니다. 남은 상품 금액은 환불되며 빈 트럭은 자동 출발합니다. 차고에 트럭이 없으면 false입니다.</summary>
    bool SendTruckAway();
}

public interface Order
{
    /// <summary>주문의 상품 종류를 손님이 말한 순서대로 중복 없이 반환합니다.</summary>
    List<Product> GetProducts();

    /// <summary>손님이 이 상품을 원하는 수량을 반환합니다. 주문에 없으면 0입니다.</summary>
    int GetCount(Product product);

    /// <summary>이 상품 중 아직 봉투에 담지 않은 수량을 반환합니다.</summary>
    int GetRemaining(Product product);

    /// <summary>주문 전체의 상품 수량을 반환합니다.</summary>
    int GetTotal();

    /// <summary>모든 상품이 봉투에 담겼는지 나타냅니다. 전부 담으면 손님이 결제하고 떠납니다.</summary>
    bool IsDone();

    /// <summary>이 상품 중 아직 봉투에 담지 않은 수량을 전부 주문에서 제외합니다. 이미 담은 상품은 유지되어 결제되고 다른 상품은 그대로입니다. 제외할 것이 없으면 false입니다. 더 담을 것이 없으면 손님이 결제하고 떠나며 빈 봉투라면 빈손으로 떠납니다.</summary>
    bool Remove(Product product);

    /// <summary>아직 봉투에 담지 않은 모든 상품을 주문에서 제외합니다. 이미 담은 것은 유지되어 결제됩니다. 제외할 것이 없으면 false입니다. 손님은 봉투의 상품을 결제하고 떠나며 봉투가 비었으면 빈손으로 떠납니다.</summary>
    bool RemoveRemaining();
}

/// <summary>매장 타일을 나타냅니다. 세 API 문서에는 이 타입의 멤버가 정의되어 있지 않습니다.</summary>
public interface Cell
{
}

/// <summary>로봇 앞의 상품 진열대, 탁자, 선반 또는 냉장고를 나타냅니다. Robot.GetShelf()로 가져오며, 상품을 집거나 놓는 동작은 로봇의 Pick과 PutOnShelf로 수행합니다.</summary>
public interface Shelf
{
    /// <summary>이 진열대에 놓이는 상품을 반환합니다. 해당 상품만 놓을 수 있습니다.</summary>
    Product GetProduct();

    /// <summary>현재 진열대의 상품 수를 반환합니다. 로봇이 이 진열대의 상품을 집을 위치에서 진열대를 바라보고 있어야 하며, 아니면 오류입니다. 멀리 있는 진열대의 재고는 Store.GetStock(x, y)로 확인합니다.</summary>
    int GetCount();

    /// <summary>진열대가 가득 찼을 때의 상품 수를 반환합니다. 로봇의 운반 용량과는 다릅니다.</summary>
    int GetCapacity();

    /// <summary>진열대가 가득 차 상품을 더 놓을 수 없는지 나타냅니다. 로봇이 이 진열대의 상품을 집을 위치에서 진열대를 바라보고 있어야 하며, 아니면 오류입니다.</summary>
    bool IsFull();

    /// <summary>진열대가 비어 집을 상품이 없는지 나타냅니다. 로봇이 이 진열대의 상품을 집을 위치에서 진열대를 바라보고 있어야 하며, 아니면 오류입니다.</summary>
    bool IsEmpty();
}

/// <summary>손님을 나타냅니다. 세 API 문서에는 이 타입의 전체 멤버와 시그니처가 정의되어 있지 않습니다.</summary>
public interface Customer
{
    bool IsAdult();
}

/// <summary>매장 타일에 놓인 물체의 종류를 나타냅니다. 큰 물체는 여러 타일에서 같은 값을 반환합니다.</summary>
public enum Entity
{
    /// <summary>토마토를 진열하는 탁자입니다.</summary>
    TOMATO_TABLE,
    /// <summary>빵을 진열하는 두 칸짜리 선반입니다.</summary>
    BREAD_SHELF,
    /// <summary>상품을 보관하고 판매하는 냉장고입니다.</summary>
    FRIDGE,
    /// <summary>손님의 주문을 받고 결제하는 계산대 화면입니다.</summary>
    REGISTER,
    /// <summary>계산대의 봉투가 놓인 타일입니다.</summary>
    BAG,
}
public enum Season
{
    Summer,
    Autumn,
    Winter,
    Spring,
}

/// <summary>매장에서 취급하는 상품입니다. 각 정적 멤버는 해당 상품 객체를 나타냅니다.</summary>
public interface Product
{
    /// <summary>이 상품이 주류인지 나타냅니다.</summary>
    bool IsAlcohol();
    /// <summary>미성년자에게 판매할 수 없는 상품인지 나타냅니다. 주류와 담배가 해당합니다.</summary>
    bool IsAdultsOnly();
    /// <summary>스테이크나 트러플 같은 프리미엄 상품인지 나타냅니다.</summary>
    bool IsPremium();
    /// <summary>자판기에서 판매하는 상품인지 나타냅니다.</summary>
    bool IsVending();
    /// <summary>커피처럼 무한 공급되어 재고 보충이 필요 없는 상품인지 나타냅니다.</summary>
    bool IsInfinite();
    /// <summary>캐비아나 바닷가재 같은 최고급 상품인지 나타냅니다.</summary>
    bool IsElite();
    /// <summary>상품의 기본 판매 가격을 반환합니다. 매장 배율이 적용된 실제 판매 금액과는 다릅니다.</summary>
    int GetPrice();
    /// <summary>게임 언어로 표시되는 상품 이름을 반환합니다.</summary>
    string GetName();
    /// <summary>이 상품이 진열되는 물체의 종류를 반환합니다.</summary>
    Entity GetRack();
    /// <summary>주문 가능한 계절을 반환합니다. 사계절 상품이면 null입니다.</summary>
    Season? GetSeason();
    /// <summary>현재 계절에 주문할 수 있는 상품인지 나타냅니다.</summary>
    bool IsInSeason();

    /// <summary>토마토.</summary>
    static Product Tomato;
    /// <summary>호박.</summary>
    static Product Pumpkin;
    /// <summary>빵.</summary>
    static Product Bread;
    /// <summary>청량음료.</summary>
    static Product Soda;
    /// <summary>맥주. 성인 전용 주류입니다.</summary>
    static Product Beer;
    /// <summary>담배. 성인 전용 상품입니다.</summary>
    static Product Cigarettes;
    /// <summary>와인. 성인 전용 주류입니다.</summary>
    static Product Wine;
    /// <summary>우유.</summary>
    static Product Milk;
    /// <summary>튤립. 봄에만 배송 주문할 수 있습니다.</summary>
    static Product Tulips;
    /// <summary>꿀. 가을에만 배송 주문할 수 있습니다.</summary>
    static Product Honey;
    /// <summary>수박. 여름에만 배송 주문할 수 있습니다.</summary>
    static Product Watermelon;
    /// <summary>귤. 겨울에만 배송 주문할 수 있습니다.</summary>
    static Product Tangerine;
    /// <summary>딸기. 여름에만 배송 주문할 수 있습니다.</summary>
    static Product Strawberry;
    /// <summary>부활절 케이크. 봄에만 배송 주문할 수 있습니다.</summary>
    static Product Eastercake;
    /// <summary>불꽃놀이. 겨울에만 배송 주문할 수 있습니다.</summary>
    static Product Fireworks;
    /// <summary>판매용 크리스마스트리. 겨울에만 배송 주문할 수 있습니다.</summary>
    static Product Tree;
    /// <summary>마블링 스테이크. 프리미엄 상품입니다.</summary>
    static Product Steak;
    /// <summary>트러플. 프리미엄 상품입니다.</summary>
    static Product Truffle;
    /// <summary>블랙 캐비아. 최고급 상품입니다.</summary>
    static Product Caviar;
    /// <summary>바닷가재. 최고급 상품입니다.</summary>
    static Product Lobster;
    /// <summary>초콜릿. 자판기에서 판매합니다.</summary>
    static Product Chocolate;
    /// <summary>팝콘. 자판기에서 판매합니다.</summary>
    static Product Popcorn;
    /// <summary>커피. 커피 머신에서 무한 공급되어 보충할 필요가 없습니다.</summary>
    static Product Coffee;
    /// <summary>아이스크림. 여름에만 배송 주문할 수 있습니다.</summary>
    static Product Icecream;
    /// <summary>잼. 가을에만 배송 주문할 수 있습니다.</summary>
    static Product Jam;
    /// <summary>비료. 봄에만 배송 주문할 수 있습니다.</summary>
    static Product Fertilizer;
    /// <summary>가면. 가을에만 배송 주문할 수 있습니다.</summary>
    static Product Mask;
    /// <summary>위스키. 성인 전용 주류이며 프리미엄 상품입니다.</summary>
    static Product Whisky;
    /// <summary>코냑. 성인 전용 주류이며 최고급 상품입니다.</summary>
    static Product Cognac;
}
public enum Direction
{
    Up,
    Down,
    Left,
    Right,
}

public static Robot robot;
public static Store store;
