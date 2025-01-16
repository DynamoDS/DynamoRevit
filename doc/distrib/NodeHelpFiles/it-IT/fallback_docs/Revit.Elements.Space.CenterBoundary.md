## In profondità
`Space.CenterBoundary` restituisce un elenco nidificato che rappresenta il contorno della linea d'asse del vano specificato. Nell'elenco restituito, il primo sottoelenco rappresenta le curve più esterne, mentre gli elenchi successivi rappresentano i perimetri all'interno del vano. I contorni centrali si trovano sulla linea d'asse del muro in tutti gli strati all'interno del vano di Revit. Per ulteriori informazioni sulle linee di ubicazione dei muri, vedere questa guida [articolo](https://help.autodesk.com/view/RVT/2024/ITA/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89).

Se viene specificato un vano non delimitato o non posizionato, viene restituito un valore null.

Nell'esempio seguente, tutti i vani vengono raccolti dal documento corrente e dalla vista selezionata. Vengono quindi restituiti i contorni centrali.
___
## File di esempio

![Space.CenterBoundary](./Revit.Elements.Space.CenterBoundary_img.jpg)
