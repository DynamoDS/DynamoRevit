## 詳細
このノードは、Revit モデル内でレイ バウンス解析を実行します。指定された始点から開始し、指定された方向ベクトルに沿って移動し、レイがモデル内の要素と交差するときの光線のパスを追跡します。光線はサーフェスに当たると、許可されているバウンスの数に応じて、そのサーフェスで継続的に跳ね返り、照明、可視性、またはパス反射の動作がシミュレートされます。

この例では、要素が選択され、その位置が RayBounce.ByOriginDirection ノードへの入力始点として、方向、maxBounces、ビューとともに使用されます。
___
## サンプル ファイル

![RayBounce.ByOriginDirection](./Revit.References.RayBounce.ByOriginDirection_img.jpg)
