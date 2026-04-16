## 詳細
Dynamo の ReferencePlane.ByLine ノードは、定義された線分を基準として使用して Revit 参照面を作成します。これを使用すると、特定の位置と方向にカスタム参照面を生成することができます。

この例では、Point.ByCoordinates と調整可能なスライダを使用して、2 つの点を定義します。次に、これら 2 点の間に Line.ByStartPointEndPoint が作成され、最後に ReferencePlane.ByLine ノードによって、この線分に沿って参照面が生成されます。
___
## サンプル ファイル

![ReferencePlane.ByLine](./Revit.Elements.ReferencePlane.ByLine_img.jpg)
