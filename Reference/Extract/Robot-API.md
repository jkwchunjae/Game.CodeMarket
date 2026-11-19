# Robot API (25개)

게임 DLL의 등록 정보에서 추출한 시그니처입니다. 설명의 한국어는 기존 번역표를 사용했습니다.

| 명령 | 반환형 | 종류 | 설명 |
|---|---|---|---|
| `move(Direction direction)` | `boolean` | Action | 다음 타일로 한 칸 이동합니다. 길이 막혀 있으면 false입니다 |
| `turn(Direction direction)` | `void` | Action | 타일을 떠나지 않고 제자리에서 해당 방향으로 회전합니다 |
| `getX()` | `int` | Query | 로봇이 있는 타일의 x 좌표 |
| `getY()` | `int` | Query | 로봇이 있는 타일의 y 좌표 |
| `getFacing()` | `Direction` | Query | 로봇이 바라보는 방향 |
| `getNumber()` | `int` | Query | 이 코드를 실행하는 로봇의 번호입니다. 첫 번째는 1, 두 번째는 2입니다 |
| `canMove(Direction direction)` | `boolean` | Query | 해당 방향으로 이동할 수 있는지 나타냅니다. 타일이 존재하고 비어 있으며 손님이나 다른 로봇이 없어야 합니다 |
| `look(Direction direction)` | `Entity` | Query | 해당 방향의 다음 타일에 있는 물체입니다. 비어 있으면 null입니다 |
| `isFacing(Entity entity)` | `boolean` | Query | 로봇이 바라보는 바로 앞 타일에 이 개체가 있는지 나타냅니다 |
| `pick(Product product)` | `boolean` | Action | 앞 진열대 또는 차고 문 앞의 트럭에서 상품 하나를 집습니다. 상품이 없거나 손이 가득 찼으면 false입니다 |
| `getLoad()` | `int` | Query | 로봇이 현재 들고 있는 상품 수 |
| `getCapacity()` | `int` | Query | 로봇이 한 번에 들 수 있는 상품 수입니다. “업그레이드”에서 늘릴 수 있습니다 |
| `has(Product product)` | `boolean` | Query | 로봇이 이 상품을 들고 있는지 나타냅니다 |
| `getCarried()` | `List<Product>` | Query | 현재 손에 든 상품의 리스트입니다. 집은 순서대로 상품 하나당 요소 하나이며 손이 비었으면 빈 리스트입니다. 사본이므로 변경해도 실제 손의 상품은 바뀌지 않습니다 |
| `getList()` | `List<Product>` | Query | 로봇 고유의 메모장 리스트입니다. 실행 간에 유지되고 게임과 함께 저장됩니다. 로봇 패널에서 확인하고 리스트 버튼으로 편집합니다 |
| `isAtPickup()` | `boolean` | Query | 로봇이 진열대를 보며 상품을 집을 위치에 서 있는지 나타냅니다. 이때만 집거나 놓을 수 있습니다(탁자는 인접 타일, 선반과 냉장고는 앞쪽) |
| `getShelf()` | `Shelf` | Query | 로봇 바로 앞의 진열대(탁자, 선반, 냉장고)입니다. 상품을 놓는 물체가 없으면 null입니다 |
| `putOnShelf(Product product)` | `boolean` | Action | 손에 든 상품을 앞 진열대에 놓습니다. 들고 있지 않거나 진열대가 가득 찼으면 false입니다. 앞에 진열대가 없거나 다른 상품용이면 오류로 프로그램이 멈춥니다 |
| `isAtTruck()` | `boolean` | Query | 문이 열린 트럭이 차고에 있고 로봇이 차고 문 앞에서 문을 바라보는지 나타냅니다. 이때 robot.pick으로 상품을 꺼냅니다 |
| `getDelivery()` | `List<Product>` | Query | 트럭에 남은 상품을 하나당 요소 하나로 반환합니다. 로봇이 트럭 앞에 없으면 빈 리스트입니다(robot.isAtTruck()으로 확인) |
| `isAtRegister()` | `boolean` | Query | 로봇이 계산대 뒤 화면 앞에서 화면을 바라보는지 나타냅니다. 그곳에서 주문을 받습니다 |
| `getCustomer()` | `Customer` | Query | 로봇이 서 있는 계산대 화면 앞에서 기다리는 손님입니다. 화면 앞이 아니거나 손님이 없으면 null입니다. isAdult()로 성인인지 확인할 수 있습니다 |
| `takeOrder()` | `Order` | Action | 이 계산대 손님의 주문을 받습니다. 로봇이 화면 앞에 없거나 손님이 없으면 null입니다 |
| `isAtBag()` | `boolean` | Query | 로봇이 계산대 뒤 봉투 앞에서 봉투를 바라보는지 나타냅니다. 그곳에서 상품을 봉투에 넣습니다 |
| `putInBag(Product product)` | `boolean` | Action | 손에 든 상품을 손님의 봉투에 넣습니다. 봉투 앞이 아니거나 상품을 안 들었거나 손님에게 필요 없으면 false입니다 |
