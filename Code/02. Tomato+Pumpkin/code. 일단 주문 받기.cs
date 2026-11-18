robot.Move(Direction.Right);
robot.Move(Direction.Right);
robot.Move(Direction.Right);
robot.Move(Direction.Left);
robot.Move(Direction.Down);

Order order = null;
while (order == null)
{
    order = robot.TakeOrder();
}
