## Podrobnosti
Uzel `Parameter.GroupType` vrací objekt GroupType daného parametru.

V aplikaci Revit jsou skupiny parametrů pro nesdílené parametry předdefinovány, zatímco sdílené parametry umožňují definovat vlastní skupiny. Další informace o práci se sdílenými parametry a skupinami naleznete v tomto článku nápovědy: [Tvorba souborů, skupin a parametrů sdílených parametrů](https://help.autodesk.com/view/RVT/2025/CSY/?guid=GUID-94EA2B8E-2C00-4D29-8D5A-C7C6664DE9CE)

Pokud není nalezena žádný objekt GroupType, je vrácena hodnota null.

V následujícím příkladu jsou vráceny všechny parametry pro informace o projektu aktuálního dokumentu. Vrátí se také objekty GroupType.
___
## Vzorový soubor

![Parameter.GroupType](./Revit.Elements.Parameter.GroupType_img.jpg)
