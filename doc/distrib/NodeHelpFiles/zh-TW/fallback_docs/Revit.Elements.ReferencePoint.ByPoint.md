## 深入資訊
`ReferencePoint.ByPoint` 會在作用中 Revit 族群文件中的給定點位置建立參考點元素。注意事項: 族群文件必須是自適應元件或量體族群。此節點與「ReferencePoint.ByCoordinates」不同，因為它使用 Dynamo 點當作位置。這很實用，因為終端使用者可以使用直接操控的方式編輯 Dynamo 點，讓參考點也跟著更新。如需直接操控的更多資訊，請參閱此 [連結](https://primer2.dynamobim.org/10_sample_workflow/10-1_getting-started-workflows/2-attractor-points#adjusting-with-direct-manipulation)。

以下範例在座標 0,0,1 建立新參考點。
___
## 範例檔案

![ReferencePoint.ByPoint](./Revit.Elements.ReferencePoint.ByPoint_img.jpg)
