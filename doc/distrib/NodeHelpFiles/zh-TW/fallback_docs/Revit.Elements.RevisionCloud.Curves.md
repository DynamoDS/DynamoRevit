## 深入資訊
此節點會擷取組成修訂雲形可見周長的曲線迴路 (通常為弧和直線)。雲形的每一段都以一個曲線物件 (通常為弧) 表示，對應到視圖或圖紙中修訂標記的「泡泡」形狀。

在此範例中，使用數字滑棒定義尺寸建立一個矩形，然後將其分解成曲線並根據方位反轉。使用這些曲線以及選取的視圖 (Site Plan) 和修訂 (Seq. 2 – Not For Construction) 透過 RevisionCloud.ByCurve 節點產生修訂雲形。然後將建立的修訂雲形連接到 RevisionCloud.Curves 節點，擷取並顯示該雲形的定義曲線。這可協助使用者確認修訂雲形的幾何圖形，可以靈活性地重複使用或進一步自動化。
___
## 範例檔案

![RevisionCloud.Curves](./Revit.Elements.RevisionCloud.Curves_img.jpg)
