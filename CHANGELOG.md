## 0.0.45
* Add nodes for path of travel element.
* Upgrade DynamoRevit Version to 2.2.1.

## 0.0.44
* Update Greg & GregRevitAuth to latest version (Greg - 1.1.7040.19960 GregRevitAuth - 1.0.7057.20655)
* Consume the version 2.2.0.4667 of DynamoCoreRuntime in D4R's CICD
* Consume the version 20.1.0-b21 of RevitAPI and RevitAPIUI

## 0.0.43
* Direct Revit Version to 2020 in config file

## 0.0.42
* Update Dynamo Core Version to 2.2.0.4565 in Config file-packages.aget
* Add CanBuildOutputAst function to RevitDropDownBase for judging whether it have valid Enumeration values to the output in dropdown list
* Update RevitSystemTestBase to make it more user friendly
* Add two new nodes to ImportInstance because of RevitAPI changes
* Make Nodes FaceAnalysisDisplay.ByViewAndFaceAnalysisData, PointAnalysisDisplay.ByViewAndPointAnalysisData, VectorAnalysisDisplay.ByViewAndVectorAnalysisData invisible in Dynamo because SurfaceData, PointData and VectorData was set obsolete in DynamoCore.
* Improve RevitSystemTestBase that DynamoRevit custom node developers could preload their nodes for test in Code.
* Improve RevitSystemTestBase that DynamoRevit custom node developers could preload their node packages for test in Code.

## 0.0.41
* Update Jenkinsfile to compatible with RC2.1.0_Revit2020 branch's CICD.

## 0.0.40
* The version of master branch will be start with 0.1.x, and Revit2020 branch will still be 0.0.xx.
* Cherry-pick fix from master branch, LibG should be more version compatible.

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