## 詳細
このノードは、長さの数値と単位タイプ識別子を取得し、入力値をアクティブな Revit プロジェクトの長さの単位に変換します。出力は、変換結果を表す倍精度値です。

この例では、数値スライダで長さの値を指定し、単位(メートルなど)を選択してその Unit.TypeId 文字列を取得します。両方とも UnitsUtilities.ConvertToCurrentProjectLengthUnit ノードに接続され、ノードはプロジェクトの単位設定に基づいて変換した長さの値を返します。
___
## サンプル ファイル

![UnitsUtilities.ConvertToCurrentProjectLengthUnit](./AnalyticalAutomation.Utilities.UnitsUtilities.ConvertToCurrentProjectLengthUnit_img.jpg)
