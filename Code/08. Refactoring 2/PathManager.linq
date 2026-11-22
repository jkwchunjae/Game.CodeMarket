<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load "./StoreMap"

interface IPathManager
{
    /// <summary> start -> end </summary>
    //MyCell[] GetPath(MyCell start, MyCell end);
    /// <summary> 주로 배송 로봇이 목적지 목록을 입력하면 경로를 알려준다. </summary>
    MyCell[] GetPathTraverse(MyCell start, MyCell[] targets);
    
    bool ContainsInCell(MyCell[] cells, MyCell cell);
}

class PathManager : IPathManager
{
    private readonly IStoreMap _map;
    public PathManager(IStoreMap map)
    {
        _map = map;
    }
    public bool ContainsInCell(MyCell[] cells, MyCell cell)
    {
        return cells.Any(c => c.Equal(cell));
    }

    //public MyCell[] GetPath(MyCell start, MyCell end)
    //{
    //    // 
    //    MyCell cell = new(start.X, start.Y);
    //    var path = new List<MyCell>
    //    {
    //        cell,
    //    };
    //    return path.ToArray();
    //}

    public MyCell[] GetPathTraverse(MyCell start, MyCell[] targets)
    {
        return new MyCell[]
        {
            new(2, 8),
            new(2, 7),
            new(2, 6),
            new(2, 5),
            new(2, 4),
            new(2, 3),
            new(3, 3),
            new(3, 4),
            new(3, 5),
            new(3, 6),
            new(3, 7),
            new(3, 8),
        };
    }
}