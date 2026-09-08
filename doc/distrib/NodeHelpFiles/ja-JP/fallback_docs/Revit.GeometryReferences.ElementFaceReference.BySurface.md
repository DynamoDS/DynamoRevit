## 詳細
`ElementFaceReference.BySurface` は、選択したサーフェスまたは生成したサーフェスから Revit の面参照を作成します。これは、Revit のジオメトリに関連付けられている Dynamo のサーフェスを `ElementFaceReference` に変換します。この参照は、単なるサーフェス ジオメトリではなく、Revit の面参照を必要とする他のノードで使用できます。

次の例では、壁を作成し、その壁のサーフェスを `ElementFaceReference.BySurface` への入力として使用しています。出力は `ElementFaceReference` であり、Revit 要素の面参照を必要とするノードで使用できます。
___
## サンプル ファイル

![ElementFaceReference.BySurface](./Revit.GeometryReferences.ElementFaceReference.BySurface_img.jpg)
