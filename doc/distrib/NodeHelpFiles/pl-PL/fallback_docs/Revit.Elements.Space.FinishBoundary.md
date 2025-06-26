## Informacje szczegółowe
Węzeł `Space.FinishBoundary` zwraca listę zagnieżdżoną reprezentującą obwiednię wykończenia danej przestrzeni. Na zwróconej liście pierwsza lista podrzędna reprezentuje krzywe najbardziej zewnętrzne, a kolejne listy reprezentują pętle w przestrzeni. Obwiednia przestrzeni zwracana przez ten węzeł znajduje się na powierzchni wykończenia wewnątrz przestrzeni programu Revit. Więcej informacji na temat linii lokalizacji ścian można znaleźć w pomocy w tym [artykule](https://help.autodesk.com/view/RVT/2024/PLK/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89).

W razie podania nieograniczonej lub nieumieszczonej przestrzeni zwracana jest wartość null.

W poniższym przykładzie pobierane są wszystkie przestrzenie z bieżącego dokumentu i wybranego widoku. Następnie zwracane są obwiednie wykończenia.

___
## Plik przykładowy

![Space.FinishBoundary](./Revit.Elements.Space.FinishBoundary_img.jpg)
