## 詳細
`ScheduleFilter.FilterType` は、入力フィルタに使用するメソッドを返します。
次のフィルタ タイプが使用可能です。

- Equal - フィールド値が指定された値と等しい。
- NotEqual - フィールド値が指定された値と等しくない。
- GreaterThan - フィールド値が指定された値より大きい。
- GreaterThanOrEqual - フィールド値が指定された値以上。
- LessThan - フィールド値が指定された値より小さい。
- LessThanOrEqual - フィールド値が指定された値以下。
- Contains - 文字列フィールドの場合、フィールド値に指定された文字列が含まれる。
- NotContains - 文字列フィールドの場合、フィールド値に指定された文字列が含まれない。
- BeginsWith - 文字列フィールドの場合、フィールド値が指定された文字列で始まる。
- NotBeginsWith - 文字列フィールドの場合、フィールド値が指定された文字列で始まらない。
- EndsWith - 文字列フィールドの場合、フィールド値が指定された文字列で終わる。
- NotEndsWith - 文字列フィールドの場合、フィールド値が指定された文字列で終わらない。
- IsAssociatedWithGlobalParameter - フィールドが、互換性のあるタイプの指定されたグローバル パラメータに関連付けられている
- IsNotAssociatedWithGlobalParameter - フィールドが、互換性のあるタイプの指定されたグローバル パラメータに関連付けられていない

次の例では、現在の Revit ファイルの最初の集計表を収集します。次に、フィルタで集計表ビューをチェックし、適用されるフィルタは「文字列が次で終わらない」フィルタ タイプのみです。
___
## サンプル ファイル

![ScheduleFilter.FilterType](./Revit.Schedules.ScheduleFilter.FilterType_img.jpg)
