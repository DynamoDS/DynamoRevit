## 深入資訊
此節點透過指定標識索引，從既有的 ElevationMarker 建立立面視圖。Revit 中的每個 ElevationMarker 最多可以容納四個單獨的立面視圖 — 每個方向 (東、西、南、北) 各一個。此節點可讓您透過參考標識和所需的索引編號，產生其中一個方向的立面。

在此範例中，建立一個立面標識作為節點 ElevationMarker.CreateElevationByMarkerIndex 的 elevationMarker 輸入，同時提供視圖和索引 (0,1,2,3)。

___
## 範例檔案

![ElevationMarker.CreateElevationByMarkerIndex](./Revit.Elements.ElevationMarker.CreateElevationByMarkerIndex_img.jpg)
