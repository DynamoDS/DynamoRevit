## Podrobnosti
Uzel `GroupType.ByTypeId` se primárně používá v pracovních postupech vytváření a správy parametrů, kde je nutné zadat skupinu parametrů aplikace Revit programově. Vstup je řetězec představující ID typu skupiny parametrů aplikace Revit. Výstupem je objekt `GroupType`, který lze použít v podřízených uzlech, které vyžadují klasifikaci seskupení parametrů.

V níže uvedeném příkladu je vytvořeno několik řetězců představujících ID typu skupiny parametrů aplikace Revit a zkombinováno do seznamu. Seznam se poté použije jako vstup pro uzel `GroupType.ByTypeId`. Výstupem je seznam objektů `GroupType` aplikace Revit představujících odpovídající skupiny parametrů.
___
## Vzorový soubor

![GroupType.ByTypeId](./Revit.Elements.GroupType.ByTypeId_img.jpg)
