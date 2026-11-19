# Product API (11개, 상품 상수 29개)

상품 종류를 나타내는 열거형입니다. `GameScripting.ProductMethods`와 `ProductMethod`, `ProductCatalog`의 등록 데이터에서 추출했습니다. 함수 설명의 한국어는 기존 번역표를 사용했습니다.

예를 들어 Java에서는 `Product.TOMATO.isInSeason()`, C#에서는 `Product.Tomato.IsInSeason()`으로 조회합니다. 상품 한 종류를 나타내며, 수량이나 개별 상품 인스턴스를 담는 객체가 아닙니다.

## 조회 함수

| 명령 | 반환형 | 종류 | 설명 |
|---|---|---|---|
| `isAlcohol()` | `boolean` | Query | 주류인지 나타냅니다. 주류는 22:00~6:00에 판매할 수 없습니다 |
| `isAdultsOnly()` | `boolean` | Query | 술이나 담배 같은 성인 전용(18+) 상품인지 나타냅니다. 어린이에게 판매할 수 없습니다(customer.isAdult()) |
| `isPremium()` | `boolean` | Query | 스테이크나 트러플 같은 프리미엄 상품인지 나타냅니다. 비싸지만 일부 손님만 구매합니다(“업그레이드”의 “프리미엄 수요”) |
| `isVending()` | `boolean` | Query | 초콜릿, 팝콘, 커피처럼 자판기에서 판매하는 상품인지 나타냅니다. 계산대에서 주문하지 않고 손님이 직접 자판기로 갑니다. 로봇은 상품만 채우면 됩니다 |
| `isInfinite()` | `boolean` | Query | 커피처럼 무한 공급되는 상품인지 나타냅니다. 해당 자판기는 비지 않아 보충할 필요가 없습니다 |
| `isElite()` | `boolean` | Query | 캐비아나 랍스터 같은 최고급 상품인지 나타냅니다. 프리미엄보다 더 비싸고 구매자도 적습니다(“업그레이드”의 “최고급 수요”) |
| `getPrice()` | `int` | Query | 상품 가격입니다. 손님은 가격 × 수익 배율(“업그레이드”의 “수익” 및 장식 효과)을 지불합니다. 매입가는 가격의 절반입니다 |
| `getName()` | `String` | Query | 게임 언어로 표시되는 이름입니다. 예: “맥주” |
| `getRack()` | `Entity` | Query | 상품이 놓이는 진열대 |
| `getSeason()` | `Season` | Query | 상품의 판매 계절입니다. 해당 계절에만 주문할 수 있으며 사계절 상품은 null입니다 |
| `isInSeason()` | `boolean` | Query | 현재 주문할 수 있는지 나타냅니다. 사계절 상품이거나 제철이어야 합니다. 제철이 아니면 배송되지 않고 상품 하나당 $10의 벌금이 부과됩니다 |

`getPrice()`는 상품 카탈로그의 기본 가격입니다. 매장의 수익 배율을 적용한 실제 결제 금액이나 매입 할인을 적용한 배송 비용과 구분하세요. 현재 매입 비용은 `store.getBuyPrice(product)`로 확인합니다.

`getSeason()`은 사계절 상품이면 `null`입니다. C#에서는 `Season?`입니다. 제철 여부는 `isInSeason()`으로 확인할 수 있지만, 무한 공급 상품의 배송 가능 여부까지 보장하는 함수는 아닙니다. 커피는 자동 보충되므로 로봇이 배송받아 채울 필요가 없습니다.

## 상품 상수

아래 상수 표기는 기존 API 문서와 같은 Java 등록명입니다. 특히 `EASTERCAKE`, `ICECREAM`에는 밑줄이 없습니다. 가격과 분류는 분석한 DLL의 카탈로그 값입니다.

