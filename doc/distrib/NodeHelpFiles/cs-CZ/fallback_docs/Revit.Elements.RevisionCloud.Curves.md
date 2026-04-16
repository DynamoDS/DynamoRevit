## Podrobnosti
Tento uzel extrahuje smyčky křivek (obvykle oblouky a čáry), které tvoří viditelný obvod revizního obláčku. Každý segment obláčku je reprezentován jako objekt křivky (obvykle oblouk) odpovídající tvaru „bubliny“ revizní značky v pohledu nebo výkresu.

V tomto příkladu je obdélník vytvořen pomocí číselných posuvníků, které definují jeho rozměry, a je poté rozložen na křivky a obrácen podle orientace. Tyto oblouky se spolu s vybraným pohledem (Půdorys pozemku) a revizí (násl. 2 – Není určeno pro konstrukci) používají ke generování revizního obláčku pomocí uzlu RevisionCloud.ByCurve. Vytvořený revizní obláček je poté připojen kuzlu RevisionCloud.Curves, který extrahuje a zobrazí definující křivky tohoto obláčku. To pomáhá uživatelům ověřit geometrii revizního obláčku a nabízí flexibilitu pro opakované použití nebo další automatizaci.
___
## Vzorový soubor

![RevisionCloud.Curves](./Revit.Elements.RevisionCloud.Curves_img.jpg)
