# Shelf API (5개)

로봇 앞의 상품 진열대, 탁자, 선반 또는 냉장고를 나타내는 객체입니다. `ShelfCommands`의 등록 정보와 `ShelfApi`의 구현을 확인했으며, 설명의 한국어는 기존 번역표를 사용했습니다.

`robot.getShelf()`로 얻습니다. 앞에 상품을 놓는 물체가 없으면 `null`입니다. 상품을 집거나 놓는 동작은 `Shelf`가 아니라 `robot.pick(product)`, `robot.putOnShelf(product)`로 수행합니다.

| 명령 | 반환형 | 종류 | 설명 |
|---|---|---|---|
| `getProduct()` | `Product` | Query | 이 진열대에 놓이는 상품입니다. 해당 상품만 놓을 수 있습니다 |
| `getCount()` | `int` | Query | 현재 진열대의 상품 수입니다. 로봇이 앞에 있어야 하며 아니면 오류입니다. 직접 다가가 확인하세요(원격은 “검색”과 store.getStock(x, y) 사용) |
| `getCapacity()` | `int` | Query | 진열대가 가득 찼을 때의 상품 수 |
| `isFull()` | `boolean` | Query | 진열대가 가득 차 더 놓을 수 없는지 나타냅니다. 로봇이 앞에 있을 때만 사용하세요 |
| `isEmpty()` | `boolean` | Query | 진열대가 비어 집을 것이 없는지 나타냅니다. 로봇이 앞에 있을 때만 사용하세요 |

## 사용 조건

`getCount()`, `isFull()`, `isEmpty()`는 호출하는 로봇이 해당 진열대의 상품을 집을 위치에서 진열대를 바라보고 있어야 합니다. 객체를 저장해 둔 뒤 다른 곳에서 호출하면 오류입니다. `getProduct()`와 `getCapacity()`에는 이 위치 검사가 적용되지 않습니다.

멀리 있는 진열대의 재고는 “검색” 기능의 `store.getStock(x, y)`로 확인합니다. `getCapacity()`는 해당 진열대가 가득 찼을 때의 수량이며 로봇의 운반 용량과 다릅니다.

## C# 시그니처

```csharp
Product GetProduct();
int GetCount();
int GetCapacity();
bool IsFull();
bool IsEmpty();
```

```csharp
Shelf shelf = robot.GetShelf();
if (shelf != null && robot.IsAtPickup() && !shelf.IsEmpty())
{
    robot.Pick(shelf.GetProduct());
}
```
