## In profondità
`ReferencePoint.ByPoint` crea un elemento punto di riferimento nel documento della famiglia di Revit attivo nella posizione del punto specificata. Nota: il documento della famiglia deve essere un componente adattivo o una famiglia di masse. Questo nodo differisce da "ReferencePoint.ByCoordinates" poiché utilizza un punto di Dynamo per la posizione. Ciò è utile perché l'utente finale può utilizzare la manipolazione diretta per modificare il punto di Dynamo, con conseguente aggiornamento anche del punto di riferimento. Per ulteriori informazioni sulla manipolazione diretta, vedere questo [collegamento](https://primer2.dynamobim.org/it/10_sample_workflow/10-1_getting-started-workflows/2-attractor-points).

Nell'esempio seguente, viene creato un nuovo punto di riferimento in corrispondenza delle coordinate 0,0,1.
___
## File di esempio

![ReferencePoint.ByPoint](./Revit.Elements.ReferencePoint.ByPoint_img.jpg)
