## 詳細
ScheduleView の指定した入力から、定義されたすべてのフィルタを削除します。

この例では、ScheduleView.ScheduleFilters に直接ルーティングされる集計表ビューを最初に定義して、既存のビュー フィルタを明示的に表示しました。変換を強調するために、次にこの同じビューを 10 ミリ秒一時停止させています。この短い遅延の後、ビューは ScheduleView.ClearAllFilters ノードに進み、適用されたすべてのフィルタが削除されます。最後に、ここで変更されたビューは別の ScheduleView.ScheduleFilters ノードに戻され、フィルタ リストが空のリストになったことを明確に示し、それによって元のすべてのフィルタが正常にクリアされたことが確認されます。
___
## サンプル ファイル

![ScheduleView.ClearAllFilters](./Revit.Elements.Views.ScheduleView.ClearAllFilters_img.jpg)
