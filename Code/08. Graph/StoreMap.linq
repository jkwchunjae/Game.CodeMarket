<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load "../basecode"
#load "./StaticData"

record MyCell(int X, int Y)
{
    public bool Equal(MyCell cell)
    {
        return X == cell.X && Y == cell.Y;
    }
    
    public string ToStr()
    {
        return $"({X}, {Y})";
    }
}
/// <summary>Cell에서 Direction방향을 보면 Product를 잡을 수 있다</summary>
record CellProductPair(MyCell ShelfPosition, MyCell Cell, Product Product, Direction Direction)
{
    public int X => Cell.X;
    public int Y => Cell.Y;
}

/// <summary>상점이 모여있는 단위. 보통 로봇 하나가 담당한다.</summary>
class ShelfGroup
{
    private readonly CellProductPair[] _pairs;

    /// <summary>상단 입구 (좌측 상단)</summary>
    public MyCell TopEnter { get; private set; }
    /// <summary>하단 입구 (우측 하단)</summary>
    public MyCell BottomEnter { get; private set; }
    /// <summary>이 로봇이 담당해야 할 상품. 이 상품만 트럭에서 꺼낸다.</summary>
    public Product[] TargetProducts { get; private set; }
    
    public ShelfGroup(CellProductPair[] pairs)
    {
        _pairs = pairs;
        
        // top enter
        var leftX = pairs.Min(p => p.Cell.X);
        var topY = pairs.Max(p => p.Cell.Y);
        TopEnter = new MyCell(leftX, topY);
        // bottom enter
        var rightX = pairs.Max(p => p.Cell.X);
        var bottomY = pairs.Min(p => p.Cell.Y);
        BottomEnter = new MyCell(rightX, bottomY);
        
        TargetProducts = pairs.Select(x => x.Product).Distinct().ToArray();
    }
    
    /// <summary>products중 하나라도 이 그룹에 포함되어 있는지 체크</summary>
    public bool ContainsProducts(Product[] products)
    {
        return _pairs.Any(x => products.Contains(x.Product));
    }
    
    public void Traverse(Action<CellProductPair> action)
    {
        
    }
}

interface IStoreMap
{
    MyCell GarageCell { get; }
    /// <summary> 배송 로봇의 전체 이동 경로. 실제로는 들고있는 제품에 따라서 달라질 수 있다. </summary>
    MyCell[] GetShelfCells();
    /// <summary> 선반 앞에 서있는지 확인한다.</summary>
    bool IsFrontOfShelf(MyCell cell);
    /// <summary>현재 위치에서 접근할 수 있는 선반 정보</summary>
    CellProductPair? GetCellProductPair(MyCell cell);
    ///
    CellProductPair? GetCellProductPairWithProduct(Product product);
    /// <summary> 선반에 접근할 수 있는 셀 목록. 여기에서 넣고 뺄 수 있다. Graph를 만들 때 사용한다. </summary>
    MyCell[] GetFrontOfShelfCells();
    /// <sumamry> 선반 그룹 목록을 반환한다. </summary>
    ShelfGroup[] GetAllShelfGroup(int startIndex);
    /// <sumamry> 선반 그룹 목록을 반환한다. </summary>
    ShelfGroup GetShelfGroup(int index);
}
class StoreMap : IStoreMap
{
    private readonly IStaticData _static;
    private readonly List<CellProductPair> _cellProductList;
    private readonly ShelfGroup[] _shelfs;
    
    public MyCell GarageCell => new MyCell(store.GetGarageX(), store.GetGarageY());
    
    public StoreMap(IStaticData staticData)
    {
        _static = staticData;
        _cellProductList = CreateCellData(_static);
        _shelfs = SplitShelfGroup();
    }

    #region Create Cell Product Pair
    private List<CellProductPair> CreateCellData(IStaticData staticData)
    {
        List<CellProductPair> cellProductList = new();
        for (var x = 0; x < store.GetWidth(); x++)
        {
            for (var y = 0; y < store.GetHeight(); y++)
            {
                var entity = store.GetEntity(x, y);
                if (entity != null)
                {
                    var product = staticData.GetProductFromEntity(entity.Value);
                    if (product != null)
                    {
                        var pickupData = GetPickupData(x, y, product);
                        cellProductList.Add(pickupData);
                    }
                }
            }
        }
        return cellProductList;
    }
    
    /// <summary>(x, y) 진열대에서 물건을 집을 수 있는 셀을 구한다.</summary>
    private CellProductPair GetPickupData(int x, int y, Product product)
    {
        // 진열대의 왼쪽이나 오른쪽에서만 집을 수 있다.
        if (IsPickupCell(x - 1, y, product))
        {
            return new(new MyCell(x, y), new MyCell(x - 1, y), product, Direction.Right);
        }
        if (IsPickupCell(x + 1, y, product))
        {
            return new(new MyCell(x, y), new MyCell(x + 1, y), product, Direction.Left);
        }
        return null;
    }
    /// <summary>(x, y)에서 물건을 집을 수 있는지 확인한다.</summary>
    private bool IsPickupCell(int x, int y, Product product)
    {
        var isInStore = x >= 0 && x < store.GetWidth()
            && y >= 0 && y < store.GetHeight();
            
        if (isInStore)
        {
            var directions = store.GetPickupDirections(x, y, product);
            return directions.Count() > 0;
        }
        else
        {
            return false;
        }
    }
    #endregion
    
    public MyCell[] GetShelfCells()
    {
        MyCell[] cells = new MyCell[]
        {
            new MyCell(2, 6),
            new MyCell(2, 5),
            new MyCell(2, 4),
            new MyCell(2, 3),
            new MyCell(3, 3),
            new MyCell(3, 4),
            new MyCell(3, 5),
            new MyCell(3, 6),
            new MyCell(3, 7),
        };
        return cells;
    }

    public bool IsFrontOfShelf(MyCell cell)
    {
        var cellData = GetCellProductPair(cell);
        return cellData != null;
    }

    public CellProductPair? GetCellProductPair(MyCell cell)
    {
        return _cellProductList
            .FirstOrDefault(x => x.Cell.Equal(cell));
    }

    public CellProductPair? GetCellProductPairWithProduct(Product product)
    {
        return _cellProductList.FirstOrDefault(x => x.Product == product);
    }

    public MyCell[] GetFrontOfShelfCells()
    {
        return _cellProductList
            .Select(x => x.Cell)
            .ToArray();
    }
    
    #region ShelfLine
    private ShelfGroup[] SplitShelfGroup()
    {
        List<List<CellProductPair>> items = new List<List<CellProductPair>>();
        foreach (var data in _cellProductList)
        {
            var line = items.FirstOrDefault(line => Math.Abs(line[0].Cell.X - data.Cell.X) <= 1);
            if (line == default)
            {
                items.Add(new List<CellProductPair> { data });
            }
            else
            {
                line.Add(data);
            }
        }
        return items
            .Select(line => new ShelfGroup(line.ToArray()))
            .OrderBy(line => line.TopEnter.X)
            .ToArray();
    }
    public ShelfGroup[] GetAllShelfGroup(int startIndex)
    {
        List<ShelfGroup> shelfs = new List<ShelfGroup>();
        for (var i = 0; i < _shelfs.Count(); i++)
        {
            var index = (i + startIndex) % _shelfs.Count();
            shelfs.Add(_shelfs[index]);
        }
        return shelfs.ToArray();
    }
    public ShelfGroup GetShelfGroup(int index)
    {
        return _shelfs[index];
    }
    #endregion
}


