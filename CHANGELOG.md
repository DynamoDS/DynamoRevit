## 0.1.15
* Update .Net to 4.8.

## 0.1.14
* Update DynamoCore to 2.5.0.7432
* Update DynamoCore Runtime to 2.5.0.7460

## 0.1.13
* update DynamoCore to 2.5.0.7186

## 0.1.12
* Remove transient elements in Revit doc when Dynamo shutdown
* Fix test failure of JsonRestoresBinding

## 0.1.11
* Element nodes second batch - add 5 new nodes for DynamoRevit

## 0.1.10
* Update README.md
* Address inconsistency with PathOfTravel Icons and other Revit Node icons
* Update Roof.cs to avoid memory issue.
* Fix Port Name - Wall.ByCurveAndLevels node.
* Element nodes first batch - add 5 new nodes for DynamoRevit
* Update dynamocore version to 2.5.0-beta6607

## 0.1.9
* Preload dynamically internal packages

## 0.1.8
* Update Python and NUnit versions

## 0.1.7
* Update DynamoCore Version to 2.4.0.6186

## 0.1.6
* Update DynamoCore Version to 2.3.0.5885
* Update DSRevitNodesUIImages & DSRevitNodesUI version to 2.0.0.0 due to PathOfTravel Node Resource added

## 0.1.5
* Exit Dynamo when Revit document lost.
* Correct spelling mistakes in Select Rule Type Node
* Resolve DynamoCore new version 2.2.1.5204
* Resolved CategoryByName Node not working with some German words
* Spell error in tooltip for Element.GetLocation Node 
* Add new Select node "All Elements of Category In View"
* Add Element Delete Node
* Update some failed nodes tests
* Add Node Icons for PathOfTravel, Element Delete, All Elements of Category In View nodes.

## 0.1.4
* Add System tests for Sample files.
* Refactoring some RevitSystemTestBase codes.
* Update RayBounce_SunStudy test due to RevitAPI update.
* Add nodes for path of travel element.
* Upgrade DynamoRevit Version to 2.2.1

## 0.1.3
* Update Greg & GregRevitAuth to latest version (Greg - 1.1.7040.19960 GregRevitAuth - 1.0.7057.20655)
* Consume the version 2.2.0.4667 of DynamoCoreRuntime in D4R's CICD

## 0.1.2
* Update Dynamo Core Version to 2.2.0.4565 in Config file-packages.aget 

## 0.1.1
* Add CanBuildOutputAst function to RevitDropDownBase for judging whether it have valid Enumeration values to the output in dropdown list
* Update RevitSystemTestBase to make it more user friendly
* Add two new nodes to ImportInstance because of RevitAPI changes
* Make Nodes FaceAnalysisDisplay.ByViewAndFaceAnalysisData, PointAnalysisDisplay.ByViewAndPointAnalysisData, VectorAnalysisDisplay.ByViewAndVectorAnalysisData invisible in Dynamo because SurfaceData, PointData and VectorData was set obsolete in DynamoCore.
* Improve RevitSystemTestBase that DynamoRevit custom node developers could preload their nodes for test in Code.
* Improve RevitSystemTestBase that DynamoRevit custom node developers could preload their node packages for test in Code.

## 0.1.0
* The version of master branch will be start with 0.1.x, and Revit2020 branch will still be 0.0.xx.
* LibG should be more version compatible.

## 0.0.39
* Consume the version 20.0.0-b94 of RevitAPI and RevitAPIUI in D4R's CICD to support Revit 2020 release.

## 0.0.38
* Consume the version 2.1.0.7500 of DynamoCoreRuntime in D4R's CICD resolve OpenIfSaved API for DynamoPlayer.

## 0.0.37
* Consume the version 2.1.0.7464 of DynamoCoreRuntime in D4R's CICD

## 0.0.36
* Consume the version 2.1.0.7455 of DynamoCoreRuntime in D4R's CICD

## 0.0.35
* Consume the final DynamoCore 2.1 nugets

## 0.0.34
* Update version for CICD.

## 0.0.33
* Compile DynamoRevit with the correct ProtoGeometry version
* DropDown Node in Dynamo now serialize SelectedString for all the dropdown nodes, among which categories node serializae interal category id

## 0.0.32
* Consume the new API DynamoShapeManager.Utilities.GetLibGPreloaderLocation()
* Upgrade DynamoCore version from 2.1.0-beta6967 to 2.1.0-beta7020
* Disable the D4R upgrade function which would download higher Dynamo Version.

## 0.0.31
* Set 5 tests to ignore because of DynamoRaaS removed.
* Update DynamoCoreRuntime version from 2.1.0-beta6967 to 2.1.0-beta7186 in D4R's CICD.
* Fix crash issue when exit Revit Host Doc and undo/redo in Dynamo.

## 0.0.30
* Remove Simple RaaS from test base.
* Update DynamoCoreRuntime version to resolve ASM upgrade - Libg_225_0_0.

## 0.0.29 
* The PR#1999 3dview-wrapper has been reverted due to it is a API break change, will merge it in next Major release.
* Remove Dynamo RaaS.