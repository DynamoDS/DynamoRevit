## In profondità


Questo nodo restituisce un elenco di zone all'interno di ogni nodo di connessione.

In questo esempio, la selezione di un elemento trave strutturale restituisce un elenco di zone. L'input è un elenco di nodi di connessione, dove un nodo di connessione raggruppa elementi dei dati della struttura (come travi e pilastri) in una singola connessione.

All'interno di un nodo di connessione, la zona rappresenta una parte specifica dell'elemento dei dati della struttura coinvolto nella connessione. I tipi di zona principali sono 'end' e 'body'.

La zona finale rappresenta la fine di un elemento dei dati della struttura: è qui che avviene la connessione.

La zona del corpo si riferisce ad una posizione su un elemento dei dati della struttura lontana dall'estremità, ad esempio il punto in cui un controvento o un telaio di membri nell'anima di una trave potrebbe collegarsi.


___
## File di esempio

![ConnectionNode.Zones](./AdvanceSteel.ConnectionAutomation.Nodes.ConnectionNode.Zones_img.jpg)
