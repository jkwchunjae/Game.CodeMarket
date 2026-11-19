# Season API (전용 함수 0개, 상수 4개)

게임의 계절을 나타내는 열거형입니다. `GameScripting.Seasons`, `World.Season`, `ProductCatalog`의 계절 데이터를 확인했습니다.

| Java 상수 | C# 상수 | 계절 | 해당 계절에만 주문할 수 있는 상품 |
|---|---|---|---|
| `Season.SUMMER` | `Season.Summer` | 여름 | `Product.WATERMELON`, `Product.STRAWBERRY`, `Product.ICECREAM` |
| `Season.AUTUMN` | `Season.Autumn` | 가을 | `Product.HONEY`, `Product.JAM`, `Product.MASK` |
| `Season.WINTER` | `Season.Winter` | 겨울 | `Product.TANGERINE`, `Product.FIREWORKS`, `Product.TREE` |
| `Season.SPRING` | `Season.Spring` | 봄 | `Product.TULIPS`, `Product.EASTERCAKE`, `Product.FERTILIZER` |

## 조회와 주문

현재 계절은 `store.getSeason()`으로 확인합니다. 여름 → 가을 → 겨울 → 봄 순서이며 각각 10일입니다. 상품의 계절은 `product.getSeason()`으로 조회하고, 사계절 상품이면 `null`입니다. 현재 주문할 수 있는 계절인지는 `product.isInSeason()`으로 확인합니다.

제철이 아닌 계절 상품을 배송 요청하면 상품은 오지 않고 상품 하나당 $10의 벌금이 부과됩니다. 계절은 배송 주문 가능 여부를 판단하는 기준이며, 사계절 상품에는 이 제한이 없습니다.

```csharp
Season current = store.GetSeason();
Season? required = Product.Tomato.GetSeason(); // 사계절 상품이므로 null
bool canOrderNow = Product.Tomato.IsInSeason();
```
