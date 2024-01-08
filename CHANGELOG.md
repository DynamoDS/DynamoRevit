## 0.5.24
* Fixed build warnings in master branch

## 0.5.23
* RC for Revit2025
* Removed configuration for .NET 7

## 0.5.22
* Update Dynamo Core to 3.0.0.7190

## 0.5.21
* Incorrect Category returned by Category.ByName node

## 0.5.20
* (re-)enable Dynamo Splash Screen

## 0.5.19
* Update / Change Dynamo Core to 3.0.0-beta7040

## 0.5.18
* Migrate to .NET 8
* Update Dynamo Core to 3.0.0-beta6975 (.NET 8)
## 0.5.17
* Update Dynamo Core to 3.0.0-beta7044

## 0.5.16
* Update Dynamo Core to 3.0.0-beta6964
* Replace Trace Data from SOAP to JSON

## 0.5.15
* Update Dynamo Core to 3.0.0-beta6885
* Drop SOAP formatter and move to JSON based serialization 

## 0.5.14
* Update Dynamo Core to 3.0.0-beta6827

## 0.5.13
* Update Dynamo Core to 3.0.0-beta6764
* Add HostAnalyticsInfo to pass host related info

## 0.5.12
* Update Dynamo Core to 3.0.0-beta6680

## 0.5.11
* Update Dynamo Core to 3.0.0-beta6634, Greg, and RestSharp
* fix missing DYnamoInstallDetective.dll
* fix yes/no (long type) value for global parameters not being set properly

## 0.5.10
* Fixed issue for node FamilyDocument.SetParameterValueByName that cannot be used for Yes/No family parameters

## 0.5.9
* merge master (linkInstance wrappers, documentation)
* update NUnit to 3.13.3 - work in progress

## 0.5.7
* disable splashcreen due to System.Windows.Application.Current not being null not being null

## 0.5.6
* enable splashcreen
* update NUnit to 3.13.3

## 0.5.5
* Fix some .net7 build issues

## 0.5.3
* Migrade DynamoRevit to .NET 7.0

## 0.4.14
* Upgrade DynamoCore version to 2.17.0-beta3141.

## 0.4.13
* Upgrade DynamoCore version to 2.16.1.
* Update Greg version to 2.3.0.1646.

## 0.4.12
* Upgrade DynamoCore  to 2.16.0.2501.

## 0.4.10
* Add support to UnwrapElement for marshaling CPython dictionary

## 0.4.9
* Make exception message of SetRenderingAssetTextureImage more friendly

## 0.4.8
* Update Dynamo For Revit to support 64-bit ElementId

## 0.4.7
* Make dynamo nodes for material texture easier for end users to use

## 0.4.6
* keep AssemblySharedInfo version same with dynamo core

## 0.4.5
* Upgrade DynamoCore  to 2.15.0.5383.

## 0.4.4
* Fix bug cannot set parameter value of integer type in Dynamo.

## 0.4.3
* Fix bug cannot set parameter value of integer type in Dynamo.

## 0.4.2
* Add 3 new nodes - Material.AppearanceAssetElement, AppearanceAssetElement.GetRenderingAssetTextureImages, AppearanceAssetElement.SetRenderingAssetTextureImage

## 0.4.1
* Update Foreground Color For Selection by Category Nodes.
* Update ComboBox Style for Selection by Category Nodes.
* Update Select Button for Selection by Category Nodes..

## 0.4.0
* Update master branch to 2.14.0.
* Fix ForgeUnit converter between some internal Revit Unit Types and ForgeUnit SDK.

## 0.3.9
* Upgrade DynamoCore Lib to involve new version of Sample rvt file.

## 0.3.8
* Upgrade Paramater.CreateProjectParameter due to Definition will sometimes be null.
* Update dynamo python for 2.13.
* Add Unit nodes to Document, Parameter, and FamilyParameter
* ForgeType Refactor.
* Fix bug - SelectModelElementByCategory and SelectModelElementsByCategory are set to the wrong Category if opened in a different language.
* Upgrade DynamoCore lib to 2.13.0.3485.

