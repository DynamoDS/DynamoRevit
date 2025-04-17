## In profondità
`Room.CoreBoundary` restituisce un elenco nidificato che rappresenta il contorno del nucleo del locale specificato. Nell'elenco restituito, il primo sottoelenco rappresenta le curve più esterne, mentre gli elenchi successivi rappresentano i perimetri all'interno del locale. I contorni del nucleo si trovano sullo strato interno o esterno del nucleo che è più vicino al locale. Per ulteriori informazioni sulle linee di ubicazione dei muri, vedere questa guida [articolo](https://help.autodesk.com/view/RVT/2024/ITA/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89).

Se viene specificato un locale non delimitato o non posizionato, viene restituito un valore null.

Nell'esempio seguente, vengono raccolti tutti i locali dal documento corrente e dalla vista selezionata. Vengono quindi restituiti i contorni del nucleo.
___
## File di esempio

![Room.CoreBoundary](./Revit.Elements.Room.CoreBoundary_img.jpg)
