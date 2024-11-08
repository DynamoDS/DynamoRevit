## 詳細
`Element.GetMaterials` は、Revit 要素内に存在するすべてのマテリアル_(およびその ID)_を返します。複数のマテリアルを持つ要素は、各要素のリストを返します。入力の「paintMaterials」は、ユーザによってペイントされたマテリアルも収集するようにノードに指示するブール値の切り替えです。

次の例では、現在のドキュメント(ファイル)内のすべての壁インスタンスのマテリアルが返されます。
___
## サンプル ファイル

![Element.GetMaterials](./Revit.Elements.Element.GetMaterials_img.jpg)
