<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load "./StoreMap"
#load "./Node"

interface IPathManager
{
    /// <summary>시작점과 도착점을 포함하는 최단 경로. 도달할 수 없으면 빈 배열을 반환한다.</summary>
    MyCell[] GetPath(MyCell start, MyCell end);
}

class PathManager : IPathManager
{
    private readonly IStoreMap _map;
    private readonly IGraph _graph;
    public PathManager(IStoreMap map)
    {
        _map = map;
        _graph = Graph.Create(map);
    }

    public MyCell[] GetPath(MyCell start, MyCell end)
    {
        var startNode = _graph.GetNode(start.X, start.Y);
        var endNode = _graph.GetNode(end.X, end.Y);
        var queue = new Queue<INode>();
        // 방문 여부와 경로 복원에 필요한 이전 노드를 함께 기록한다.
        var previous = new Dictionary<INode, INode?> { [startNode] = null };
        queue.Enqueue(startNode);

        // 모든 간선의 이동 비용이 같으므로 BFS로 최단 경로를 찾는다.
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current == endNode)
            {
                var path = new List<MyCell>();
                for (INode? node = current; node is not null; node = previous[node])
                {
                    path.Add(new MyCell(node.X, node.Y));
                }
                path.Reverse();
                return path.ToArray();
            }

            foreach (var next in current.Next)
            {
                if (previous.TryAdd(next, current))
                {
                    queue.Enqueue(next);
                }
            }
        }

        return Array.Empty<MyCell>();
    }
}
