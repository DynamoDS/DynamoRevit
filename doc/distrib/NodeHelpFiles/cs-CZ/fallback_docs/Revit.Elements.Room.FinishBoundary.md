## Podrobnosti
Uzel `Room.FinishBoundary` vrací vnořený seznam představující hranici povrchu dané místnosti. Ve vráceném seznamu představuje první dílčí seznam vnější křivky, zatímco další dílčí seznamy představují smyčky uvnitř místnosti. Hranice místnosti vrácená tímto uzlem je umístěna na ploše povrchové úpravy uvnitř místnosti aplikace Revit. Další informace o čarách umístění stěn naleznete v tomto [článku](https://help.autodesk.com/view/RVT/2024/CSY/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89) nápovědy.

Pokud je zadána neohraničená nebo neumístěná místnost, je vrácena hodnota null.

V níže uvedeném příkladu jsou shromážděny všechny místnosti z aktuálního dokumentu a vybraného pohledu. Poté jsou vráceny hranice povrchu.
___
## Vzorový soubor

![Room.FinishBoundary](./Revit.Elements.Room.FinishBoundary_img.jpg)
