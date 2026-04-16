## 詳細
このノードは、モデル曲線や詳細線分など、指定された Revit CurveElement に関連付けられた曲線参照を取得します。この参照は、寸法記入、位置合わせ、分割されたパスの作成など、ジオメトリ参照を必要とする他のノードへの入力として使用することができます。

この例では、始点と終点を使用してモデル曲線が作成され、次に、CurveElement.ElementCurveReference ノードに入力されます。出力は、下流の操作で使用することができる曲線要素のジオメトリ参照です。
___
## サンプル ファイル

![CurveElement.ElementCurveReference](./Revit.Elements.CurveElement.ElementCurveReference_img.jpg)
