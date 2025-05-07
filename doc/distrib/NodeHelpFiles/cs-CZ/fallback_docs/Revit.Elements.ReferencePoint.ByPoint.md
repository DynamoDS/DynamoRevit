## Podrobnosti
Uzel `ReferencePoint.ByPoint` vytvoří v aktivním dokumentu rodiny aplikace Revit prvek referenčního bodu v daném umístění bodu. Poznámka: Dokument rodiny musí být adaptivní komponentou nebo rodinou objemů. Tento uzel se liší od uzlu „ReferencePoint.ByCoordinates“, protože jako umístění používá bod aplikace Dynamo. Koncový uživatel může díky tomu upravovat bod aplikace Dynamo pomocí přímé manipulace, což má za následek také aktualizaci referenčního bodu. Další informace o přímé manipulaci nabízí tento [odkaz](https://primer2.dynamobim.org/10_sample_workflow/10-1_getting-started-workflows/2-attractor-points#adjusting-with-direct-manipulation).

V níže uvedeném příkladu je vytvořen nový referenční bod na souřadnicích 0,0,1.
___
## Vzorový soubor

![ReferencePoint.ByPoint](./Revit.Elements.ReferencePoint.ByPoint_img.jpg)
