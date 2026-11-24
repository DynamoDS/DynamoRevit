## Podrobnosti
Uzel `VectorAnalysisDisplay.ByViewPointsAndVectorValues` vytvoří zobrazení analýzy vektorů v pohledu aplikace Revit. Zobrazení vstupu definuje, kde se zobrazí zobrazení analýzy. Body definují umístění značek analýzy, vektory definují směr zobrazené analýzy a hodnoty definují číselný výsledek asociovaný s každým bodem. Výstupem je prvek zobrazení analýzy aplikace Revit zobrazený ve vybraném pohledu.

V níže uvedeném příkladu jsou vytvořeny dva body pro definování umístění vzorků v Revit 3D pohledu. Poté jsou vytvořeny dva vektory, které definují data směrové analýzy v těchto umístěních. Body a vektory jsou sloučeny do seznamů a použity jako vstupy pro uzel `VectorAnalysisDisplay.ByViewPointsAndVectorValues` společně s pohledem, názvem a popisem analýzy. Výstupem je zobrazení analýzy vektoru zobrazené ve vybraném pohledu aplikace Revit.
___
## Vzorový soubor

![VectorAnalysisDisplay.ByViewPointsAndVectorValues](./Revit.AnalysisDisplay.VectorAnalysisDisplay.ByViewPointsAndVectorValues_img.jpg)
