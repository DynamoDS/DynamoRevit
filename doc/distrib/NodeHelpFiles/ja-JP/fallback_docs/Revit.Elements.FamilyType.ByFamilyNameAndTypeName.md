## 詳細
`Revit.Elements.FamilyType.ByFamilyAndName` と同様に、`Revit.Elements.FamilyType.ByFamilyNameAndTypeName` は、現在のドキュメントからファミリ タイプの定義を返します(存在する場合)。これは `Revit.Elements.FamilyType.ByFamilyAndName` に似ています。ただし、このノードは、ファミリ定義を使用する代わりに、両方の値の文字列入力に依存します。ファミリ タイプが現在のドキュメントで使用できない場合は、null 値が返されます。

次の例では、ファミリ「ドア-通路-片開き-フラッシュ」のドア ファミリ タイプ「36" x 84」が返されます。
___
## サンプル ファイル

![FamilyType.ByFamilyNameAndTypeName](./Revit.Elements.FamilyType.ByFamilyNameAndTypeName_img.jpg)
