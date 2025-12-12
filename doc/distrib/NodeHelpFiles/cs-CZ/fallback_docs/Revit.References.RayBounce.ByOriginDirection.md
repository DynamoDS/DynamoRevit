## Podrobnosti
Tento uzel provádí analýzu odrazů paprsků v modelu aplikace Revit. Vychází z daného bodu počátku, postupuje podél určeného směrového vektoru a sleduje dráhu paprsku při průsečíku s prvky v modelu. Když paprsek dopadne na povrch, může se od tohoto povrchu dále odrážet, v závislosti na povoleném počtu odrazů, a simulovat světlo, viditelnost nebo chování dráhy odrazu.

V tomto příkladu je vybrán prvek a jeho umístění je použito jako vstup počátku do uzlu RayBounce.ByOriginDirection spolu se směrem, indexem maxBounces a pohledem.
___
## Vzorový soubor

![RayBounce.ByOriginDirection](./Revit.References.RayBounce.ByOriginDirection_img.jpg)
