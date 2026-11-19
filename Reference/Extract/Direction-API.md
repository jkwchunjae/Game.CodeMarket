# Direction API (전용 함수 0개, 상수 4개)

매장 격자의 방향을 나타내는 열거형입니다. `GameScripting.Directions`와 `ToVector`의 변환을 확인했습니다. 카메라를 회전해도 방향은 매장 기준으로 유지됩니다.

| Java 상수 | C# 상수 | 좌표 변화 | 의미 |
|---|---|---|---|
| `Direction.UP` | `Direction.Up` | x 그대로, y + 1 | 입구에서 안쪽 벽 방향 |
| `Direction.DOWN` | `Direction.Down` | x 그대로, y - 1 | 입구 방향 |
| `Direction.LEFT` | `Direction.Left` | x - 1, y 그대로 | 매장 기준 왼쪽 |
| `Direction.RIGHT` | `Direction.Right` | x + 1, y 그대로 | 매장 기준 오른쪽 |

## 사용 위치

`robot.move(direction)`, `robot.turn(direction)`, `robot.canMove(direction)`, `robot.look(direction)`에 전달합니다. 현재 방향은 `robot.getFacing()`, 차고 문을 볼 방향은 `store.getGarageSide()`, 상품을 집을 수 있는 방향 목록은 `store.getPickupDirections(x, y, product)`로 조회합니다.

`store.getRegisterSide(x, y)`는 계산대가 없으면 `null`을 반환합니다. C#에서는 `Direction?`로 받아 확인 후 사용합니다.

```csharp
Direction? side = store.GetRegisterSide(x, y);
if (side != null)
{
    robot.Turn(side.Value);
}
```