## 0.3.7
* Add Transaction controls - "Sync with Revit" Toggle button.
* Upgrade Greg, GregRevitOAuth, and RestSharp to RestSharp 106.12.0 to address a security issue.
* Use OAuth2 provider to communicate with package manager.
* Expose DynamoViewModel property.
* Upgrade DynamoCore Lib to 2.13.0-beta2845, and use the latest DynamoCoreRuntime with ASM 228.
* Hide Transaction Control button.

## 0.3.6
* Upgrade DynamoRevit to 2.13.0.

## 0.3.5
* Remove Obsolete code due to the API was removed from Revit side.

## 0.3.4
* Improve Floor creation node and Revision creation node API.
* Improve 2 DocumentTests.
* Improve Category.ByName Nodes due to the current way can't get Views cateory in non-English languages.
* Add Performance tests for Revit.

## 0.3.3
* Add new FamilyInstance node - FamilyInstance.ByHostAndPoint.
* Update DynamoCore to 2.10.0,
* Update Revision and FamilyDocument API due to Revit API changed.
* Fix a bug - Toggling "Show Edges" or "Revit Background Preview" setting causes blank Geometry View until regeneration.

## 0.3.2
* Add new selection node  - "Select Reference on Element" which can select a reference on an element (contains linked element).
* Add new Viewport nodes - Viewport.LabelOffset, Viewport.SetLabelOffset, Viewport.LabelLineLength, Viewport.SetLabelLineLength.
* Update Floor API with new RevitAPI.
* Add new Ceiling&CeilingType nodes - Ceiling.ByOutlineTypeAndLevel, Ceiling.ByOutlineTypeAndLevel(polycurve), Ceiling Types, CeilingType.ByName, CeilingType.Name, CeilingTypeGetThermalProperties.
* Update ParameterType and some related contents because of RevitAPI changed.

## 0.3.1
* Fix CICD issue with nuget.

## 0.3.0
* Rename "Element Types" to "Element Classes" distinguished from "ElementType".
* Improve AnalysisDisplay Nodes.
* Upgrade DynamoSample file to Revit 2022.
* Upgrade DynamoCore to 2.10.1 to fix CPython3 issues.
* Upgrade Tag Nodes with New Revit API (Preview Release), and only support one tag with one element.

## 0.2.25
* Update DynamoCore version 2.10.0 to updated nuget packages.

## 0.2.24
* Add icons for new TagNodes.
* Upgrade DynamoCore version to 2.10.0.

## 0.2.23
* Upgrade DynamoCore version to 2.9.0 with ASM 227.

## 0.2.22
* Improve a Systemtests, CanPurgeUnusedElementsFromDocument, which use lots of element id and will fail due to changes in RevitAPI.
* Fix a issue that it will have an offset when select a face or faces from Mass FamilyInstance into Dynamo.
* Add some new Tag nodes - Tag.HeadLocation, Tag.LeaderElbow, Tag.LeaderEnd, Tag.SetHeadLocation, Tag.SetLeaderElbow, Tag.SetLeaderEnd, Tag.LeaderEndCondition, Tag.SetLeaderEndCondition, LeaderEnd Condition


## 0.2.21
* Restore the Hide Dimension nodes and Add Dimension.ByElementDirection
* Add icons for some view nodes.

## 0.2.20
* Add some new Nodes - Dimension.ByFaces, Dimension.ByEdges, Dimension.ByReferences, ElementCurveReference.ByCurve, ElementFaceReference.BySurface.
* Emergency Upgrade - Hide Dimension new nodes temporary.

