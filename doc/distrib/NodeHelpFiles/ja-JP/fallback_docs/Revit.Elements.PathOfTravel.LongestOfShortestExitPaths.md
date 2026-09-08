## 詳細
`PathOfTravel.LongestOfShortestExitPaths` は、複数の点から 1 つまたは複数の出口地点までの最短ルートの中で、最も長いルートを表す移動経路を作成します。各始点は使用可能な出口に対して検証され、Revit はそれぞれについて最も近い有効な出口経路を決定します。このノードは、こうした最短経路の結果の中で移動距離が最も長い経路を返します。

次の例では、移動経路の計算に必要な Revit ビューを用意するために、まずレベルと平面図ビューを作成しています。`Point.ByCoordinates` を使用して複数の点を作成し、出口候補の位置を示すリストとしてまとめています。`PathOfTravel.LongestOfShortestExitPaths` は、平面図ビューと目的地点のリストを使用して移動経路を計算し、移動距離が最も長い経路を返します。
___
## サンプル ファイル

![PathOfTravel.LongestOfShortestExitPaths](./Revit.Elements.PathOfTravel.LongestOfShortestExitPaths_img.jpg)
