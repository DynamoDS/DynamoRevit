## 詳細
`ElementCurveReference.ByCurve` は、Dynamo の曲線から Revit の要素曲線参照を作成します。曲線参照は、Revit の別ノードで単なる Dynamo のジオメトリでなく、選択可能または参照可能な曲線を必要とする場合に役立ちます。これは通常、モデル ジオメトリを参照する必要がある寸法、位置合わせ、拘束、その他の要素の作成など、Revit の参照を必要とするワークフローで使用されます。

次の例では、曲線を選択し、`ElementCurveReference.ByCurve` の入力として使用しています。出力は `ElementCurveReference` であり、曲線ベースの参照を必要とする下流ノードで Revit の曲線参照として使用されます。
___
## サンプル ファイル

![ElementCurveReference.ByCurve](./Revit.GeometryReferences.ElementCurveReference.ByCurve_img.jpg)
