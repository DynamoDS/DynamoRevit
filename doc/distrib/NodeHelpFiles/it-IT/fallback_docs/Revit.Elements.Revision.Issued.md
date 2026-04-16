## In profondità
Il nodo Revision.Issued in Dynamo viene utilizzato per verificare se una revisione in Revit è contrassegnata come emessa. Restituisce un valore true o false (Boolean), aiutando i team a verificare rapidamente lo stato delle revisioni senza aprire le impostazioni di revisione di Revit.

In questo grafico, il nodo Select Revision viene utilizzato per scegliere una revisione dal progetto. Il nodo Revision.Issued verifica quindi se la revisione selezionata è stata emessa e il risultato viene visualizzato nel nodo Watch come true o false. In questo modo è più semplice confermare lo stato di emissione di una revisione direttamente tramite Dynamo.

___
## File di esempio

![Revision.Issued](./Revit.Elements.Revision.Issued_img.jpg)
