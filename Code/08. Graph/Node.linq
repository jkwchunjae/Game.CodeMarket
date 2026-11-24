<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load "../basecode"
#load "./StoreMap"

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
8 _ _ _ g←g←g←g _ _
        ↓ ↕ ↕ ↑    
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
    string ToGridString();
}

class Graph : IGraph
{
    Node[,] nodes;
    
    public Graph(int width, int height)
    {
        nodes = new Node[width, height];

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                nodes[x, y] = new Node(this, x, y);
            }
        }
    }

    public INode GetNode(int x, int y)
    {
        return nodes[x, y];
    }
    
    #region ToGridString
    public string ToGridString()
    {
        var width = nodes.GetLength(0);
        var height = nodes.GetLength(1);
        var lines = new List<string>();

        // 좌표 헤더
        lines.Add("  " + string.Join(" ", Enumerable.Range(0, width).Select(x => x % 10)));

        // 위쪽 행부터 출력한다. 수평 화살표는 출발 노드와 도착 노드의
        // 연결 정보를 함께 확인해 양방향 연결도 표현한다.
        for (var y = height - 1; y >= 0; y--)
        {
            var row = new System.Text.StringBuilder();
            row.Append(y % 10).Append(' ');
            for (var x = 0; x < width; x++)
            {
                var node = nodes[x, y];
                var connected = node.Up is not null || node.Down is not null ||
                    node.Left is not null || node.Right is not null;
                row.Append(connected ? 'o' : '_');

                if (x < width - 1)
                {
                    var right = node.Right is not null;
                    var left = nodes[x + 1, y].Left is not null;
                    row.Append(right && left ? '↔' : right ? '→' : left ? '←' : ' ');
                }
            }
            lines.Add(row.ToString());

            if (y == 0)
                continue;

            var vertical = new System.Text.StringBuilder();
            vertical.Append("  ");
            for (var x = 0; x < width; x++)
            {
                var down = nodes[x, y].Down is not null;
                var up = nodes[x, y - 1].Up is not null;
                vertical.Append(down && up ? '↕' : down ? '↓' : up ? '↑' : ' ');
                if (x < width - 1)
                    vertical.Append(' ');
            }
            lines.Add(vertical.ToString());
        }

        return string.Join(Environment.NewLine, lines);
    }
    #endregion

    record Rect(int X, int Y, int Width, int Height)
    {
        /// <summary>사각형 안쪽 끝 x좌표</summary>
        public int XEnd => X + Width - 1;
        /// <summary>사각형 안쪽 끝 y좌표</summary>
        public int YEnd => Y + Height - 1;
    }
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
        Rect registerBox = CalcRegisterBox(shelfBoxes);

        // 차고앞, 배달로봇 동선 박스. 각각 구한다.
        Rect garageBox = CalcGarageBox();
        Rect deliveryBox = CalcDeliveryBox(garageBox, shelfBoxes);
        
        MakeRegisterBoxConnection(graph, registerBox);
        MakeShelfBoxesConnection(graph, shelfBoxes);
        MakeDeliveryBoxConnection(graph, garageBox, deliveryBox);
        
        return graph;
    }
    
    #region Calc Box
    #region Shelf Box
    /// shelf box는 가로2, 세로N 짜리 박스의 묶음이다.
    static Rect[] CalcShelfBoxes(IStoreMap map)
    {
        List<Rect> boxes = new List<Rect>();
        
        foreach (var shelfCell in map.GetFrontOfShelfCells())
        {
            var box = boxes.FirstOrDefault(box => ContainsInShelfBox(box, shelfCell.X, shelfCell.Y));
            if (box is null)
            {
                boxes.Add(new Rect(shelfCell.X, shelfCell.Y, 1, 1));
            }
            else
            {
                boxes[boxes.IndexOf(box)] = UpdateShelfBox(box, shelfCell.X, shelfCell.Y);
            }
        }
        
        return boxes.ToArray();
    }
    static Rect UpdateShelfBox(Rect box, int x, int y)
    {
        if (box.X != x)
        {
            var newX = Math.Min(box.X, x);
            var newWidth = Math.Max(box.XEnd, x) - newX + 1;
            box = new Rect(newX, box.Y, newWidth, box.Height);
        }
        if (box.Y != y)
        {
            var newY = Math.Min(box.Y, y);
            var newHeight = Math.Max(box.YEnd, y) - newY + 1;
            box = new Rect(box.X, newY, box.Width, newHeight);
        }
        return box;
    }
    /// <summary> (x, y)가 box에 포함되는지 체크. 정확히 포함되지 않더라도 같은 box로 판단되면 true</summary>
    static bool ContainsInShelfBox(Rect box, int x, int y)
    {
        // x좌표만 보면 될지도?
        if (Math.Abs(box.X - x) <= 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion
    
    #region Register Box
    static Rect CalcRegisterBox(Rect[] shelfBoxes)
    {
        // 계산대는 모두 같은 행에 있다. (y값이 같다.)
        List<Cell> registerCells = store.FindAll(Entity.Register)
            .Concat(store.FindAll(Entity.Bag))
            .ToList();

        if (registerCells.Count() == 0)
        {
            throw new InvalidOperationException("계산대를 찾을 수 없습니다. store.FindAll(Entity.Register).Count() == 0.");
        }
        var minX = Math.Min(shelfBoxes.Min(s => s.X), registerCells.Min(c => c.GetX()));
        var maxX = Math.Max(shelfBoxes.Max(s => s.XEnd), registerCells.Max(c => c.GetX()));
        var y = registerCells[0].GetY() + 1;
        var height = 2;
        return new Rect(minX, y, maxX - minX + 1, height);
    }
    #endregion
    
    #region Garage Box
    /// 차고 앞 세로1, 가로4짜리 box.
    static Rect CalcGarageBox()
    {
        
        var garageX = store.GetGarageX();
        var garageY = store.GetGarageY();
        
        var x = garageX;
        var width = 1;
        if (store.IsFree(garageX - 1, garageY))
        {
            x = garageX - 1;
            width += 1;
        }
        if (store.IsFree(garageX + 1, garageY))
        {
            width += 1;
        }
        return new Rect(x, garageY, width, 1);
    }
    #endregion
    
    #region Delivery Box
    /// <summary>
    /// 배달 로봇이 움직일 공간
    /// 아래로는 선반 박스와 닿아야 하고, 위로는 차고 박스랑 닿아야한다.
    /// </summary>
    static Rect CalcDeliveryBox(Rect garageBox, Rect[] shelfBoxes)
    {
        var minX = Math.Min(garageBox.X, shelfBoxes.Min(s => s.X));
        var maxX = Math.Max(garageBox.X, shelfBoxes.Max(s => s.XEnd));
        var minY = shelfBoxes.Max(s => s.YEnd) + 1;
        var maxY = garageBox.Y - 1;

        return new Rect(minX, minY, maxX - minX + 1, maxY - minY + 1);
    }
    #endregion
    #endregion
    
    #region Make Connection
    #region Register Box
    static void MakeRegisterBoxConnection(Graph graph, Rect registerBox)
    {
        // 2 _ r←r←r←r←r←r←r←r
        //     ↓ ↕ ↕ ↕ ↕ ↕ ↕ ↑
        // 1 _ r→r→r→r→r→r→r→r
        
        // 1번 행 우측 일방통행
        for (var x = registerBox.X; x <= registerBox.XEnd - 1; x++)
        {
            var y = registerBox.Y;
            graph.nodes[x, y].CanGotoRight();
        }
        // 2번 행 좌측 일방통행
        for (var x = registerBox.X + 1; x <= registerBox.XEnd; x++)
        {
            var y = registerBox.YEnd;
            graph.nodes[x, y].CanGotoLeft();
        }
        // 가장 좌측 열 아래로 일방통행
        for (var y = registerBox.Y + 1; y <= registerBox.YEnd; y++)
        {
            var x = registerBox.X;
            graph.nodes[x, y].CanGotoDown();
        }
        // 가장 우측 열 위로 일방통행
        for (var y = registerBox.Y; y <= registerBox.YEnd - 1; y++)
        {
            var x = registerBox.XEnd;
            graph.nodes[x, y].CanGotoUp();
        }
        // 중앙 영역 양방향 통행
        for (var x = registerBox.X; x <= registerBox.XEnd; x++)
        {
            for (var y = registerBox.Y; y <= registerBox.YEnd; y++)
            {
                // 위로 올라갈 수 있는 셀 처리
                if (x != registerBox.X && x != registerBox.XEnd && y != registerBox.YEnd)
                {
                    graph.nodes[x, y].CanGotoUp();
                }
                // 아래로 내려갈 수 있는 셀 처리
                if (x != registerBox.X && x != registerBox.XEnd && y != registerBox.Y)
                {
                    graph.nodes[x, y].CanGotoDown();
                }
                // 왼쪽으로 갈 수 있는 셀 처리
                if (y != registerBox.Y && y != registerBox.YEnd && x != registerBox.X)
                {
                    graph.nodes[x, y].CanGotoLeft();
                }
                // 오른쪽으로 갈 수 있는 셀 처리
                if (y != registerBox.Y && y != registerBox.YEnd && x != registerBox.XEnd)
                {
                    graph.nodes[x, y].CanGotoRight();
                }
            }
        }
    }
    #endregion

    #region Shelf Box
    static void MakeShelfBoxesConnection(Graph graph, Rect[] shelfBoxes)
    {
/*
  0 1 2 3 4 5 6 7 8
6 _ d→d→d→d→d→d _ _
    ↓ ↑     ↓ ↑    
5 _ s↔s _ _ s↔s _ _
    ↓ ↑     ↓ ↑    
4 _ s↔s _ _ s↔s _ _
    ↓ ↑     ↓ ↑    
3 _ s↔s _ _ s↔s _ _
    ↓ ↑     ↓ ↑    
2 _ r←r←r←r←r←r←r←r
*/
        foreach (var shelfBox in shelfBoxes)
        {
            // 왼쪽은 아래로, 오른쪽은 위로.
            for (var y = shelfBox.Y + 1; y <= shelfBox.YEnd; y++)
            {
                var x = shelfBox.X;
                graph.nodes[x, y].CanGotoDown();
            }
            for (var y = shelfBox.Y; y <= shelfBox.YEnd - 1; y++)
            {
                var x = shelfBox.X + 1;
                graph.nodes[x, y].CanGotoUp();
            }
            // 선반 내부는 양방향으로 연결한다.
            for (var y = shelfBox.Y; y <= shelfBox.YEnd; y++)
            {
                graph.nodes[shelfBox.X, y].CanGotoRight();
                graph.nodes[shelfBox.X + 1, y].CanGotoLeft();
            }

            // 왼쪽 아래에선 계산대 박스로 내려갈 수 있어야 하고,
            graph.nodes[shelfBox.X, shelfBox.Y].CanGotoDown();
            // 오른쪽 아래 계산대 박스에선 선반 박스로 올라올 수 있어야 한다.
            graph.nodes[shelfBox.XEnd, shelfBox.Y - 1].CanGotoUp();
            // 왼쪽 위 배달 박스에선 선반 박스로 내려올 수 있어야 한다.
            graph.nodes[shelfBox.X, shelfBox.YEnd + 1].CanGotoDown();
            // 오른쪽 위에선 배달 박스로 올라갈 수 있어야 한다.
            graph.nodes[shelfBox.XEnd, shelfBox.YEnd].CanGotoUp();
        }
    }
    #endregion
    
    #region Delivery Box
    static void MakeDeliveryBoxConnection(Graph graph, Rect garageBox, Rect deliveryBox)
    {
/*
  0 1 2 3 4 5 6 7 8
8 _ _ _ g←g←g←g _ _
        ↓ ↕ ↕ ↑    
7 _ d←d←d←d←d←d _ _
    ↓ ↕ ↕ ↕ ↕ ↑    
6 _ d→d→d→d→d→d _ _
*/
        // 배달 박스 생긴게 계산대 생긴거랑 같다?!
        // 재활용하자.
        MakeRegisterBoxConnection(graph, deliveryBox);

        // 차고 연결
        // 양쪽 끝 일방통행
        graph.nodes[garageBox.X, garageBox.Y].CanGotoDown();
        graph.nodes[garageBox.XEnd, garageBox.Y - 1].CanGotoUp();
        
        // 중간 양방통행 (한 칸이거나 없거나. 그런데 확장성을 고려해서 for로 해본다. 처음 설계도 4칸이었고..)
        for (var x = garageBox.X + 1; x <= garageBox.XEnd - 1; x++)
        {
            graph.nodes[x, garageBox.Y].CanGotoDown();
            graph.nodes[x, garageBox.Y - 1].CanGotoUp();
        }
        
        // 차고 안에서는 왼쪽 일방통행
        for (var x = garageBox.X + 1; x <= garageBox.XEnd; x++)
        {
            graph.nodes[x, garageBox.Y].CanGotoLeft();
        }
    }
    #endregion
    #endregion
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

    private readonly IGraph _graph;

    public Node(IGraph graph, int x, int y)
    {
        _graph = graph;
        (X, Y) = (x, y);
        _next = new Lazy<INode[]>(() => new [] { Up, Down, Left, Right }
            .OfType<INode>()
            .ToArray());
    }
    
    public void CanGotoUp()
    {
        Up = _graph.GetNode(X, Y + 1);
    }
    public void CanGotoDown()
    {
        Down = _graph.GetNode(X, Y - 1);
    }
    public void CanGotoLeft()
    {
        Left = _graph.GetNode(X - 1, Y);
    }
    public void CanGotoRight()
    {
        Right = _graph.GetNode(X + 1, Y);
    }
}
