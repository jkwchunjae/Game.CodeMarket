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

var deliveryRobot = new DeliveryRobot(staticData, storeMap, pathManager);

deliveryRobot.Run();

