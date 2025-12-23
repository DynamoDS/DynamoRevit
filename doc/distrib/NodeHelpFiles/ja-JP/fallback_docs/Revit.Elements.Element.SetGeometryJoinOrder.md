## 詳細
このノードは、既に幾何学的に結合されている 2 つの Revit 要素間の結合順序を変更します。このノードを使用すると、どの要素のジオメトリを「切り取る」かや、どの要素を他よりも優先させるかをユーザが決定することができます。

この例では、結合された 2 つの壁が選択され、Element.SetGeometryJoinOrder ノードへの入力(cuttingElement と otherElement)として使用されます。出力は、割り当てられた結合順序です。

___
## サンプル ファイル

![Element.SetGeometryJoinOrder](./Revit.Elements.Element.SetGeometryJoinOrder_img.jpg)
