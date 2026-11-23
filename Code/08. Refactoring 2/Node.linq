<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load "../basecode"
#load "./StoreMap"
#load "./StaticData"

void Main()
{
    var graph = Graph.Create(new StoreMap(new StaticData()));
}

// 맵 구조
// cell
// g: garage
// d: delivery
// r: register
// s: shelf
// _: empty (dont care)
//
// direction: ←→↔↕↑↓
/*
case1)


  0 1 2 3 4 5 6 7 8
8 _ _ _ _ g←g _ _ _
          ↓ ↑      
7 _ d←d←d←d←d←d _ _
    ↓ ↕ ↕ ↕ ↕ ↑    
6 _ d→d→d→d→d→d _ _
    ↓ ↑     ↓ ↑    
5 _ s↔s _ _ s↔s _ _
    ↓ ↑     ↓ ↑    
4 _ s↔s _ _ s↔s _ _
    ↓ ↑     ↓ ↑    
3 _ s↔s _ _ s↔s _ _
    ↓ ↑     ↓ ↑    
2 _ r←r←r←r←r←r←r←r
    ↓ ↕ ↕ ↕ ↕ ↕ ↕ ↑
1 _ r→r→r→r→r→r→r→r
                   
0 _ _ _ _ _ _ _ _ _


*/

interface IGraph
{
    INode GetNode(int x, int y);
}

class Graph : IGraph
{
    INode[,] nodes;
    
    public Graph(int width, int height)
    {
        nodes = new INode[width, height];

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                nodes[x, y] = new Node(x, y);
            }
        }
    }

    public INode GetNode(int x, int y)
    {
        return nodes[x, y];
    }

    record Rect(int X, int Y, int Width, int Height);
    /// <summary>
    /// Graph를 객체를 만드는 함수.
    /// 코드마켓의 컨텐츠 로직은 이 함수 안에만 존재해야한다.
    /// IGraph, INode에는 컨텐츠 로직이 들어있지 않은 순수 graph객체여야한다.
    /// </summary>
    public static IGraph Create(IStoreMap map)
    {
        var width = store.GetWidth();
        var height = store.GetHeight();

        var graph = new Graph(width, height);

        // 계산대 위쪽만 고려했을 때 진열대가 있는 범위의 높이.
        // 계산대 아래쪽에 자판기가 있을 수 있다.
        // 그것은 당장 고려하지 않는다.
        Rect[] shelfBoxes = CalcShelfBoxes(map);

        // 계산대를 기준으로 생각해야함.
        // 계산대가 여러개 있을 수 있음. 가로로 죽 늘어선 형태 계산대 간격이 벌어져 있을 수 있음.
        Rect registerBox = CalcRegisterBox(map, shelfBoxes);

        Rect[] deliveryBox = CalcDeliveryBox(map, shelfBoxes);
        
        MakeRegisterBoxConnection(registerBox, shelfBoxes);
        MakeShelfBoxesConnection(shelfBoxes, registerBox, deliveryBox);
        MakeDeliveryBoxConnection(deliveryBox, shelfBoxes);
        
        return default;
    }
}


interface INode
{
    int X { get; }
    int Y { get; }

    /// 위로 올라갈 수 있는가? 객체가 있으면 갈 수 있다는 뜻.
    INode? Up { get; set; }
    INode? Down { get; set; }
    INode? Left { get; set; }
    INode? Right { get; set; }
    INode[] Next { get; }
}

class Node : INode
{
    public int X { get; init; }
    public int Y { get; init; }

    public INode? Up { get; set; }
    public INode? Down { get; set; }
    public INode? Left { get; set; }
    public INode? Right { get; set; }
    
    private readonly Lazy<INode[]> _next;
    public INode[] Next => _next.Value;

    public Node(int x, int y)
    {
        (X, Y) = (x, y);
        _next = new Lazy<INode[]>(() => new [] { Up, Down, Left, Right }
            .OfType<INode>()
            .ToArray());
    }
}
