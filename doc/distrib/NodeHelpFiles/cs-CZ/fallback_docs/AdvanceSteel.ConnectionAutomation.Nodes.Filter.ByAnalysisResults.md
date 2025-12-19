## Podrobnosti
Tento uzel filtruje seznam uzlů přípojů kontrolou, zda hodnota síly na daném indexu spadá do definovaného rozsahu. Data sil pocházejí z výsledků konstrukční analýzy nebo analytického modelu aplikace Revit a jsou filtrována podle vybraného typu výsledku (například Fx, Fy, Fz, Mx, My, Mz).

V tomto příkladu se vybere sada prvků sloupu a vyhodnotí se podle složky síly Fz pomocí vybraného výsledku analýzy a zatěžovacího stavu. Jako přijatá připojení jsou vráceny pouze prvky, jejichž hodnota Fz spadá do zadaného rozsahu síly.
___
## Vzorový soubor

![Filter.ByAnalysisResults](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.ByAnalysisResults_img.jpg)
