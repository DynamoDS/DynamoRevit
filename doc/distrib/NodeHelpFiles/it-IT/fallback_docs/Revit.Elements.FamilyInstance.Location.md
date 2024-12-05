## In profondità
`FamilyInstance.Location` restituisce un punto di Dynamo per l'istanza di famiglia specificata. Se non è presente alcuna posizione, viene restituito un valore null. `FamilyInstance.Location` funziona nell'elemento basato su punti e non restituirà una posizione per l'elemento basato su curve in Revit, _ad es. un elemento muro o trave_.

Nell'esempio seguente, vengono raccolte tutte le istanze della famiglia di porte nella vista corrente del documento corrente. Le posizioni delle porte vengono quindi restituite con `FamilyInstance.Location`.

Nel caso di questo esempio, le porte di facciata continua restituiscono null. Le posizioni dei pannelli di facciata continua sono disponibili tramite i nodi dei pannelli di facciata continua.
___
## File di esempio

![FamilyInstance.Location](./Revit.Elements.FamilyInstance.Location_img.jpg)
