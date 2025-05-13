## 詳細
`Transaction.End` は現在の Dynamo トランザクションを完了し、指定された要素を返します。Revit では、トランザクションは Revit ドキュメントに加えられた変更内容を表します。変更が発生すると、有効になっている[元に戻す]ボタンで確認できます。`Transaction.End` を使用すると、Dynamo グラフにステップを追加し、`Transaction.End` が使用されているステップごとに[元に戻す]アクションを作成できます。

次の例では、ファミリ インスタンスが Revit ドキュメントに配置されます。`Transaction.End` は、`FamilyInstance.SetRotation` でファミリ インスタンスを回転する前に配置を完了するために呼び出されます。

___
## サンプル ファイル

![Transaction.End](./Revit.Transaction.Transaction.End_img.jpg)
