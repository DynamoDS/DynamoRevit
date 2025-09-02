## In profondità

Restituisce il tipo di unità di un parametro.

Nell'esempio seguente, vengono estratti tutti i parametri degli elementi e utilizzati come input. L'obiettivo è quello di visualizzare il tipo di unità per ogni parametro.
Ad esempio, se Element.Parameters mostra "Base Diameter : 1’ – 3 ¼”", l'output di Parameter.Unit sarà "Unit (Name = Feet or Meters)".
Se un parametro non dispone di un'unità (come Element.Parameters = Apply Cut: No), Parameter.Unit restituirà null.
Poiché Dynamo stesso è senza unità, è fondamentale identificare i tipi di unità in entrata per eseguire calcoli accurati. Questo nodo consente a Dynamo di riconoscere ed elaborare i dati di tali unità.

___
## File di esempio

![Parameter.Unit](./Revit.Elements.Parameter.Unit_img.jpg)