## 0.2.19
* Fix a bug - filter out unavailable Parameters from Material.
* Add some view 3d properties - View.Outline, View.Origin, View.Scale, View.CropBox, View.SetCropBox, View.CropBoxActive, View.SetCropBoxActive, View.CropBoxVisible, View.SetCropBoxVisible, View.ViewDirection, View.RightDirection.
* Add some view properties - View.Discipline, View.SetDiscipline, View.DisplayStyle, View.SetDisplayStyle, View.SketchPlane, View.SetSketchPlane.
* Add some view properties - View.CanViewBeDuplicated, View.Partsvisibility, View.SetPartsVisibility
* Add some new nodes - View.GetCategoryOverrides, View.IsCategoryHidden, Element.OverridesInView, Element.IsHiddeninView
* Update DynamoCore to 2.8.0 for Dynamo CPython3 engine.
* Add some View Nodes - View.HideCategoriesTemporary, View.HideElementsTemporary, View.IsolateCategoriesTemporary, View.IsolateElementsTemporary.
* Improve DuplicateSheet Node - add "duplicateWithContents" option, add default suffix value when prefix & suffix are both empty.
* Improve DuplicateSheet Node - Set Sheet information when duplicating; Improve View Temporary Nodes.
* Add 4 UI Nodes - View Duplicate Options, View Disciplines, View DisplayStyles, View PartsVisibilitys
* Fix bug - Element.GetParentElement can't get StairsRun Parent Element.

## 0.2.18
* Add a Category ScheduleOnSheet and its nodes - ScheduleOnSheet.Sheet, ScheduleOnSheet.Schedule, ScheduleOnSheet.BySheetViewLocation, ScheduleOnSheet.Location and ScheduleOnSheet.SetLocation
* Add some new nodes for Sheet - Sheet.Schedules, Sheet.Viewports, Sheet.TitleBlock, Sheet.SetSheetName, Sheet.SetSheetNumber
* Add 2 new nodes - View.DuplicateView and Sheet.DuplicateSheet.
* Add 2 new nodes - Sheet.ByNameNumberTitleBlockViewsAndLocations and Sheet.ByNameNumberTitleBlockViewAndLocation

## 0.2.17
* Check if InternalElement is valid Revit Element to avoid calling funcitons or get properties of an invalid object and cause Dynamo crash.
* Add some new nodes to Viewport - Viewport.Sheet, Viewport.View, Viewport.BoxCenter, Viewport.SetBoxCenter, Viewport.BoxOutline and Viewport.LabelOutline.

## 0.2.16
* Update Test_Python to avoid test results that will vary with API changes
* Update DynamoCoreRuntime version due to emergency fix

## 0.2.15
* Update DynamoCore version to 2.6.

## 0.2.14
* Implemented Additional Elements for Warning.GetFailingElements
* Add a listener of changes in Revit for DropDown list nodes
* Add new node - SelectModelElementByCategory 

## 0.2.13
* Fix Roof.ByOutlineExtrusionTypeAndLevel bug - the input outline should not be closed.
* Longest of shortest exit paths node
* Add Get Remove Insert and Set waypoints nodes on a PathOfTravel element

## 0.2.12
* Fix some tests was deleted by mistake.

## 0.2.11
* Bump a new version due to package mistake.

## 0.2.10
* Fix All Elements of Element Type supporting less element types than before
* Update ScheduleView.Export comment
* Update some system tests which category is failure.
* Update Wall node to allow creation from ellipses
* Update New Nodes Descriptions

## 0.2.9
* Update RayBounce_SunStudy test.
* Update some system test which category is failure.
* Add new Sheet nodes.
* Add new Selection Nodes.

## 0.2.8
* Update DynamoCore Runtime to 2.5.2.7915

## 0.2.7
* Fix CI build error.

## 0.2.6
* Update some System tests and their dynamo files.
* Rename "IncludeShadows" in node "Element.GetHostedElements".
* Upgrade some obsolete classed and functions for RevitAPI updated

## 0.2.5
* Update RevitAPI folder name to net48.

## 0.2.4
* Update Assembly Shared Info for Revit Preview Release

## 0.2.3
* Element Type nodes - introduces an ElementType category to the Dynamo library
* Added some new Category Nodes - Group nodes, Space nodes, Elevation marker nodes, Dimension nodes, Area nodes, View port nodes, Family document nodes, Family instance nodes, Warning nodes.
* Element.Geometry should provide geometry for FamilySymbols with no instances

## 0.2.2
* Update .Net to 4.8.

## 0.2.1
* Element nodes third batch - contains 3 nodes related to Revit.Elements.Element.
* Update DynamoCore Runtime to 2.5.0.7460

## 0.2.0
* Fix issue - Joined geometry does not convert correctly
* Document nodes - Add 3 node in the Application.Document category
* Update Dynamo Core to 2.5.0.7432

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
