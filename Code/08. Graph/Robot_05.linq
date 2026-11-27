<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load "../basecode"
#load "./StaticData"
#load "./StoreMap"
#load "./PathManager"
#load "./DeliveryRobot"

var staticData = new StaticData();
var storeMap = new StoreMap(staticData);
var pathManager = new PathManager(storeMap);

var init = new DeliveryRobotInit
{
    StartIndex = 2,
    MyShelfGroups = [2],
    GarageX = 9,
};

var deliveryRobot = new DeliveryRobot(init, staticData, storeMap, pathManager);

deliveryRobot.Run();

