## In profondità
Questo nodo crea un nuovo elemento DividedSurface su una faccia selezionata di un elemento di Revit e ne definisce il layout utilizzando le divisioni U e V specificate. Una superficie divisa è una griglia con motivo applicata ad una faccia, comunemente utilizzata per posizionare pannelli di facciata continua, componenti adattivi o sistemi a pannelli sulla superficie di una forma.

Le divisioni U e V determinano il numero di suddivisioni in ogni direzione della superficie, mentre il parametro di rotazione regola l'orientamento della griglia rispetto al sistema di coordinate U-V della superficie.

In questo esempio, viene selezionata una faccia che è poi utilizzata come input surface per il nodo DividedSurface.ByFaceUVDivisions insieme a UDivs e VDivs controllati dai dispositivi di scorrimento. Gli ultimi nodi espongono i valori della superficie divisa. Durante l'esecuzione di questo grafico di esempio, è necessario osservare l'avviso di Revit ed eliminare gli elementi suggeriti in modo che le griglie vengano visualizzate sulla superficie selezionata.

Per ulteriori informazioni, vedere il collegamento.
https://help.autodesk.com/view/RVT/2025/ITA/?guid=GUID-81D9C500-F9CE-462A-AEB7-AA3AEC0C940D
___
## File di esempio

![DividedSurface.ByFaceUVDivisionsAndRotation](./Revit.Elements.DividedSurface.ByFaceUVDivisionsAndRotation_img.jpg)
___
## File di esempio

![DividedSurface.ByFaceAndUVDivisions](./Revit.Elements.DividedSurface.ByFaceAndUVDivisions_img.jpg)
