## Podrobnosti
Uzel `SpecType.ByTypeId` převede řetězec Spec TypeId aplikace Revit na objekt `SpecType`. Typy specifikací definují druh dat, která parametr v aplikaci Revit představuje, například délka, plocha, úhel, materiál, síla, text, celé číslo a mnoho dalších. Tato funkce se běžně používá při tvorbě sdílených parametrů, parametrů projektů nebo definic parametrů v novějších verzích aplikace Revit, kde společnost Autodesk nahradila starší systém „ParameterType“ systémem Forge TypeIds.

V níže uvedeném příkladu je předán řetězec představující položku Revit Spec TypeId uzlu `SpecType.ByTypeId`. Výstupem je objekt `SpecType`, který lze použít následně při vytváření nové definice parametru, aby aplikace Revit pochopila, jaký typ dat by měl parametr ukládat.
___
## Vzorový soubor

![SpecType.ByTypeId](./Revit.Elements.SpecType.ByTypeId_img.jpg)
