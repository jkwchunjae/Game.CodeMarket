<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load "../basecode"
#load "./StaticData"
#load "./StoreMap"
#load "./PathManager"
#load "./ServingRobot"

var staticData = new StaticData();
var storeMap = new StoreMap(staticData);
var pathManager = new PathManager(storeMap);

var servingRobot = new ServingRobot(staticData, storeMap, pathManager);

servingRobot.Run();

