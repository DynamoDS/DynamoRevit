## 詳細
`Parameter.GroupType` は、指定されたパラメータの GroupType を返します。

Revit では、非共有パラメータのパラメータ グループは事前に設定されていますが、共有パラメータではカスタム グループを定義できます。共有パラメータとグループの使用の詳細については、こちらのヘルプ記事「共有パラメータのファイル、グループ、パラメータを作成する」(https://help.autodesk.com/view/RVT/2025/JPN/?guid=GUID-94EA2B8E-2C00-4D29-8D5A-C7C6664DE9CE)を参照してください。

GroupType が見つからない場合は、null 値が返されます。

次の例では、現在のドキュメント内のプロジェクト情報のすべてのパラメータが返されます。GroupType も返されます。
___
## サンプル ファイル

![Parameter.GroupType](./Revit.Elements.Parameter.GroupType_img.jpg)
