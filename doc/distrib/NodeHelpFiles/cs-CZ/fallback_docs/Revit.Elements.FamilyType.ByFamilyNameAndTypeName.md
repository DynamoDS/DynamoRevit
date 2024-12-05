## Podrobnosti
Podobně jako uzel `Revit.Elements.FamilyType.ByFamilyAndName` vrací uzel `Revit.Elements.FamilyType.ByFamilyNameAndTypeName` definici typu rodiny z aktuálního dokumentu (pokud je k dispozici). Tato akce je podobná jako u uzlu `Revit.Elements.FamilyType.ByFamilyAndName`. Místo použití definice rodiny však tento uzel spoléhá na řetězcový vstup u obou hodnot. Pokud není typ rodiny v aktuálním dokumentu dostupný, bude vrácena hodnota null.

V níže uvedeném příkladu je vrácen typ rodiny dveří "36" x 84" z rodiny „Dveře, průchod, jednokřídlé, hladké“.
___
## Vzorový soubor

![FamilyType.ByFamilyNameAndTypeName](./Revit.Elements.FamilyType.ByFamilyNameAndTypeName_img.jpg)
