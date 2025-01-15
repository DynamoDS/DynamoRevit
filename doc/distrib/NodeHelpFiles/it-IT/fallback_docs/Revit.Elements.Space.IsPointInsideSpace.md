## In profondità
`Space.IsPointInsideSpace` verifica se un determinato punto si trova all'interno di un dato vano. Ciò può essere utile quando si assegnano valori di contrassegno ad elementi all'interno di Revit.

Nell'esempio seguente, vengono raccolti tutti i bocchettoni nella vista specificata nel documento di Revit corrente. Le posizioni dei punti vengono quindi confrontate con i vani nella vista specificata con `Space.IsPointInsideSpace`. Utilizzando la gestione degli elenchi, vengono sviluppati sottoelenchi per escludere tramite filtro i bocchettoni che si trovano all'interno dei vani. Per ulteriori informazioni sull'utilizzo di List@Level, vedere questo [articolo](https://primer2.dynamobim.org/it/5_essential_nodes_and_concepts/5-4_designing-with-lists/3-lists-of-lists).
___
## File di esempio

![Space.IsPointInsideSpace](./Revit.Elements.Space.IsPointInsideSpace_img.jpg)
