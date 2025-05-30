## 詳細
`Space.IsPointInsideSpace` は、指定された点が指定されたスペースの内側にあるかどうかを確認します。これは、Revit 内の要素にマーク値を割り当てる際に便利です。

次の例では、現在の Revit ドキュメント内の指定されたビュー内のすべての制気口が収集されます。次に、これらの点の位置が `Space.IsPointInsideSpace` を使用して指定されたビューのスペースと比較されます。リスト管理を使用して、スペース内で発生する制気口を除外するためのサブリストが作成されます。List@Level の使用に関する詳細については、こちら[記事](https://primer2.dynamobim.org/ja/5_essential_nodes_and_concepts/5-4_designing-with-lists/3-lists-of-lists)をご覧ください。
___
## サンプル ファイル

![Space.IsPointInsideSpace](./Revit.Elements.Space.IsPointInsideSpace_img.jpg)
