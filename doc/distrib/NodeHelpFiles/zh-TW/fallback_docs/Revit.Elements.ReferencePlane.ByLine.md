## 深入資訊
Dynamo 中的 ReferencePlane.ByLine 節點會使用定義的線作為基準來建立 Revit 參考平面。您可以用特定位置和方位產生自訂參考平面。

在此範例中，使用 Point.ByCoordinates 搭配可調整的滑棒定義兩個點，然後在這兩點之間建立 Line.ByStartPointEndPoint，最後，ReferencePlane.ByLine 節點會沿著該條線產生參考平面。
___
## 範例檔案

![ReferencePlane.ByLine](./Revit.Elements.ReferencePlane.ByLine_img.jpg)
