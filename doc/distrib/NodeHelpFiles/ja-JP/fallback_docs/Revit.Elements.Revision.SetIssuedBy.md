## 詳細
Dynamo の Revision.SetIssuedBy ノードは、Revit の改訂の[発行元]の値を設定または更新するために使用されます。誰が改訂を発行したかを記録することで、改訂管理を自動化し、Revit で手動で編集することなく、ドキュメントを明確で一貫性のあるものにします。

このグラフでは、Select Revision ノードを使用して必要な改訂を選択し、文字列入力(例: ABC)で発行者の名前を指定します。次に、Revision.SetIssuedBy ノードがこの値を選択された改訂に適用し、Revit モデルの[発行元]フィールドを直接更新します。

___
## サンプル ファイル

![Revision.SetIssuedBy](./Revit.Elements.Revision.SetIssuedBy_img.jpg)
