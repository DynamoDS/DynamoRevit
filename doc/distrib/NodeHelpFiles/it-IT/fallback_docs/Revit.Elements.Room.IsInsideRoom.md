## In profondità
`Room.IsInsideRoom` restituisce un valore booleano che indica se il punto specificato viene trovato o meno nel locale dato.

Nell'esempio seguente, vengono raccolti tutti gli arredi nel documento di Revit corrente, insieme a tutti i locali. I punti di posizione degli arredi vengono quindi passati a `Room.IsInsideRoom` per verificare in quale locali si trovano i punti specificati (se disponibili). Infine, gli arredi vengono filtrati per trovare elementi con le posizioni dei locali e questi valori vengono combinati in elenchi.
___
## File di esempio

![Room.IsInsideRoom](./Revit.Elements.Room.IsInsideRoom_img.jpg)
