## Podrobnosti
Tento uzel extrahuje prvek revize připojený ke konkrétnímu reviznímu obláčku v aplikaci Revit. Poskytuje data revizí asociovaná s tímto obláčkem a umožňuje uživatelům kontrolovat, sledovat nebo ověřovat podrobnosti revizí programově v rámci projektu.

V tomto příkladu je obdélník vytvořen pomocí číselných posuvníků šířky a délky, poté rozložen do křivek a obrácen, aby byla zajištěna správná orientace. Tyto křivky se spolu s vybraným pohledem (L1_SD) a vybranou revizí (násl. 2 – Není určeno pro konstrukci) používají ke generování revizního obláčku prostřednictvím uzlu RevisionCloud.ByCurve. Výsledný revizní obláček je připojen k uzlu RevisionCloud.Revision, který načte a vytvoří výstup revize asociované s daným obláčkem. Uživatelé tak mohou potvrdit, která revize je svázána s každým revizním obláčkem.
___
## Vzorový soubor

![RevisionCloud.Revision](./Revit.Elements.RevisionCloud.Revision_img.jpg)
