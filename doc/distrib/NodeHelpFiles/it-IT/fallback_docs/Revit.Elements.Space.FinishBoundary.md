## In profondità
`Space.FinishBoundary` restituisce un elenco nidificato che rappresenta il contorno di finitura del vano specificato. Nell'elenco restituito, il primo sottoelenco rappresenta le curve più esterne, mentre gli elenchi successivi rappresentano i perimetri all'interno del vano. Il contorno del vano restituito da questo nodo si trova in corrispondenza della superficie di finitura all'interno del vano di Revit. Per ulteriori informazioni sulle linee di ubicazione dei muri, vedere questa guida [articolo](https://help.autodesk.com/view/RVT/2024/ITA/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89).

Se viene specificato un vano non delimitato o non posizionato, viene restituito un valore null.

Nell'esempio seguente, tutti i vani vengono raccolti dal documento corrente e dalla vista selezionata. Vengono quindi restituiti i contorni di finitura.

___
## File di esempio

![Space.FinishBoundary](./Revit.Elements.Space.FinishBoundary_img.jpg)
