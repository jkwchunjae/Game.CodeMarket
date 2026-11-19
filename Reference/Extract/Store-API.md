# Store API (28개)

게임 DLL의 등록 정보에서 추출한 시그니처입니다. 설명의 한국어는 기존 번역표를 사용했습니다.

| 명령 | 반환형 | 종류 | 설명 |
|---|---|---|---|
| `getWidth()` | `int` | Query | 매장 너비(타일 수): x 방향의 칸 수 |
| `getHeight()` | `int` | Query | 매장 깊이(타일 수): 앞 벽부터 뒤 벽까지 y 방향의 칸 수 |
| `isFree(int x, int y)` | `boolean` | Query | 타일 (x, y)가 매장 안에 있고 아무것도 없어 비어 있는지 나타냅니다 |
| `getEntity(int x, int y)` | `Entity` | Query | 타일 (x, y)에 있는 물체입니다. 비어 있으면 null입니다. 매장 밖도 오류 없이 null이므로 x는 0~getWidth() − 1, y는 0~getHeight() − 1로 유지하세요 |
| `getCashierCell(int x, int y)` | `Cell` | Query | 타일 (x, y)의 계산대 화면이나 봉투에서 주문을 받을 위치, 즉 손님 반대쪽 화면 뒤 타일입니다. 그곳에서 store.getRegisterSide(x, y) 방향을 보세요. 계산대가 없거나 화면 뒤에 물체가 있으면 null입니다 |
| `getPackerCell(int x, int y)` | `Cell` | Query | 타일 (x, y)의 계산대 봉투에 상품을 넣을 위치, 즉 봉투 뒤 타일입니다. 그곳에서 store.getRegisterSide(x, y) 방향을 보세요. 계산대가 없거나 봉투 뒤에 물체가 있으면 null입니다 |
| `getRegisterSide(int x, int y)` | `Direction` | Query | 타일 (x, y)의 계산대 뒤에서 볼 방향입니다. 화면과 봉투 모두 손님이 있는 계산대 쪽을 봅니다. 계산대가 없으면 null입니다 |
| `getRegisterNumber(int x, int y)` | `int` | Query | 타일 (x, y)의 계산대 화면이나 봉투가 속한 계산대 번호입니다. 패널에 표시된 1, 2, 3, 4 중 하나입니다. 설치 시 가장 작은 빈 번호를 받고 옮겨도 유지됩니다. 계산대가 없으면 0입니다 |
| `getStock(int x, int y)` | `int` | Query | 타일 (x, y)의 물체에 있는 상품 수입니다. 상품이 없거나 매장 밖이면 0입니다 |
| `find(Entity entity)` | `Cell` | Query | 이 개체가 놓인 첫 타일입니다. y 순서로 행을 살피고 각 행은 왼쪽부터 찾습니다. 없으면 null입니다 |
| `findAll(Entity entity)` | `List<Cell>` | Query | 이 개체가 놓인 모든 타일을 같은 순서로 반환합니다. 빵 진열대처럼 두 칸짜리는 두 타일 모두 포함합니다. 계산대는 화면이 REGISTER, 봉투가 BAG입니다 |
| `getPickupDirections(int x, int y, Product product)` | `List<Direction>` | Query | 타일 (x, y)에서 이 상품을 집을 수 있는 방향입니다. 보통 하나이며 같은 상품 진열대 사이에서는 두 방향입니다. 집을 수 없으면 빈 리스트입니다 |
| `getHour()` | `int` | Query | 게임 시각의 시: 0~23(게임 하루는 실제 10분) |
| `getMinute()` | `int` | Query | 게임 시각의 분: 0~59 |
| `getDay()` | `int` | Query | 현재 날짜: 1, 2, … |
| `getSeason()` | `Season` | Query | 현재 계절: Season.SUMMER, AUTUMN, WINTER 또는 SPRING이며 각각 10일입니다. 계절 상품은 해당 계절에만 주문할 수 있습니다 |
| `getWaitingCustomer()` | `Customer` | Query | 계산대에서 아직 주문을 접수하지 않은 손님 중 가장 먼저 떠날 손님입니다. 없으면 null입니다. 로봇 위치와 관계없이 customer.getPatience()로 제시간에 도착할지 확인할 수 있습니다 |
| `getMoney()` | `int` | Query | 매장의 보유 금액 |
| `getBuyPrice(Product product)` | `int` | Query | 상품 하나의 주문 비용입니다. 판매가의 절반이며 “매입 할인”으로 줄일 수 있습니다 |
| `getTruckCapacity()` | `int` | Query | 트럭 적재량입니다. 한 번의 배송에는 이 수량까지만 주문할 수 있습니다 |
| `callDelivery(List<Product> goods)` | `boolean` | Action | 배송을 요청합니다. 리스트의 요소 하나당 상품 하나를 싣고 오며 즉시 결제됩니다. 제철이 아닌 계절 상품은 오지 않고 하나당 $10의 벌금이 부과됩니다. 트럭이 사용 중이거나 리스트가 비었거나 돈이 부족하면 false이며 로봇 위에 이유가 표시됩니다. 적재량을 넘기면 오류이므로 주문을 나누세요 |
| `getRemaining(Product product)` | `int` | Query | 요청한 배송에서 이 상품을 아직 트럭에서 내리지 않은 수량입니다. 이동 중과 차고에 있는 동안 유효하며, 모두 내렸거나 배송 요청이 없으면 0입니다. 주문의 getRemaining과 같은 트럭용 기능입니다 |
| `isTruckHere()` | `boolean` | Query | 트럭이 차고에 있는지 나타냅니다. 막 도착하면 아직 문을 여는 중이므로 robot.isAtTruck()으로 상품을 꺼낼 수 있는지 확인하세요 |
| `isTruckAway()` | `boolean` | Query | 트럭이 이동 중도 아니고 차고에도 없어 새로 호출할 수 있는지 나타냅니다 |
| `getGarageX()` | `int` | Query | 차고 문 바로 앞 타일의 x입니다. 이 타일에서 문을 바라보면 트럭 상품을 꺼낼 수 있습니다. 문 앞 두 타일 중 하나가 막혔으면 다른 타일을 사용합니다 |
| `getGarageY()` | `int` | Query | 차고 문 앞 같은 타일의 y |
| `getGarageSide()` | `Direction` | Query | 차고에서 바라볼 방향: 문 쪽 |
| `sendTruckAway()` | `boolean` | Action | 다 내리지 않은 트럭을 보냅니다. 남은 상품 금액은 환불됩니다(빈 트럭은 자동 출발). 차고에 트럭이 없으면 false입니다 |
