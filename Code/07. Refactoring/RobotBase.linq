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
    protected MyCell MyCell => new MyCell(robot.GetX(), robot.GetY());
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
        //if (robot.GetX() == target.X && robot.GetY() == target.Y)
        //{
        //    return true;
        //}
        //else if (robot.GetX() + 1 == target.X && robot.GetY() == target.Y)
        //{
        //    return robot.Move(Direction.Right);
        //}
        //else if (robot.GetX() - 1 == target.X && robot.GetY() == target.Y)
        //{
        //    return robot.Move(Direction.Left);
        //}
        //else if (robot.GetX() == target.X && robot.GetY() + 1 == target.Y)
        //{
        //    return robot.Move(Direction.Up);
        //}
        //else if (robot.GetX() == target.X && robot.GetY() - 1 == target.Y)
        //{
        //    return robot.Move(Direction.Down);
        //}
        //else
        //{
        //    while (Goto(target) == false)
        //    {
        //        // 멀다! 요청하자.
        //        var paths = _path.GetPath(MyCell, target);
        //        foreach (var cell in paths)
        //        {
        //            if (Goto(cell) == false)
        //            {
        //                break;
        //            }
        //        }
        //    }
        //    return true;
        //}
        var x = target.X;
        var y = target.Y;
        while (true)
        {
            if (robot.GetX() == x)
            {
                if (robot.GetY() == y)
                {
                    // 도착할 때 까지 계속 움직여본다.
                    return true;
                }
            }
            if (robot.GetX() == 3)
            {
                while (robot.GetY() < y)
                {
                    if (robot.CanMove(Direction.Up))
                    {
                        if (robot.Move(Direction.Up) == false)
                            break;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (robot.GetX() == 2)
            {
                while (robot.GetY() > y)
                {
                    if (robot.CanMove(Direction.Down))
                    {
                        if (robot.Move(Direction.Down) == false)
                            break;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            while (robot.GetX() < x && (robot.GetY() == 2 || robot.GetY() == 3 || robot.GetY() == 7))
            {
                if (robot.CanMove(Direction.Right))
                {
                    if (robot.Move(Direction.Right) == false)
                        break;
                }
                else
                {
                    break;
                }
            }
            while (robot.GetX() > x && (robot.GetY() == 2 || robot.GetY() == 6 || robot.GetY() == 7))
            {
                if (robot.CanMove(Direction.Left))
                {
                    if (robot.Move(Direction.Left) == false)
                        break;
                }
                else
                {
                    break;
                }
            }
        }
    }
}

