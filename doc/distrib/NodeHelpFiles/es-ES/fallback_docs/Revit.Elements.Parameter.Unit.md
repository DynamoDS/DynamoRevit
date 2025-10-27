## En detalle:

Devuelve el tipo de unidad de un parámetro.

En el ejemplo siguiente, extraemos todos los parámetros de elementos y los utilizamos como entrada. El objetivo es mostrar el tipo de unidad de cada parámetro.
Por ejemplo, si Element.Parameters muestra "Base Diameter : 1’ – 3 ¼”", la salida de Parameter.Unit será "Unit (Name = Feet or Meters)".
Si un parámetro no tiene una unidad (como "Element.Parameters = Apply Cut : No"), Parameter.Unit devolverá un valor nulo.
Como Dynamo en sí no tiene unidades, es crucial identificar los tipos de unidades entrantes para realizar cálculos precisos. Este nodo ayuda a Dynamo a reconocer y procesar los datos de unidades.

___
## Archivo de ejemplo

![Parameter.Unit](./Revit.Elements.Parameter.Unit_img.jpg)
