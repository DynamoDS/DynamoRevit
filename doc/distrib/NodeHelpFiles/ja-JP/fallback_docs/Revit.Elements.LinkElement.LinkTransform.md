## 詳細
このノードは、ホスト モデル内の Revit リンク要素に適用された変換行列を取得します。
つまり、リンクされた要素の座標系をホスト Revit プロジェクトの座標系にマッピングする位置、回転、スケール変換を返します。
これは、リンク モデル間でジオメトリの位置合わせ、解析、操作を行う必要がある場合に便利です。

この例では、レベル L3 でリンクされた Revit 要素がすべて選択され、LinkElement.LinkTransform に入力されます。出力は、リンクされた要素の位置、回転、およびスケール変換です。
___
## サンプル ファイル

![LinkElement.LinkTransform](./Revit.Elements.LinkElement.LinkTransform_img.jpg)
