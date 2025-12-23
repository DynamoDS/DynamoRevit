## In profondità
Il nodo Revision.SetIssued in Dynamo consente di controllare se una revisione selezionata in Revit è contrassegnata come emessa o non emessa. Richiede un elemento di revisione e un input Boolean (True/False) per fornire agli utenti il controllo diretto sullo stato della revisione senza modificarlo manualmente in Revit.
In questo grafico, il nodo Select Revision viene utilizzato per selezionare una revisione specifica (ad esempio, "Seq. 1 – Schematic Design"). Il nodo Boolean fornisce un valore True/False, che viene quindi collegato al nodo Revision.SetIssued per aggiornare automaticamente lo stato emesso della revisione.

___
## File di esempio

![Revision.SetIssued](./Revit.Elements.Revision.SetIssued_img.jpg)
