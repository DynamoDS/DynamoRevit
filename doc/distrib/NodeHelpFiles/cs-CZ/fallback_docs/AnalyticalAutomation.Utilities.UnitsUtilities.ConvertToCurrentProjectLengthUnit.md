## Podrobnosti
Tento uzel přijímá číselnou hodnotu délky a identifikátor typu jednotky a převádí vstupní hodnotu na jednotky délky aktivního projektu aplikace Revit. Výstupem je hodnota double představující převedený výsledek.

V tomto příkladu číselný posuvník poskytuje hodnotu délky a jednotka (například Meters) je vybrána pro získání řetězce Unit.TypeId. Obě jsou připojeny k uzlu UnitsUtilities.ConvertToCurrentProjectLengthUnit, který vrací převedenou hodnotu délky na základě nastavení jednotek projektu.
___
## Vzorový soubor

![UnitsUtilities.ConvertToCurrentProjectLengthUnit](./AnalyticalAutomation.Utilities.UnitsUtilities.ConvertToCurrentProjectLengthUnit_img.jpg)
