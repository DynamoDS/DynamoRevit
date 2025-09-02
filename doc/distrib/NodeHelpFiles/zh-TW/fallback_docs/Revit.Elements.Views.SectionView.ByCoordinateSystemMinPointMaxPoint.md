## 深入資訊
給定具有最小點和最大點的任何 CoordinateSystem，建立剖面視圖。

在此範例中，我們產生其位置和方位與輸入 CoordinateSystem 直接相關的 SectionView。這裡有策略地定位該 CoordinateSystem，以定義一個聚焦在建築右上角的剖面。請務必觀察 minPoint 和 maxPoint 輸入的 Z 分量精確決定所產生 Revit SectionView 的「遠裁剪偏移」參數，進而控制其在模型中的有效視景深度。
如需控制元素顯示的更多資訊，請參閱下面的連結
https://help.autodesk.com/view/RVT/2024/CHT/?guid=GUID-48484D2E-28ED-4BC3-8703-3B0488F1DA56
___
## 範例檔案

![SectionView.ByCoordinateSystemMinPointMaxPoint](./Revit.Elements.Views.SectionView.ByCoordinateSystemMinPointMaxPoint_img.jpg)
