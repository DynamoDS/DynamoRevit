## Informacje szczegółowe
Węzeł `Room.CenterBoundary` zwraca listę zagnieżdżoną reprezentującą obwiednię linii środkowej danego pomieszczenia. Na zwróconej liście pierwsza lista podrzędna reprezentuje krzywe najbardziej zewnętrzne, a kolejne listy reprezentują pętle w pomieszczeniu. Obwiednie środkowe znajdują się na linii środkowej ściany we wszystkich warstwach pomieszczenia programu Revit. Więcej informacji na temat linii lokalizacji ścian można znaleźć w pomocy w tym [artykule](https://help.autodesk.com/view/RVT/2024/PLK/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89).

W razie podania nieograniczonego lub nieumieszczonego pomieszczenia zwracana jest wartość null.

W poniższym przykładzie pobierane są wszystkie pomieszczenia z bieżącego dokumentu i wybranego widoku. Następnie zwracane są obwiednie środkowe.
___
## Plik przykładowy

![Room.CenterBoundary](./Revit.Elements.Room.CenterBoundary_img.jpg)
