## In profondità
Questo nodo esegue un'analisi dei rimbalzi dei raggi all'interno del modello di Revit. Partendo da un punto di origine specificato e viaggiando lungo un vettore di direzione specificato, traccia il percorso del raggio mentre interseca gli elementi nel modello. Quando il raggio colpisce una superficie, può continuare a rimbalzare su di essa, a seconda del numero di rimbalzi consentiti, simulando il comportamento della luce, della visibilità o del riflesso del percorso.

In questo esempio, viene selezionato un elemento e la sua posizione viene utilizzata per l'input origin nel nodo RayBounce.ByOriginDirection, insieme a direction, maxBounces e view.
___
## File di esempio

![RayBounce.ByOriginDirection](./Revit.References.RayBounce.ByOriginDirection_img.jpg)
