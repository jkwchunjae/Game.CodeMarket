<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load "../basecode"
#load "./StaticData"

record MyCell(int X, int Y);
/// <summary>Cell에서 Direction방향을 보면 Product를 잡을 수 있다</summary>
record CellProductPair(MyCell Cell, Product Product, Direction Direction)
{
    public int X => Cell.X;
    public int Y => Cell.Y;
}


interface IStoreMap
{
    /// <summary> 배송 로봇의 전체 이동 경로. 실제로는 들고있는 제품에 따라서 달라질 수 있다. </summary>
    MyCell[] GetDeliveryCells();
    /// <summary> 선반 앞에 서있는지 확인한다.</summary>
    bool IsFrontOfShelf(MyCell cell);
    CellProductPair GetCellProductPair(MyCell cell);
}
class StoreMap : IStoreMap
{
    private readonly IStaticData _static;
    private readonly List<CellProductPair> _cellProductList = new();
    
    public StoreMap()
    {
        _static = new StaticData();
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
                    var product = staticData.GetProductFromEntity(entity);
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
            return new(new MyCell(x - 1, y), product, Direction.Right);
        }
        if (IsPickupCell(x + 1, y, product))
        {
            return new(new MyCell(x + 1, y), product, Direction.Left);
        }
        throw new UnreachableException();
        //return null;
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
}


