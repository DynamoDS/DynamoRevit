## 詳細
このノードは、マーカー インデックスを指定して、既存の ElevationMarker から立面図ビューを作成します。Revit の各 ElevationMarker は、方向(北、南、東、西)ごとに 1 つずつ、最大 4 つの個別の立面図ビューをホストすることができます。このノードを使用すると、マーカーと目的のインデックス番号を参照して、これらの方向の立面図の 1 つを生成することができます。

この例では、立面図マーカーが作成され、ElevationMarker.CreateElevationByMarkerIndex ノードに対する入力 elevationMarker として、ビューとインデックス(0,1,2,3)とともに使用されます。

___
## サンプル ファイル

![ElevationMarker.CreateElevationByMarkerIndex](./Revit.Elements.ElevationMarker.CreateElevationByMarkerIndex_img.jpg)
