## 詳細
`Dimension.ByReferences` は、指定された参照を使用して、選択したビューの入力線分に沿って寸法を配置します。出力は Revit 寸法要素です。

次の例では、複数の点を作成して Line ノードで接続し、Revit でモデル曲線を生成しています。その後、モデル曲線は Revit 曲線参照に変換されます。これらの参照はリスト化され、`Dimension.ByReferences` の `references` 入力として使用されます。さらに、追加の線分を作成して寸法の配置と方向を定義しています。アクティブな Revit ビューも、`view` の入力として与えています。
___
## サンプル ファイル

![Dimension.ByReferences](./Revit.Elements.Dimension.ByReferences_img.jpg)
