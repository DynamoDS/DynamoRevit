## Podrobnosti
Uzel `FamilyType.ByName` se pokusí načíst daný typ rodiny s daným názvem z aktuálního dokumentu. Pokud není typ rodiny v aktuálním dokumentu dostupný, bude vrácena hodnota null.

Poznámka: Uzel `FamilyType.ByName` prohledává definice typů rodin v pořadí, v jakém byly vytvořeny nadřazené rodiny (podle id prvku). Pokud má více nadřazených rodin definici typu se stejným názvem, je vrácena první nalezená definice. Za účelem výstiženějšího vyhledávání typů rodin použijte uzel `FamilyType.ByFamilyAndName` nebo uzel `FamilyType.ByFamilyNameAndTypeName`

V níže uvedeném příkladu je vrácen typ rodiny dveří „36" x 84“ z rodiny „Dveře, průchod, jednokřídlé, hladké“.
___
## Vzorový soubor

![FamilyType.ByName](./Revit.Elements.FamilyType.ByName_img.jpg)