| 상수 | 상품 | 기본 가격 | 진열대 Entity | 주문 가능 계절 | 분류 |
|---|---|---|---|---|---|
| `Product.TOMATO` | 토마토 | $6 | `Entity.TOMATO_TABLE` | 사계절 | 일반 |
| `Product.PUMPKIN` | 호박 | $9 | `Entity.PUMPKIN_TABLE` | 사계절 | 일반 |
| `Product.BREAD` | 빵 | $13 | `Entity.BREAD_SHELF` | 사계절 | 일반 |
| `Product.SODA` | 청량음료 | $15 | `Entity.FRIDGE` | 사계절 | 일반 |
| `Product.BEER` | 맥주 | $25 | `Entity.BEER_FRIDGE` | 사계절 | 주류, 성인 전용 |
| `Product.CIGARETTES` | 담배 | $30 | `Entity.CIGARETTE_STAND` | 사계절 | 성인 전용 |
| `Product.WINE` | 와인 | $85 | `Entity.WINE_STAND` | 사계절 | 주류, 성인 전용 |
| `Product.MILK` | 우유 | $17 | `Entity.MILK_STAND` | 사계절 | 일반 |
| `Product.TULIPS` | 튤립 | $80 | `Entity.TULIP_COUNTER` | 봄 | 일반 |
| `Product.HONEY` | 꿀 | $45 | `Entity.HONEY_STALL` | 가을 | 일반 |
| `Product.WATERMELON` | 수박 | $32 | `Entity.WATERMELON_TABLE` | 여름 | 일반 |
| `Product.TANGERINE` | 귤 | $30 | `Entity.TANGERINE_TABLE` | 겨울 | 일반 |
| `Product.STRAWBERRY` | 딸기 | $75 | `Entity.STRAWBERRY_STAND` | 여름 | 일반 |
| `Product.EASTERCAKE` | 부활절 케이크 | $50 | `Entity.EASTER_TABLE` | 봄 | 일반 |
| `Product.FIREWORKS` | 불꽃놀이 | $55 | `Entity.FIREWORKS_STAND` | 겨울 | 일반 |
| `Product.TREE` | 크리스마스트리 | $80 | `Entity.TREE_STAND` | 겨울 | 일반 |
| `Product.STEAK` | 마블링 스테이크 | $100 | `Entity.MEAT_FRIDGE` | 사계절 | 프리미엄 |
| `Product.TRUFFLE` | 트러플 | $125 | `Entity.TRUFFLE_FRIDGE` | 사계절 | 프리미엄 |
| `Product.CAVIAR` | 블랙 캐비아 | $180 | `Entity.CAVIAR_FRIDGE` | 사계절 | 최고급 |
| `Product.LOBSTER` | 바닷가재 | $260 | `Entity.LOBSTER_FRIDGE` | 사계절 | 최고급 |
| `Product.CHOCOLATE` | 초콜릿 | $10 | `Entity.VENDING_MACHINE` | 사계절 | 자판기 |
| `Product.POPCORN` | 팝콘 | $20 | `Entity.POPCORN_MACHINE` | 사계절 | 자판기 |
| `Product.COFFEE` | 커피 | $30 | `Entity.COFFEE_MACHINE` | 사계절 | 자판기, 무한 공급 |
| `Product.ICECREAM` | 아이스크림 | $65 | `Entity.ICE_CREAM_COUNTER` | 여름 | 일반 |
| `Product.JAM` | 잼 | $30 | `Entity.JAM_TABLE` | 가을 | 일반 |
| `Product.FERTILIZER` | 비료 | $35 | `Entity.FERTILIZER_TABLE` | 봄 | 일반 |
| `Product.MASK` | 가면 | $40 | `Entity.MASK_STAND` | 가을 | 일반 |
| `Product.WHISKY` | 위스키 | $150 | `Entity.WHISKY_CABINET` | 사계절 | 주류, 성인 전용, 프리미엄 |
| `Product.COGNAC` | 코냑 | $220 | `Entity.COGNAC_CABINET` | 사계절 | 주류, 성인 전용, 최고급 |

상품인 `Product.TREE`의 진열대는 `Entity.TREE_STAND`입니다. 장식물 `Entity.CHRISTMAS_TREE`와 구분하세요.

## C# 시그니처

```csharp
bool IsAlcohol();
bool IsAdultsOnly();
bool IsPremium();
bool IsVending();
bool IsInfinite();
bool IsElite();
int GetPrice();
string GetName();
Entity GetRack();
Season? GetSeason();
bool IsInSeason();
```

```csharp
Product product = Product.Tomato;
if (product.IsInSeason())
{
    int buyPrice = store.GetBuyPrice(product);
}
```
