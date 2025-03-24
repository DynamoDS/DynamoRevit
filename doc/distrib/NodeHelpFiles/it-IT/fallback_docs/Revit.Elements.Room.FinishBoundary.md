## In profondità
`Room.FinishBoundary` restituisce un elenco nidificato che rappresenta il contorno di fine del locale specificato. Nell'elenco restituito, il primo sottoelenco rappresenta le curve più esterne, mentre gli elenchi successivi rappresentano i perimetri all'interno del locale. Il contorno del locale restituito da questo nodo si trova sulla superficie di finitura all'interno del locale di Revit. Per ulteriori informazioni sulle linee di ubicazione dei muri, vedere questa guida [articolo](https://help.autodesk.com/view/RVT/2024/ITA/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89).

Se viene specificato un locale non delimitato o non posizionato, viene restituito un valore null.

Nell'esempio seguente, vengono raccolti tutti i locali dal documento corrente e dalla vista selezionata. Vengono quindi restituiti i contorni di fine.
___
## File di esempio

![Room.FinishBoundary](./Revit.Elements.Room.FinishBoundary_img.jpg)
