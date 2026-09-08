## 详细
“PathOfTravel.LongestOfShortestExitPaths”创建行进路径，表示从一组点到一个或多个出口点的最长最短路线。每个起点都会根据可用的出口进行测试，并且 Revit 会为每个出口确定最近的有效出口路径。然后，该节点返回最短路径结果中具有最长行进距离的路径。

在下面的示例中，首先创建标高和楼层平面视图，以提供行进路径计算所需的 Revit 视图。使用“Point.ByCoordinates”生成多个点，并将其合并到表示可能出口位置的列表中。“PathOfTravel.LongestOfShortestExitPaths”使用楼层平面视图和目标点列表来计算行进路径，从而返回具有最长行进距离的路径。
___
## 示例文件

![PathOfTravel.LongestOfShortestExitPaths](./Revit.Elements.PathOfTravel.LongestOfShortestExitPaths_img.jpg)
