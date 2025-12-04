## In profondità
Questo nodo proietta un raggio in un modello di Revit collegato da un'origine e una direzione specificate, quindi traccia i rimbalzi successivi sugli elementi collegati. Ogni rimbalzo rappresenta un punto in cui il raggio interseca la geometria nel modello collegato, fino al numero massimo di riflessioni definito.

In questo esempio, viene selezionato un elemento collegato e la sua posizione viene utilizzata come input origin per LinkElement.ByRayBounce insieme ad una direzione, maxBounces e una vista. Gli output sono punti ed elementi collegati.
___
## File di esempio

![LinkElement.ByRayBounce](./Revit.Elements.LinkElement.ByRayBounce_img.jpg)
