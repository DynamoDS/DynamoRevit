## In profondità
`Dimension.ByReferences` posiziona una quota lungo la linea di input nella vista selezionata, utilizzando i riferimenti specificati. L'output è un elemento Quota di Revit.

Nell'esempio seguente, vengono creati diversi punti che vengono collegati con nodi Line per generare curve del modello in Revit. Le curve del modello vengono quindi convertite in riferimenti alle curve di Revit. I riferimenti vengono combinati in un elenco e utilizzati come input `references` per `Dimension.ByReferences`. Vengono create linee aggiuntive per definire il posizionamento e la direzione delle quote. Anche la vista Revit attiva viene fornita come input `view`.
___
## File di esempio

![Dimension.ByReferences](./Revit.Elements.Dimension.ByReferences_img.jpg)
