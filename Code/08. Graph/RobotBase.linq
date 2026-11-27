<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load "../basecode"
#load "./StoreMap"
#load "./PathManager"
#load "./StaticData"

abstract class RobotBase
{
    protected IStaticData _static;
    protected IStoreMap _map;
    protected IPathManager _path;
    protected MyCell CurrentCell => new MyCell(robot.GetX(), robot.GetY());
    public RobotBase(IStaticData staticData, IStoreMap map, IPathManager pathManager)
    {
        _static = staticData;
        _map = map;
        _path = pathManager;
    }
    /// <summary>목적지로 간다. 못 가면 false</summary>
    protected bool Goto(MyCell target, bool canChangePath = false)
    {
        // 이미 그 위치면 움직일 필요 없다.
        // 내 근처이면 그냥 움직인다.
        // 멀리 있으면 경로를 계산해서 움직인다.
        if (robot.GetX() == target.X && robot.GetY() == target.Y)
        {
            return true;
        }
        else if (robot.GetX() + 1 == target.X && robot.GetY() == target.Y)
        {
            while (true)
            {
                if (robot.CanMove(Direction.Right))
                {
                    robot.Move(Direction.Right);
                    return true;
                }
                else if (canChangePath)
                {
                    return false;
                }
            }
        }
        else if (robot.GetX() - 1 == target.X && robot.GetY() == target.Y)
        {
            while (true)
            {
                if (robot.CanMove(Direction.Left))
                {
                    robot.Move(Direction.Left);
                    return true;
                }
                else if (canChangePath)
                {
                    return false;
                }
            }
        }
        else if (robot.GetX() == target.X && robot.GetY() + 1 == target.Y)
        {
            while (true)
            {
                if (robot.CanMove(Direction.Up))
                {
                    robot.Move(Direction.Up);
                    return true;
                }
                else if (canChangePath)
                {
                    return false;
                }
            }
        }
        else if (robot.GetX() == target.X && robot.GetY() - 1 == target.Y)
        {
            while (true)
            {
                if (robot.CanMove(Direction.Down))
                {
                    robot.Move(Direction.Down);
                    return true;
                }
                else if (canChangePath)
                {
                    return false;
                }
            }
        }
        else
        {
            MyCell[] excepted = new MyCell[] {};
            int tryCount = 0;
            while (true)
            {
                tryCount++;
                if (tryCount > 10)
                {
                    Console.Error.WriteLine($"경로찾기 중: target: {target.ToStr()}");
                }
                var shortestPath = _path.GetPath(CurrentCell, target, excepted);
                foreach (var nextTarget in shortestPath)
                {
                    if (Goto(nextTarget))
                    {
                        continue;
                    }
                    else if (canChangePath)
                    {
                        // 경로를 다시 계산한다.
                        excepted = new MyCell[] { nextTarget };
                        break;
                    }
                    else
                    {
                        // 그냥 계속 재시도한다.
                        while (Goto(nextTarget) == false)
                        {
                        }
                    }
                }
                if (robot.GetX() == target.X && robot.GetY() == target.Y)
                {
                    return true;
                }
            }
        }
    }
}

