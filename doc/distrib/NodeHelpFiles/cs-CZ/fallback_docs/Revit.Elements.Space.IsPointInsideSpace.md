## Podrobnosti
Uzel `Space.IsPointInsideSpace` kontroluje, zda je daný bod uvnitř daného prostoru. To může být užitečné při přiřazování hodnot značek k prvkům v aplikaci Revit.

V níže uvedeném příkladu jsou z aktuálního dokumentu aplikace Revit shromážděny všechny vyústky vzduchotechniky v daném pohledu. Umístění jejich bodů jsou poté porovnána s prostory v daném pohledu pomocí uzlu `Space.IsPointInsideSpace`. Pomocí správy seznamů se vytvoří dílčí seznamy k odfiltrování vyústek vzduchotechniky, které končí uvnitř prostorů. Další informace o používání funkce List@Level naleznete v tomto [článku](https://primer2.dynamobim.org/5_essential_nodes_and_concepts/5-4_designing-with-lists/3-lists-of-lists#list-level).
___
## Vzorový soubor

![Space.IsPointInsideSpace](./Revit.Elements.Space.IsPointInsideSpace_img.jpg)
