## Podrobnosti
Uzel `ElementFaceReference.BySurface` vytvoří referenci plochy aplikace Revit z vybraného nebo generovaného povrchu. Převede povrch aplikace Dynamo, který je asociován s geometrií aplikace Revit, na objekt `ElementFaceReference`. Tuto referenci pak mohou použít jiné uzly, které potřebují referenci plochy aplikace Revit, nikoli pouhou geometrii povrchu.

V níže uvedeném příkladu se vytvoří stěna a povrch z této stěny se použije jako vstup do uzlu `ElementFaceReference.BySurface`. Výstupem je vlastnost `ElementFaceReference`, kterou mohou používat uzly, které vyžadují referenci na plochu prvku aplikace Revit.
___
## Vzorový soubor

![ElementFaceReference.BySurface](./Revit.GeometryReferences.ElementFaceReference.BySurface_img.jpg)
