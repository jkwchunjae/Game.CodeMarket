# Cell API (3개)

매장의 타일 하나를 나타내는 좌표 객체입니다. `CellCommands`의 등록 정보와 `CellApi`의 구현을 확인했으며, 설명의 한국어는 기존 번역표를 사용했습니다.

`store.getCashierCell(x, y)`, `store.getPackerCell(x, y)`, `store.find(entity)`, `store.findAll(entity)`로 얻습니다. 앞의 세 함수는 조건에 맞는 타일이 없으면 `null`을 반환하므로 확인 후 사용하세요.

| 명령 | 반환형 | 종류 | 설명 |
|---|---|---|---|
| `getX()` | `int` | Query | 타일의 x 좌표 |
| `getY()` | `int` | Query | 타일의 y 좌표 |
| `equals(Object other)` | `boolean` | Query | 같은 타일인지 나타냅니다. cell 객체는 ==가 아닌 equals로 비교합니다 |

## 좌표와 비교

(0, 0)은 입구 쪽 맨 왼쪽 타일입니다. x는 오른쪽으로, y는 입구에서 안쪽 벽으로 갈수록 커집니다. 같은 좌표의 `Cell` 객체인지 비교할 때 Java에서는 `equals`, C#에서는 `Equals`를 사용합니다. 객체의 `==` 비교로 좌표 일치를 판단하지 마세요.

`equals`는 상대가 `Cell`이고 x, y가 모두 같을 때 `true`입니다. `null`이나 다른 타입이면 `false`입니다. 문자열 표현은 `(x, y)` 형식입니다.

## C# 시그니처

```csharp
int GetX();
int GetY();
bool Equals(object other);
```

```csharp
Cell cell = store.Find(Entity.Register);
if (cell != null)
{
    int x = cell.GetX();
    int y = cell.GetY();
}
```
