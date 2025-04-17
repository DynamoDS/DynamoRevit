## Podrobnosti
Uzel `Room.CoreCenterBoundary` vrací vnořený seznam představující hranici středu nosné části místnosti dané místnosti. Ve vráceném seznamu představuje první dílčí seznam vnější křivky, zatímco další dílčí seznamy představují smyčky uvnitř místnosti. Hranice středu nosné části se nacházejí ve středu stěn, které jsou definovány jako nosná část. Další informace o čarách umístění stěn naleznete v tomto [článku](https://help.autodesk.com/view/RVT/2024/CSY/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89) nápovědy.

Pokud je zadána neohraničená nebo neumístěná místnost, je vrácena hodnota null.

V níže uvedeném příkladu jsou shromážděny všechny místnosti z aktuálního dokumentu a vybraného pohledu. Poté jsou vráceny hranice středu nosné části.
___
## Vzorový soubor

![Room.CoreCenterBoundary](./Revit.Elements.Room.CoreCenterBoundary_img.jpg)
