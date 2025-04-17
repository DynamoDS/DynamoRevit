## In profondità
`Room.CoreCenterBoundary` restituisce un elenco nidificato che rappresenta il contorno del centro del nucleo del locale specificato. Nell'elenco restituito, il primo sottoelenco rappresenta le curve più esterne, mentre gli elenchi successivi rappresentano i perimetri all'interno del locale. I contorni del centro del nucleo si trovano al centro dei muri definiti come nucleo. Per ulteriori informazioni sulle linee di ubicazione dei muri, vedere questa guida [articolo](https://help.autodesk.com/view/RVT/2024/ITA/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89).

Se viene specificato un locale non delimitato o non posizionato, viene restituito un valore null.

Nell'esempio seguente, vengono raccolti tutti i locali dal documento corrente e dalla vista selezionata. Vengono quindi restituiti i contorni del centro del nucleo.
___
## File di esempio

![Room.CoreCenterBoundary](./Revit.Elements.Room.CoreCenterBoundary_img.jpg)
