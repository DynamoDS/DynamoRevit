## En detalle:
El nodo Revision.SetIssued de Dynamo permite controlar si una revisión seleccionada en Revit se marca como emitida o no emitida. Se necesita un elemento de revisión y una entrada booleana (verdadero/falso), lo que proporciona a los usuarios control directo sobre el estado de revisión sin necesidad de editarlo manualmente en Revit.
En este gráfico, el nodo Select Revision se utiliza para elegir una revisión específica (por ejemplo, "Seq. 1 – Schematic Design"). El nodo booleano proporciona un valor de verdadero/falso que se conecta al nodo Revision.SetIssued para actualizar automáticamente el estado de emisión de la revisión.

___
## Archivo de ejemplo

![Revision.SetIssued](./Revit.Elements.Revision.SetIssued_img.jpg)
