## In profondità
Simile a `Revit.Elements.FamilyType.ByFamilyAndName`, `Revit.Elements.FamilyType.ByFamilyNameAndTypeName` restituisce la definizione del tipo di famiglia dal documento corrente (se disponibile). È simile a `Revit.Elements.FamilyType.ByFamilyAndName`. Tuttavia, invece di utilizzare una definizione di famiglia, questo nodo si basa sull'input di stringa per entrambi i valori. Se il tipo di famiglia non è disponibile nel documento corrente, viene restituito un valore null.

Nell'esempio seguente, viene restituito un tipo di famiglia di porte, "36" x 84", della famiglia "Door-Passage-Single-Flush".
___
## File di esempio

![FamilyType.ByFamilyNameAndTypeName](./Revit.Elements.FamilyType.ByFamilyNameAndTypeName_img.jpg)
