## In profondità
Questo nodo restituisce un elenco di nodi di connessione filtrati in base ad un intervallo di gradi min-max.

Nell'esempio, vengono selezionati un pilastro a forma di W e un controvento HSS angolato e l'output consiste in un elenco di dizionari con le proprietà "accepted" e "rejected". Il valore accettato è un nodo di connessione compreso nell'intervallo tra 0 e 60 gradi (questo nodo si misura dall'asse X). Se il valore massimo fosse 45 gradi, il nodo di connessione verrebbe rifiutato. I parametri `indexFirst` e `indexSecond` sono impostati su 'use levels' per allinearsi con la struttura dei dati di input.
___
## File di esempio

![Filter.ByAngleBetweenBeams](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.ByAngleBetweenBeams_img.jpg)
