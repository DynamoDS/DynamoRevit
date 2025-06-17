## Informacje szczegółowe
Węzeł `Space.CoreBoundary` zwraca listę zagnieżdżoną reprezentującą granicę warstwy nośnej danej przestrzeni. Na zwróconej liście pierwsza lista podrzędna reprezentuje krzywe najbardziej zewnętrzne, a kolejne listy reprezentują pętle w przestrzeni. Granice warstwy nośnej znajdują się w warstwie wewnętrznej lub zewnętrznej warstwy nośnej znajdującej się najbliżej danego pomieszczenia. Więcej informacji na temat linii lokalizacji ścian można znaleźć w pomocy w tym [artykule](https://help.autodesk.com/view/RVT/2024/PLK/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89).

W razie podania nieograniczonej lub nieumieszczonej przestrzeni zwracana jest wartość null.

W poniższym przykładzie pobierane są wszystkie przestrzenie z bieżącego dokumentu i wybranego widoku. Następnie zwracane są granice warstwy nośnej.

___
## Plik przykładowy

![Space.CoreBoundary](./Revit.Elements.Space.CoreBoundary_img.jpg)
