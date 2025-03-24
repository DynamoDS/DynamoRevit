## 詳細
`Element.GetLocation`は、入力された各 Revit 要素の位置を表す Dynamo のジオメトリ オブジェクトを返します。要素の位置は通常、その挿入点によって決まります。たとえば、円柱基礎の杭の位置は上部中心にあり、壁の位置は通常、曲線または線分です。

次の例では、現在のドキュメント(ファイル)内のすべての家具インスタンス_(共有ファミリを含む)_の点の位置が返されます。
___
## サンプル ファイル

![Element.GetLocation](./Revit.Elements.Element.GetLocation_img.jpg)
