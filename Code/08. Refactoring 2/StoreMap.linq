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


interface IStoreMap
{
    /// <summary> 배송 로봇의 전체 이동 경로. 실제로는 들고있는 제품에 따라서 달라질 수 있다. </summary>
    MyCell[] GetShelfCells();
    /// <summary> 선반 앞에 서있는지 확인한다.</summary>
    bool IsFrontOfShelf(MyCell cell);
    /// <summary>현재 위치에서 접근할 수 있는 선반 정보</summary>
    CellProductPair? GetCellProductPair(MyCell cell);
    ///
    CellProductPair? GetCellProductPairWithProduct(Product product);
}
class StoreMap : IStoreMap
{
    private readonly IStaticData _static;
    private readonly List<CellProductPair> _cellProductList = new();
    
    public StoreMap(IStaticData staticData)
    {
        _static = staticData;
        _cellProductList = CreateCellData(_static);
    }

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
    
    private List<List<Node>> 
    
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
}


