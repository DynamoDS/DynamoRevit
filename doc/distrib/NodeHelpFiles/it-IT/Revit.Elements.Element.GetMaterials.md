## In profondità
`Element.GetMaterials` restituisce tutti i materiali _(e i relativi ID)_ presenti in un elemento di Revit. Gli elementi con più materiali restituiranno un elenco per ogni elemento. L'input "paintMaterials" è un pulsante di commutazione booleano che consente di indicare al nodo di raccogliere anche i materiali su cui l'utente ha applicato la vernice.

Nell'esempio seguente, vengono restituiti i materiali per tutte le istanze di muro nel documento (file) corrente.
___
## File di esempio

![Element.GetMaterials](./Revit.Elements.Element.GetMaterials_img.jpg)
