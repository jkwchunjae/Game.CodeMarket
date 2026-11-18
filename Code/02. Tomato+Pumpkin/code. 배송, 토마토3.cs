robot.GetList().Clear();

robot.GetList().Add(Product.Tomato);
robot.GetList().Add(Product.Tomato);
robot.GetList().Add(Product.Tomato);

store.CallDelivery(robot.GetList());

robot.Move(Direction.Right);
robot.Move(Direction.Right);
robot.Move(Direction.Right);
robot.Move(Direction.Up);
robot.Move(Direction.Up);

while (robot.GetLoad() != 3)
{
    robot.Pick(Product.Tomato);
    robot.Pick(Product.Tomato);
    robot.Pick(Product.Tomato);
}