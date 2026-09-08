## In profondità
`Gate` controlla se i dati di input possono passare in base ad un valore booleano. Se il gate è aperto (True), l'input viene trasmesso all'output. Se il gate è chiuso (False), l'input è bloccato.

Nell'esempio seguente, il nodo `Gate` viene utilizzato per evitare che la geometria venga esplosa ad ogni esecuzione del grafico. Quando il gate è aperto (True), la geometria appena creata lo attraversa e viene esplosa. Quando il gate è chiuso (False), la geometria viene bloccata e non viene né trasmessa né esplosa nuovamente.
___
## File di esempio

![Gate](./CoreNodeModels.Logic.Gate_img.jpg)
