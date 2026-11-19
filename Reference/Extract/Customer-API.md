# Customer API (2개)

계산대의 손님을 나타내는 객체입니다. `CustomerCommands`의 등록 정보와 `CustomerApi`의 구현을 확인했으며, 설명의 한국어는 기존 번역표를 사용했습니다.

`robot.getCustomer()`는 로봇이 주문을 받는 계산대 화면 앞에 있을 때 그 계산대의 손님을 반환합니다. 위치가 맞지 않거나 손님이 없으면 `null`입니다. `store.getWaitingCustomer()`는 아직 주문을 접수하지 않은 손님 중 가장 먼저 떠날 손님을 로봇 위치와 관계없이 반환하며, 해당 손님이 없으면 `null`입니다.

| 명령 | 반환형 | 종류 | 설명 |
|---|---|---|---|
| `isAdult()` | `boolean` | Query | 손님이 성인인지 나타냅니다. 어린이에게는 술이나 담배(18세 이상 상품, product.isAdultsOnly())를 판매할 수 없습니다. order.remove로 주문에서 제외하세요. 봉투에 담으면 상품 하나당 $150의 벌금이 부과됩니다 |
| `getPatience()` | `int` | Query | 손님이 주문 접수(robot.takeOrder())를 기다리며 계산대에 머무를 남은 시간(초)입니다. 주문을 접수하면 시간이 줄지 않고 계속 기다립니다. 남은 시간이 너무 적으면 계산대에 제때 도착할 수 없습니다 |

## 사용 조건

손님을 조회하는 것만으로 주문이 접수되지는 않습니다. 로봇이 계산대 화면 앞에서 화면을 바라보는 상태로 `robot.takeOrder()`를 호출해야 주문을 받고 대기 시간 감소를 멈출 수 있습니다.

`isAdult()`가 `false`인 손님에게는 `product.isAdultsOnly()`인 상품을 판매하지 마세요. `order.remove(product)`로 아직 포장하지 않은 상품을 제외할 수 있습니다.

## C# 시그니처

```csharp
bool IsAdult();
int GetPatience();
```

```csharp
Customer customer = robot.GetCustomer();
if (customer != null)
{
    bool adult = customer.IsAdult();
    int seconds = customer.GetPatience();
    Order order = robot.TakeOrder();
}
```
