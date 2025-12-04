## Podrobnosti
Tento uzel vytvoří nový prvek rozděleného povrchu na vybrané ploše prvku aplikace Revit a definuje jeho rozvržení pomocí zadaných dělení U a V.  Rozdělený povrch je vzorovaná osnova použitá na plochu, obvykle používaná k umístění panelů obvodového pláště, adaptivních komponent nebo panelových systémů podél povrchu tvaru.

Dílky U a V určují, kolik úseků se vyskytne v každém směru povrchu, zatímco parametr otočení upravuje orientaci rastru vzhledem k souřadnicovému systému U-V povrchu.

V tomto příkladu je vybrána plocha a použije se jako vstup do povrchu pro uzel DividedSurface.ByFaceUVDivisions spolu s uzly UDivs a VDivs, které jsou řízeny posuvníky.  Poslední uzly odhalují hodnoty rozděleného povrchu.  Při spuštění tohoto vzorového grafu bude nutné sledovat upozornění aplikace Revit a odstranit navrhované prvky, aby se osnovy zobrazily na vybraném povrchu.

Další informace naleznete v tomto odkazu.
https://help.autodesk.com/view/RVT/2025/CSY/?guid=GUID-81D9C500-F9CE-462A-AEB7-AA3AEC0C940D
___
## Vzorový soubor

![DividedSurface.ByFaceUVDivisionsAndRotation](./Revit.Elements.DividedSurface.ByFaceUVDivisionsAndRotation_img.jpg)
___
## Vzorový soubor

![DividedSurface.ByFaceAndUVDivisions](./Revit.Elements.DividedSurface.ByFaceAndUVDivisions_img.jpg)
