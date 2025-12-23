## In profondità
Questo nodo posiziona componenti adattivi applicando i valori dei parametri UV alla faccia selezionata, definendo i punti di posizionamento per il tipo di famiglia adattiva.

In questo esempio, viene creata una superficie all'interno della famiglia di masse mediante l'estrusione di una curva (questa operazione viene eseguita manualmente) e tale superficie viene selezionata come input face. Vengono quindi forniti i valori UV per determinare i punti di posizionamento e viene utilizzata la famiglia Diagnostic Tripod – 1 Point.rfa come tipo. Il nodo AdaptiveComponent.ByParametersOnFace genera componenti adattivi posizionati sulla faccia selezionata. Tenere presente che il file "Diagnostic Trepod – 1 Point.rfa" deve essere caricato nella famiglia di masse prima di eseguire questo grafico.

Per eseguire questo file di esempio della Guida del nodo, è necessario caricare Diagnostics Tripod-1 point.rfa nel file di Revit. La famiglia viene memorizzata qui in C:\ProgramData\Autodesk\RVT 2027\Dynamo\Samples\Data
___
## File di esempio

![AdaptiveComponent.ByParametersOnFace](./Revit.Elements.AdaptiveComponent.ByParametersOnFace_img.jpg)
