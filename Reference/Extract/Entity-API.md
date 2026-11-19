# Entity API (전용 함수 0개, 상수 45개)

매장에 설치된 물체의 **종류**를 나타내는 열거형입니다. 물체 인스턴스나 공통 부모 인터페이스가 아닙니다. `GameScripting.Enums`의 등록 경로, `StoreObjectType`, `EntityName`, `EntityNameAt`을 확인했습니다.

`store.getEntity(x, y)`와 `robot.look(direction)`가 해당 타일의 물체 종류를 반환합니다. 타일이 비어 있으면 `null`입니다. `store.find(entity)`, `store.findAll(entity)`, `robot.isFacing(entity)`에는 찾거나 비교할 종류를 전달합니다. 재고와 같은 개별 진열대 정보는 [Shelf API](Shelf-API.md) 또는 `store.getStock(x, y)`로 확인합니다.

## 상수

기존 API 문서와 같은 Java 등록명입니다. `StoreObjectType` 44개를 이름 변환해 등록하며, 계산대 봉투를 위한 `BAG`를 추가합니다.

| 상수 | 의미 | 관련 상품 |
|---|---|---|
| `Entity.BREAD_SHELF` | 빵 진열대 | `Product.BREAD` |
| `Entity.REGISTER` | 계산대 화면 | — |
| `Entity.FRIDGE` | 음료 냉장고 | `Product.SODA` |
| `Entity.PUMPKIN_TABLE` | 호박 상자 | `Product.PUMPKIN` |
| `Entity.PLANT` | 화분 | — |
| `Entity.BOXES` | 상자 더미 | — |
| `Entity.FLOWER` | 꽃 | — |
| `Entity.TOMATO_TABLE` | 토마토 판매대 | `Product.TOMATO` |
| `Entity.BEER_FRIDGE` | 맥주 냉장고 | `Product.BEER` |
| `Entity.CIGARETTE_STAND` | 담배 진열대 | `Product.CIGARETTES` |
| `Entity.WINE_STAND` | 와인 진열대 | `Product.WINE` |
| `Entity.MILK_STAND` | 우유 진열대 | `Product.MILK` |
| `Entity.TULIP_COUNTER` | 튤립 판매대 | `Product.TULIPS` |
| `Entity.HONEY_STALL` | 꿀 가판대 | `Product.HONEY` |
| `Entity.WATERMELON_TABLE` | 수박 판매대 | `Product.WATERMELON` |
| `Entity.TANGERINE_TABLE` | 귤 판매대 | `Product.TANGERINE` |
| `Entity.STRAWBERRY_STAND` | 딸기 진열대 | `Product.STRAWBERRY` |
| `Entity.EASTER_TABLE` | 부활절 케이크 판매대 | `Product.EASTERCAKE` |
| `Entity.FIREWORKS_STAND` | 불꽃놀이 진열대 | `Product.FIREWORKS` |
| `Entity.TREE_STAND` | 크리스마스트리 진열대 | `Product.TREE` |
| `Entity.SIGN_BOARD` | 입간판 | — |
| `Entity.PALM_TREE` | 야자수 | — |
| `Entity.FAN_PALM` | 부채야자 | — |
| `Entity.MONSTERA` | 고사리 (내부 타입명: Monstera) | — |
| `Entity.DRACAENA` | 드라세나 | — |
| `Entity.MEAT_FRIDGE` | 스테이크 냉장 진열대 | `Product.STEAK` |
| `Entity.TRUFFLE_FRIDGE` | 트러플 냉장 진열대 | `Product.TRUFFLE` |
| `Entity.CAVIAR_FRIDGE` | 캐비아 냉장 진열대 | `Product.CAVIAR` |
| `Entity.LOBSTER_FRIDGE` | 바닷가재 냉장 진열대 | `Product.LOBSTER` |
| `Entity.VENDING_MACHINE` | 초콜릿 자동판매기 | `Product.CHOCOLATE` |
| `Entity.POPCORN_MACHINE` | 팝콘 기계 | `Product.POPCORN` |
| `Entity.COFFEE_MACHINE` | 커피 머신 | `Product.COFFEE` |
| `Entity.CHRISTMAS_TREE` | 장식용 크리스마스트리 (장식물) | — |
| `Entity.ICE_CREAM_COUNTER` | 아이스크림 판매대 | `Product.ICECREAM` |
| `Entity.JAM_TABLE` | 잼 판매대 | `Product.JAM` |
| `Entity.FERTILIZER_TABLE` | 비료 판매대 | `Product.FERTILIZER` |
| `Entity.MASK_STAND` | 가면 진열대 | `Product.MASK` |
| `Entity.WHISKY_CABINET` | 프리미엄 위스키 | `Product.WHISKY` |
| `Entity.COGNAC_CABINET` | 최고급 코냑 | `Product.COGNAC` |
| `Entity.AQUARIUM` | 수족관 | — |
| `Entity.WATER_COOLER` | 냉온수기 | — |
| `Entity.BALLOONS` | 풍선 | — |
| `Entity.FORTUNE_WHEEL` | 행운의 룰렛 | — |
| `Entity.DAISIES` | 데이지 | — |
| `Entity.BAG` | 계산대의 두 번째 타일인 봉투 | — |

## 계산대와 빈 타일

계산대는 화면 타일이 `Entity.REGISTER`, 봉투 타일이 `Entity.BAG`입니다. `store.findAll(Entity.REGISTER)`는 화면 타일을, `store.findAll(Entity.BAG)`는 봉투 타일을 찾습니다. 한 물체가 여러 타일을 차지하면 같은 종류가 여러 좌표에서 반환될 수 있습니다.

`null`은 빈 타일이나 조회 실패를 나타내며 `Entity` 상수가 아닙니다. `GetRegisterNumber(x, y)`의 0도 `Entity` 값과 관계없습니다. 같은 종류의 진열대가 여러 개 있으면 모두 같은 `Entity` 값을 가지므로, 실제 위치는 `Cell`로 구분합니다.

## C# 예제

C#에서는 `Entity.Register`, `Entity.Bag`, `Entity.TomatoTable`처럼 사용합니다. `store.GetEntity(...)`의 반환형은 빈 타일을 표현할 수 있는 `Entity?`입니다.

```csharp
Entity? entity = store.GetEntity(robot.GetX(), robot.GetY());
List<Cell> shelves = store.FindAll(Entity.TomatoTable);
bool facingBag = robot.IsFacing(Entity.Bag);
```
