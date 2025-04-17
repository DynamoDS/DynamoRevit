## Informacje szczegółowe
Podobnie jak w przypadku węzła `Revit.Elements.FamilyType.ByFamilyAndName`, węzeł `Revit.Elements.FamilyType.ByFamilyNameAndTypeName` zwraca definicję typu rodziny z bieżącego dokumentu (jeśli jest dostępna). To działanie podobne do działania węzła `Revit.Elements.FamilyType.ByFamilyAndName`. Jednak zamiast używać definicji rodziny, ten węzeł opiera się na wejściu w postaci ciągu (string) w przypadku obu wartości. Jeśli typ rodziny nie jest dostępny w bieżącym dokumencie, zwracana jest wartość null.

W poniższym przykładzie zwracany jest typ rodziny drzwi, 36" x 84" z rodziny „Door-Passage-Single-Flush” (Drzwi-korytarz-jednoskrzydłowe-wpuszczane).
___
## Plik przykładowy

![FamilyType.ByFamilyNameAndTypeName](./Revit.Elements.FamilyType.ByFamilyNameAndTypeName_img.jpg)
