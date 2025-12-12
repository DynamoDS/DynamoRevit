## 詳細
このノードは、選択された参照面の実際の Revit 要素参照を抽出します。これは、Revit 内のジオメトリまたは寸法のホスト参照としてその平面を使用する必要がある場合に便利です。

例:
このグラフでは、座標を使用して 2 つの点を定義し、ReferencePlane.ByStartPointEndPoint を使用してこの 2 点の間に参照面を作成します。次に、この参照面を ReferencePlane.ElementPlaneReference に接続し、面の Revit ネイティブの参照を出力して、ホストや位置合わせのタスクに使用できるようにします。
___
## サンプル ファイル

![ReferencePlane.ElementPlaneReference](./Revit.Elements.ReferencePlane.ElementPlaneReference_img.jpg)
