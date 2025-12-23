## 詳細
このノードは、指定された基準点と方向から、Revit リンク モデルに光線を照射し、次に、リンクされた要素からの連続するバウンスをトレースします。各バウンスは、定義された最大反射数までの、光線がリンク モデル内のジオメトリと交差する点を表します。

この例では、リンクされた要素が選択され、その要素の位置が LinkElement.ByRayBounce への基準点入力として、方向、maxBounces、ビューとともに使用されます。出力は、点とリンクされた要素です。
___
## サンプル ファイル

![LinkElement.ByRayBounce](./Revit.Elements.LinkElement.ByRayBounce_img.jpg)
