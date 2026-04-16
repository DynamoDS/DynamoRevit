## In profondità
Questo nodo estrae le linee chiuse delle curve (in genere archi e linee) che costituiscono il perimetro visibile di una nube di revisione. Ogni segmento della nube è rappresentato come un oggetto curvo (in genere un arco) che corrisponde alla forma “a bolla” del contrassegno di revisione in una vista o in una tavola.

In questo esempio, viene creato un rettangolo utilizzando dispositivi di scorrimento numerici per definirne le quote, che viene poi esploso in curve e invertito per l'orientamento. Queste curve, insieme alla vista scelta (Site Plan) e alla revisione selezionata (Seq. 2 – Not For Construction), vengono utilizzate per generare una nube di revisione con il nodo RevisionCloud.ByCurve. La nube di revisione creata viene quindi collegata al nodo RevisionCloud.Curves, che estrae e visualizza le curve di definizione di tale nube. Ciò consente agli utenti di verificare la geometria della nube di revisione e offre flessibilità per il riutilizzo o un'ulteriore automazione.
___
## File di esempio

![RevisionCloud.Curves](./Revit.Elements.RevisionCloud.Curves_img.jpg)
