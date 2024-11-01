## 詳細
Revit のすべてのビューには基準点があります。`View.Origin` はこの値を Dynamo における点として返します。Revit API のドキュメントによると、「平面図ビューの基準点は意味がない」とされています。エンド ユーザはこれを念頭に置いて、`View.Origin` で提供される値について考慮してください。

次の例では、アクティブなビューと選択された 3D ビューの基準点を返します。
___
## サンプル ファイル

![View.Origin](./Revit.Elements.Views.View.Origin_img.jpg)
