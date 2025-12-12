## 詳細
Dynamo の Revision.SetIssued ノードを使用すると、Revit で選択した改訂を発行済みとしてマークするか、未発行としてマークするかをコントロールすることができます。このノードは、改訂要素とブール値入力(True/False)を受け取るため、ユーザは Revit で手動で編集しなくても、改訂ステータスを直接コントロールすることができます。
このグラフでは、Select Revision ノードを使用して特定の改訂(例: 「Seq. 1 – Schematic Design」)を選択します。Boolean ノードは True または False の値を提供します。次に、この値が Revision.SetIssued ノードに接続され、改訂の発行ステータスが自動的に更新されます。

___
## サンプル ファイル

![Revision.SetIssued](./Revit.Elements.Revision.SetIssued_img.jpg)
