## 詳細
`ReferencePoint.ByPoint` は、指定された点の位置にアクティブな Revit ファミリ ドキュメントの参照点要素を作成します。注: ファミリ ドキュメントは、アダプティブ コンポーネントまたはマス ファミリである必要があります。このノードは、位置に Dynamo の点を使用する「ReferencePoint.ByCoordinates」とは異なります。こちらは、Dynamo の点を編集するのにエンド ユーザが直接操作でき、その結果参照点も更新されるため便利です。直接操作の詳細については、こちらの[リンク](https://primer2.dynamobim.org/ja/10_sample_workflow/10-1_getting-started-workflows/2-attractor-points#dewosuru)をご覧ください。

次の例では、座標 0,0,1 に新しい参照点が作成されます。
___
## サンプル ファイル

![ReferencePoint.ByPoint](./Revit.Elements.ReferencePoint.ByPoint_img.jpg)
