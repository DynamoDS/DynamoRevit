## In profondità
Questo nodo estrae l'elemento di revisione collegato ad una nube di revisione specifica in Revit. Fornisce i dati di revisione associati a tale nube, consentendo agli utenti di controllare, tracciare o convalidare i dettagli di revisione a livello di programmazione all'interno del progetto.

In questo esempio, viene creato un rettangolo con i dispositivi di scorrimento numerici per larghezza e lunghezza, quindi esploso in curve e invertito per ottenere un orientamento corretto. Queste curve, insieme alla vista scelta (L1_SD) e alla revisione selezionata (Seq. 2 – Not For Construction), vengono utilizzate per generare una nube di revisione tramite il nodo RevisionCloud.ByCurve. La nube di revisione risultante viene connessa al nodo RevisionCloud.Revision, che recupera e genera la revisione associata a tale nube. In questo modo gli utenti possono verificare quale revisione è collegata a ciascuna nube di revisione.
___
## File di esempio

![RevisionCloud.Revision](./Revit.Elements.RevisionCloud.Revision_img.jpg)
