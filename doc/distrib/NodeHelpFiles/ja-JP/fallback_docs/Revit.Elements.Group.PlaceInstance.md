## 詳細
位置(点)と groupType を入力として使用して Revit グループを配置します。groupType の Object.Type 出力は Revit.Elements.ElementType を読み取ります。

次の例では、アクティブな Revit ドキュメントからすべてのモデル グループが収集されます。グループのタイプは Group.GroupType で返され、プロジェクト リストの最初のグループに縮小され、GroupType の入力として使用されます。位置の入力は 100,100 に配置されます。
___
## サンプル ファイル

![Group.PlaceInstance](./Revit.Elements.Group.PlaceInstance_img.jpg)
