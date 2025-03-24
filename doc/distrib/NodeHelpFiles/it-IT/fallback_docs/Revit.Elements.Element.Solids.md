## In profondità
`Element.Solids` restituisce la geometria solida per l'elemento specificato. I solidi vengono restituiti come elenchi nidificati, poiché qualsiasi elemento specificato può avere più di un solido al suo interno. Se si desidera un singolo solido che rappresenta l'elemento, è possibile utilizzare `Solid.ByUnion` negli elenchi.

Nell'esempio seguente, vengono raccolti tutti i muri dalla vista selezionata. I muri creati come famiglie locali vengono quindi rimossi e vengono restituiti i solidi dei muri rimanenti.

___
## File di esempio

![Element.Solids](./Revit.Elements.Element.Solids_img.jpg)
