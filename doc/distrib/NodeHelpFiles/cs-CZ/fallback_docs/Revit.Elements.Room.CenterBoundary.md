## Podrobnosti
Uzel `Room.CenterBoundary` vrací vnořený seznam představující hranici osy dané místnosti. Ve vráceném seznamu představuje první dílčí seznam vnější křivky, zatímco další dílčí seznamy představují smyčky uvnitř místnosti. Středové hranice jsou umístěny na ose stěny přes všechny hladiny uvnitř místnosti aplikace Revit. Další informace o čarách umístění stěn naleznete v tomto [článku](https://help.autodesk.com/view/RVT/2024/CSY/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89) nápovědy.

Pokud je zadána neohraničená nebo neumístěná místnost, je vrácena hodnota null.

V níže uvedeném příkladu jsou shromážděny všechny místnosti z aktuálního dokumentu a vybraného pohledu. Poté jsou vráceny středové hranice.
___
## Vzorový soubor

![Room.CenterBoundary](./Revit.Elements.Room.CenterBoundary_img.jpg)
