## 詳細
`Transaction.Start` は、現在の Revit ドキュメントでトランザクションを開始します。トランザクションは、Revit モデルに加えられた変更をグループ化し、それらをまとめてコミットまたはロールバックできるようにするために使用されます。`Transaction.Start` は通常、Revit ドキュメントの変更開始と終了を Dynamo ワークフローで明示的にコントロールする必要がある場合は、`Transaction.End` とともに使用されます。

次の例では、レベルを作成し、`Transaction.Start` への入力として使用しています。これにより、モデルが変更される前に Revit のトランザクションが開始されます。必要な Revit の操作が完了すると、`Transaction.End` はトランザクションを閉じ、ドキュメントに変更をコミットします。出力は、他のトランザクション関連ノードに渡すことができるトランザクション オブジェクトです。
___
## サンプル ファイル

![Transaction.Start](./Revit.Transaction.Transaction.Start_img.jpg)
