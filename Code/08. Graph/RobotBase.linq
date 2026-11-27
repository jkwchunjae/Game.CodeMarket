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
    protected bool Goto(MyCell target)
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
            }
        }
        else if (robot.GetX() - 1 == target.X && robot.GetY() == target.Y)
        {
            int tryCount = 0;
            while (true)
            {
                if (robot.CanMove(Direction.Left))
                {
                    robot.Move(Direction.Left);
                    return true;
                }
                else
                {
                    tryCount++;
                    if (tryCount >= 5)
                    {
                        // 시도하다가 안되면 일단 위로 올라가보자.
                        robot.Move(Direction.Up);
                    }
                    if (tryCount >= 10)
                    {
                        // 시도하다가 안되면 일단 위로 올라가보자.
                        robot.Move(Direction.Down);
                    }
                }
            }
        }
        else if (robot.GetX() == target.X && robot.GetY() + 1 == target.Y)
        {
            int tryCount = 0;
            while (true)
            {
                if (robot.CanMove(Direction.Up))
                {
                    robot.Move(Direction.Up);
                    return true;
                }
                else if (robot.GetY() >= store.GetHeight() - 3) // 배달로봇의 충돌을 방지 (그냥 경로 하드코딩을 해야 하나..)
                {
                    tryCount++;
                    if (tryCount >= 5)
                    {
                        // 시도하다가 안되면 일단 오른쪽으로 가보자.
                        robot.Move(Direction.Right);
                    }
                    if (tryCount >= 10)
                    {
                        // 시도하다가 안되면 일단 왼쪽으로 가보자.
                        robot.Move(Direction.Left);
                    }
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
            }
        }
        else
        {
            var shortestPath = _path.GetPath(CurrentCell, target);
            foreach (var nextTarget in shortestPath)
            {
                // 일단 이동에 성공할 때까지 계속 시도한다.
                // 개선해야함.
                while (true)
                {
                    if (Goto(nextTarget))
                    {
                        // 이동 성공!
                        break;
                    }
                }
            }
            return true;
        }
    }
}

