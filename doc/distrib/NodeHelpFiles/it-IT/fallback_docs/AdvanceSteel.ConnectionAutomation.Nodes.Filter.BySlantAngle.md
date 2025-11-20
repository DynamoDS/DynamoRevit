## In profondità
Questo nodo restituisce un elenco di nodi di connessione filtrati in base ad un intervallo di gradi min-max dell'angolo di inclinazione. L'angolo di inclinazione è l'angolo tra l'asse x di un elemento dei dati della struttura e l'asse verticale.

Nell'esempio, vengono selezionati un pilastro a forma di W e un controvento HSS angolato e l'output consiste in un elenco di dizionari con le proprietà "accepted" e "rejected". Il valore accettato è un nodo di connessione con un angolo di inclinazione compreso tra 30 e 60 gradi. Il controvento HSS angolato ha un angolo di inclinazione di circa 30 gradi, pertanto è accettato. Il pilastro a forma di W ha un angolo di inclinazione di 0 gradi e viene rifiutato.
___
## File di esempio

![Filter.BySlantAngle](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.BySlantAngle_img.jpg)
