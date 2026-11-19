# Order API (7개)

게임 DLL의 등록 정보에서 추출한 시그니처입니다. 설명의 한국어는 기존 번역표를 사용했습니다.

| 명령 | 반환형 | 종류 | 설명 |
|---|---|---|---|
| `getProducts()` | `List<Product>` | Query | 주문의 상품 종류를 손님이 말한 순서대로 중복 없이 반환합니다 |
| `getCount(Product product)` | `int` | Query | 손님이 이 상품을 원하는 수량입니다. 주문에 없으면 0입니다 |
| `getRemaining(Product product)` | `int` | Query | 이 상품 중 아직 봉투에 담지 않은 수량 |
| `getTotal()` | `int` | Query | 주문 전체의 상품 수량 |
| `isDone()` | `boolean` | Query | 모든 상품이 봉투에 담겼는지 나타냅니다. 전부 담으면 손님이 결제하고 떠납니다 |
| `remove(Product product)` | `boolean` | Action | 이 상품 중 아직 봉투에 담지 않은 수량을 전부 주문에서 제외합니다. 이미 담은 상품은 유지되어 결제됩니다. 다른 상품은 그대로이며 removeRemaining()은 전부 제외합니다. 제외할 것이 없으면 false입니다. 더 담을 것이 없으면 손님이 결제하고 떠나며 빈 봉투라면 빈손으로 떠납니다 |
| `removeRemaining()` | `boolean` | Action | 아직 봉투에 담지 않은 모든 상품을 주문에서 제외합니다. 이미 담은 것은 유지되어 결제됩니다. 제외할 것이 없으면 false입니다. 손님은 봉투의 상품을 결제하고 떠나며 봉투가 비었으면 빈손으로 떠납니다 |
