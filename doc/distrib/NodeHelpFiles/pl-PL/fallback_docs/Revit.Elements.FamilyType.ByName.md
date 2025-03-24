## Informacje szczegółowe
Węzeł `FamilyType.ByName` próbuje pobrać dany typ rodziny o danej nazwie z bieżącego dokumentu. Jeśli typ rodziny nie jest dostępny w bieżącym dokumencie, zwracana jest wartość null.

Uwaga: węzeł `FamilyType.ByName` wyszukuje definicje typów rodzin w kolejności utworzenia rodziny nadrzędnej (według identyfikatora elementu). Jeśli wiele rodzin nadrzędnych ma definicję typu o tej samej nazwie, zwracana jest pierwsza znaleziona. Aby skorzystać z bardziej zwięzłego wyszukiwania typów rodzin, użyj węzła `FamilyType.ByFamilyAndName` lub `FamilyType.ByFamilyNameAndTypeName`

W poniższym przykładzie zwracany jest typ rodziny drzwi, 36" x 84" z rodziny „Door-Passage-Single-Flush” (Drzwi-korytarz-jednoskrzydłowe-wpuszczane).
___
## Plik przykładowy

![FamilyType.ByName](./Revit.Elements.FamilyType.ByName_img.jpg)
