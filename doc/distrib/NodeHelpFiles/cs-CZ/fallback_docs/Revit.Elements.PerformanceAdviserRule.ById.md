## Podrobnosti
Tento uzel načte konkrétní pravidlo nástroje Poradce pro výkon z aplikace Revit podle jeho jedinečného ID pravidla.

V tomto příkladu je vybráno pravidlo „Projekt obsahuje nepoužívané rodiny a typy“ a extrahuje se jeho ID pravidla. Toto ID se převede na řetězec a pak se předá zpět do uzlu PerformanceAdviserRule.ById, který znovu identifikuje a načte stejné pravidlo pro další použití nebo automatizaci.
___
## Vzorový soubor

![PerformanceAdviserRule.ById](./Revit.Elements.PerformanceAdviserRule.ById_img.jpg)
