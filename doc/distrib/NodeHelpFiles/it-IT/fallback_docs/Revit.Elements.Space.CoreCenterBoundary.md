## In profondità
`Space.CoreCenterBoundary` restituisce un elenco nidificato che rappresenta il contorno centrale del nucleo del vano specificato. Nell'elenco restituito, il primo sottoelenco rappresenta le curve più esterne, mentre gli elenchi successivi rappresentano i perimetri all'interno del vano. I contorni centrali del nucleo si trovano al centro dei muri definiti come nucleo. Per ulteriori informazioni sulle linee di ubicazione dei muri, vedere questa guida [articolo](https://help.autodesk.com/view/RVT/2024/ITA/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89).

Se viene specificato un vano non delimitato o non posizionato, viene restituito un valore null.

Nell'esempio seguente, tutti i vani vengono raccolti dal documento corrente e dalla vista selezionata. Vengono quindi restituiti i contorni centrali del nucleo.

___
## File di esempio

![Space.CoreCenterBoundary](./Revit.Elements.Space.CoreCenterBoundary_img.jpg)
