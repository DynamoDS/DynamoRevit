## In profondità
`Room.CenterBoundary` restituisce un elenco nidificato che rappresenta il contorno della linea d'asse del locale specificato. Nell'elenco restituito, il primo sottoelenco rappresenta le curve più esterne, mentre gli elenchi successivi rappresentano i perimetri all'interno del locale. I contorni del centro si trovano sulla linea d'asse del muro in tutti gli strati all'interno del locale di Revit. Per ulteriori informazioni sulle linee di ubicazione dei muri, vedere questa guida [articolo](https://help.autodesk.com/view/RVT/2024/ITA/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89).

Se viene specificato un locale non delimitato o non posizionato, viene restituito un valore null.

Nell'esempio seguente, vengono raccolti tutti i locali dal documento corrente e dalla vista selezionata. Vengono quindi restituiti i contorni del centro.
___
## File di esempio

![Room.CenterBoundary](./Revit.Elements.Room.CenterBoundary_img.jpg)
