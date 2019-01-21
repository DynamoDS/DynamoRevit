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