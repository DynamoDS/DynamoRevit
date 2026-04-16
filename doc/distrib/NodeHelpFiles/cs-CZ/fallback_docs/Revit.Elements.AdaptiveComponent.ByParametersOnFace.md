## Podrobnosti
Tento uzel umístí adaptivní komponenty aplikováním hodnot parametrů UV na vybranou plochu, čímž definujete umístění pro adaptivní typ rodiny.

V tomto příkladu je povrch vytvořen vrámci rodiny objemů vysunutím křivky (toto se provádí ručně) a tento povrch je vybrán jako vstup plochy. Poté jsou zadány hodnoty UV, které určují pozice umístění, a jako typ je použita rodina Diagnostic Tripod – 1 Point.rfa. Uzel AdaptiveComponent.ByParametersOnFace vytvoří výstup adaptivních komponent umístěných na vybrané ploše.  Před spuštěním tohoto grafu je nutné do rodiny objemů načíst soubor „Diagnostic Tripod – 1 Point.rfa“.

Aby bylo možné spustit tento vzorový soubor nápovědy uzlu, je nutné do souboru aplikace Revit načíst soubor „Diagnostics Tripod-1 point.rfa“. Rodina je uložena zde. C:\ProgramData\Autodesk\RVT 2027\Dynamo\Samples\Data
___
## Vzorový soubor

![AdaptiveComponent.ByParametersOnFace](./Revit.Elements.AdaptiveComponent.ByParametersOnFace_img.jpg)
