## In profondità
`ElementCurveReference.ByCurve` crea un riferimento alla curva dell'elemento di Revit da una curva di Dynamo. Un riferimento alla curva è utile quando un altro nodo di Revit necessita di una curva selezionabile o a cui fare riferimento, anziché solo della geometria di Dynamo. Questa opzione viene in genere utilizzata per i workflow che richiedono riferimenti di Revit, ad esempio la creazione di quote, allineamenti, vincoli o altri elementi che devono fare riferimento alla geometria del modello.

Nell'esempio seguente, viene selezionata una curva che viene utilizzata come input per `ElementCurveReference.ByCurve`. L'output è `ElementCurveReference` che viene quindi utilizzato come riferimento alla curva di Revit per i nodi a valle che richiedono riferimenti basati sulle curve.
___
## File di esempio

![ElementCurveReference.ByCurve](./Revit.GeometryReferences.ElementCurveReference.ByCurve_img.jpg)
