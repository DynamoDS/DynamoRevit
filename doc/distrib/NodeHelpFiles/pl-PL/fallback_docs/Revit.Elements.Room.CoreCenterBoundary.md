## Informacje szczegółowe
Węzeł `Room.CoreCenterBoundary` zwraca listę zagnieżdżoną reprezentującą środek granicy warstwy nośnej danego pomieszczenia. Na zwróconej liście pierwsza lista podrzędna reprezentuje krzywe najbardziej zewnętrzne, a kolejne listy reprezentują pętle w pomieszczeniu. Środki granic warstwy nośnej znajdują się w środku ścian zdefiniowanych jako warstwy nośne. Więcej informacji na temat linii lokalizacji ścian można znaleźć w pomocy w tym [artykule](https://help.autodesk.com/view/RVT/2024/PLK/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89).

W razie podania nieograniczonego lub nieumieszczonego pomieszczenia zwracana jest wartość null.

W poniższym przykładzie pobierane są wszystkie pomieszczenia z bieżącego dokumentu i wybranego widoku. Następnie zwracane są środki granic warstwy nośnej.
___
## Plik przykładowy

![Room.CoreCenterBoundary](./Revit.Elements.Room.CoreCenterBoundary_img.jpg)
