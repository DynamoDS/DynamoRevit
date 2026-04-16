## 詳細
Dynamo の Revision.Issued ノードは、Revit の改訂が発行済みとしてマークされているかどうかを確認するために使用されます。true または false の値(ブール値)を返すため、チームは Revit の改訂の設定を開かなくても、改訂のステータスをすばやく確認できます。

このグラフでは、Select Revision ノードを使用してプロジェクトから改訂を選択します。すると、Revision.Issued ノードによって、選択された改訂が発行されているかどうかが確認されて、結果が Watch ノードに true または false として表示されます。これにより、Dynamo から直接、改訂の発行ステータスを確認しやすくなります。

___
## サンプル ファイル

![Revision.Issued](./Revit.Elements.Revision.Issued_img.jpg)
