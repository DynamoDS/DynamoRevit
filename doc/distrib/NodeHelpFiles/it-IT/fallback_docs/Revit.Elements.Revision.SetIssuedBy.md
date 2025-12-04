## In profondità
Il nodo Revision.SetIssuedBy in Dynamo viene utilizzato per impostare o aggiornare il valore “Issued By” per una revisione in Revit. Consente di automatizzare il controllo delle revisioni registrando chi ha emesso la revisione e garantendo che la documentazione sia chiara e coerente senza modifiche manuali in Revit.

In questo grafico, il nodo Select Revision viene utilizzato per selezionare la revisione richiesta e un input string (ad esempio, ABC) fornisce il nome dell'emittente. Il nodo Revision.SetIssuedBy applica quindi questo valore alla revisione selezionata, aggiornando il campo “Issued By” direttamente nel modello di Revit.

___
## File di esempio

![Revision.SetIssuedBy](./Revit.Elements.Revision.SetIssuedBy_img.jpg)
