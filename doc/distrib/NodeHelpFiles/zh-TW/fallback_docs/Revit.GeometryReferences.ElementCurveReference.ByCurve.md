## 深入資訊
`ElementCurveReference.ByCurve` 會從 Dynamo 曲線建立 Revit 元素曲線參考。當其他 Revit 節點需要一條可選取或可參考的曲線而不只是 Dynamo 幾何圖形時，曲線參考就很有用。這常用於需要 Revit 參考的工作流程，例如建立尺寸標註、定線、約束或其他需要參考模型幾何圖形的元素。

以下範例選取一條曲線作為 `ElementCurveReference.ByCurve` 的輸入。輸出是 `ElementCurveReference`，之後要作為 Revit 曲線參考供需要曲線型參考的下游節點使用。
___
## 範例檔案

![ElementCurveReference.ByCurve](./Revit.GeometryReferences.ElementCurveReference.ByCurve_img.jpg)
