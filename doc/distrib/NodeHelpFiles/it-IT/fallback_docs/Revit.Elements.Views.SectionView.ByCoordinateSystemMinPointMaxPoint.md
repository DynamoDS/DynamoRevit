## In profondità
Crea viste in sezione a partire da qualsiasi CoordinateSystem con punti min e max.

In questo esempio, viene generato un elemento SectionView il cui posizionamento e il cui orientamento sono direttamente correlati all'input CoordinateSystem. Qui, l'elemento CoordinateSystem è stato posizionato in modo strategico per definire una sezione incentrata sull'angolo superiore destro dell'edificio. È fondamentale osservare che il componente Z dell'input minPoint e maxPoint determina con precisione il parametro Offset del ritaglio orizzontale dell'elemento SectionView di Revit risultante, controllando così la profondità effettiva di visualizzazione nel modello.
Per ulteriori informazioni su come controllare la visualizzazione degli elementi, vedere il collegamento riportato di seguito.
https://help.autodesk.com/view/RVT/2024/ITA/?guid=GUID-48484D2E-28ED-4BC3-8703-3B0488F1DA56
___
## File di esempio

![SectionView.ByCoordinateSystemMinPointMaxPoint](./Revit.Elements.Views.SectionView.ByCoordinateSystemMinPointMaxPoint_img.jpg)
