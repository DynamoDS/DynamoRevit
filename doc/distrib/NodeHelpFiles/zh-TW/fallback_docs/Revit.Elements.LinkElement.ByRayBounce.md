## 深入資訊
此節點會將光線從指定的原點和方向投射到連結的 Revit 模型中，然後追蹤從連結元素上連續反彈的情況。每次反彈都表示光線與連結模型中幾何圖形相交的一個點，最多達到定義的最大反射次數。

在此範例中，選取連結的元素，並使用該元素的位置作為 LinkElement.ByRayBounce 的原點輸入，同時提供方向、maxBounces 和視圖。輸出是點和連結的元素。
___
## 範例檔案

![LinkElement.ByRayBounce](./Revit.Elements.LinkElement.ByRayBounce_img.jpg)
