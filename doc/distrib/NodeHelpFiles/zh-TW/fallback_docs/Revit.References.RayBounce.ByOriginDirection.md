## 深入資訊
此節點會在 Revit 模型內執行光線反彈分析。從給定的原點開始，沿著指定方向向量行進，追蹤當光線與模型中的元素相交時的路徑。當光線到達某個表面時，可以繼續從該表面反彈，取決於允許的反射次數、模擬光線、可見性或路徑反射行為。

在此範例中，選取元素並使用其位置作為 RayBounce.ByOriginDirection 節點的輸入原點，同時提供方向、maxBounces 和視圖。
___
## 範例檔案

![RayBounce.ByOriginDirection](./Revit.References.RayBounce.ByOriginDirection_img.jpg)
