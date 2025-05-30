## In profondità
`Space.CoreBoundary` restituisce un elenco nidificato che rappresenta il contorno del nucleo del vano specificato. Nell'elenco restituito, il primo sottoelenco rappresenta le curve più esterne, mentre gli elenchi successivi rappresentano i perimetri all'interno del vano. I contorni del nucleo si trovano in corrispondenza dello strato interno o esterno del nucleo più vicino al locale. Per ulteriori informazioni sulle linee di ubicazione dei muri, vedere questa guida [articolo](https://help.autodesk.com/view/RVT/2024/ITA/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89).

Se viene specificato un vano non delimitato o non posizionato, viene restituito un valore null.

Nell'esempio seguente, tutti i vani vengono raccolti dal documento corrente e dalla vista selezionata. Vengono quindi restituiti i contorni del nucleo.

___
## File di esempio

![Space.CoreBoundary](./Revit.Elements.Space.CoreBoundary_img.jpg)
