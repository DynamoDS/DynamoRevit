## 詳細
このノードは、指定された Revit リンク要素の逆変換行列を取得します。Revit では、リンク モデルを変換(移動、回転、スケール)を使用して配置することができます。このノードを使用すると、その変換を逆にすることができ、リンク モデルの空間からホスト Revit モデルの座標系に座標を効果的に変換して戻すことができます。

この例では、レベル L3 でリンクされた Revit 要素がすべて選択され、LinkElement.LinkInverseTransform に入力されます。出力はホスト Revit モデルの座標系です。
___
## サンプル ファイル

![LinkElement.LinkInverseTransform](./Revit.Elements.LinkElement.LinkInverseTransform_img.jpg)
