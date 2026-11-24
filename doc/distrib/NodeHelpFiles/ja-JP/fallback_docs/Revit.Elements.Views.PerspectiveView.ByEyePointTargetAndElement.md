## 詳細
`PerspectiveView.ByEyePointTargetAndElement` は、視点、目標位置、要素、名前、および `isolateElement` のブール値を使用して、Revit の新しいパース 3D ビューを作成します。

次の例では、Revit 要素を選択し、パース ビューのフォーカスとして使用しています。カメラ位置用の点と目標位置用の点をそれぞれ作成します。これらは、選択した要素とともに `PerspectiveView.ByEyePointTargetAndElement` への入力として使用します。出力は、視点から目標位置の方向を向いた新しいパース ビューで、ビュー範囲は選択した要素に基づいています。
___
## サンプル ファイル

![PerspectiveView.ByEyePointTargetAndElement](./Revit.Elements.Views.PerspectiveView.ByEyePointTargetAndElement_img.jpg)
