## Informacje szczegółowe
Węzeł `Space.CoreCenterBoundary` zwraca listę zagnieżdżoną reprezentującą środek granicy warstwy nośnej danej przestrzeni. Na zwróconej liście pierwsza lista podrzędna reprezentuje krzywe najbardziej zewnętrzne, a kolejne listy reprezentują pętle w przestrzeni. Środki granic warstwy nośnej znajdują się w środku ścian zdefiniowanych jako warstwy nośne. Więcej informacji na temat linii lokalizacji ścian można znaleźć w pomocy w tym [artykule](https://help.autodesk.com/view/RVT/2024/PLK/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89).

W razie podania nieograniczonej lub nieumieszczonej przestrzeni zwracana jest wartość null.

W poniższym przykładzie pobierane są wszystkie przestrzenie z bieżącego dokumentu i wybranego widoku. Następnie zwracane są środki granic warstwy nośnej.

___
## Plik przykładowy

![Space.CoreCenterBoundary](./Revit.Elements.Space.CoreCenterBoundary_img.jpg)
