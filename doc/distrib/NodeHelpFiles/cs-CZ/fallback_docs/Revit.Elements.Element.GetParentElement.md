## Podrobnosti
Při shromažďování prvků v aplikaci Revit pomocí kolektoru kategorií je možné shromáždit sdílené vnořené rodiny. Pomocí uzlu `Element.GetParentElement` je možné určit, zda je daná instance rodiny vnořena, a to identifikací jejího nadřazeného prvku.

V níže uvedeném příkladu jsou všechny instance rodin „Křeslo-Breuer“ seskupeny podle instance nadřazené rodiny.
___
## Vzorový soubor

![Element.GetParentElement](./Revit.Elements.Element.GetParentElement_img.jpg)
