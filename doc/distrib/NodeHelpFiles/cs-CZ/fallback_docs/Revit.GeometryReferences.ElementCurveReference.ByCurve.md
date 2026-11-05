## Podrobnosti
Uzel `ElementCurveReference.ByCurve` vytvoří referenci křivky prvku aplikace Revit z křivky aplikace Dynamo. Reference křivky je užitečná, pokud jiný uzel aplikace Revit potřebuje vybrat nebo odkazovat na křivku, nikoli pouze geometrii aplikace Dynamo. Tato možnost se obvykle používá u pracovních postupů, které vyžadují reference Revit, například při vytváření kót, zarovnání, vazeb nebo jiných prvků, které potřebují odkazovat na geometrii modelu.

V následujícím příkladu je vybrána křivka a použije se jako vstup pro uzel `ElementCurveReference.ByCurve`. Výstupem je uzel `ElementCurveReference`, který se pak použije jako reference křivky aplikace Revit pro následné uzly, které vyžadují reference založené na křivce.
___
## Vzorový soubor

![ElementCurveReference.ByCurve](./Revit.GeometryReferences.ElementCurveReference.ByCurve_img.jpg)
