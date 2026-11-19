# Code Market 게임 API

설치된 Unity 게임의 관리 DLL에서 스크립트에 공개된 게임 전용 타입 10종을 확인한 문서입니다. 등록 시그니처, 열거형 목록, 상품 카탈로그와 관련 구현을 대조했습니다. IL 명령어와 Unity 내부 메서드는 포함하지 않습니다.

| 타입 | 문서 | 게임 전용 함수 수 | 상수 수 |
|---|---|---|---|
| Robot | [Robot-API.md](Robot-API.md) | 25 | — |
| Store | [Store-API.md](Store-API.md) | 28 | — |
| Order | [Order-API.md](Order-API.md) | 7 | — |
| Cell | [Cell-API.md](Cell-API.md) | 3 | — |
| Shelf | [Shelf-API.md](Shelf-API.md) | 5 | — |
| Customer | [Customer-API.md](Customer-API.md) | 2 | — |
| Entity | [Entity-API.md](Entity-API.md) | 0 | 45 |
| Product | [Product-API.md](Product-API.md) | 11 | 29 |
| Direction | [Direction-API.md](Direction-API.md) | 0 | 4 |
| Season | [Season-API.md](Season-API.md) | 0 | 4 |

게임 전용 함수는 총 81개입니다. 표의 함수 수에는 일반 객체·열거형·컬렉션의 공통 기능을 포함하지 않습니다. Java, C#, C++의 표준 라이브러리는 별도 범위입니다.

## 표기와 타입

기존 `Robot`, `Store`, `Order` 문서와 맞춰 등록 시그니처는 Java 표기인 `getX()`, `boolean`, `String` 등을 유지했습니다. 새 객체 API 문서에는 C# 시그니처를 함께 실었습니다. C# 메서드는 `GetX()`, `IsFull()`처럼 대문자로 시작하며 `bool`, `string`, `object`를 사용합니다. 상수 표는 별도 표시가 없으면 Java 등록명입니다.

`Cell`, `Shelf`, `Customer`, `Order`는 조회 결과가 없으면 `null`일 수 있는 객체입니다. `Entity`, `Direction`, `Season`, `Product`는 열거형이며, `null`을 반환할 수 있는 API는 C#에서 `Entity?`, `Direction?`, `Season?`처럼 표현합니다.

`Entity`는 물체 종류이고 `Shelf`는 특정 진열대 객체입니다. 두 타입을 같은 인터페이스나 상속 관계로 취급하지 마세요. `Cell`은 물체 종류가 아닌 좌표를 나타냅니다.

## 분석 원본

- 앱: `/Users/jkwchunjae/Library/Application Support/Steam/steamapps/common/Code Market/Code Market.app`
- 앱 표시 버전: `1.0`
- 분석 기준일: 2026-11-19
- 주요 원본: `Contents/Resources/Data/Managed/JavaMarket.dll`
- 확인한 등록 경로: `GameScripting`, `CellCommands`, `ShelfCommands`, `CustomerCommands`, `CsGame`
- 열거형·상품 데이터: `StoreObjectType`, `World.Product`, `World.Season`, `ProductCatalog`
- 한국어 설명: 저장소의 `KoreanPatch/ko.json`; 상수 설명과 사용 조건은 등록 데이터 및 구현을 대조해 정리
- 분석한 DLL의 SHA-256: `5443c6638051230fea62bb51023c92d34c5a291aa6ba48cccbef9c984861e326`

가격·분류·상수 목록은 이 DLL 기준입니다. 게임이 업데이트되면 달라질 수 있습니다.
