## En detalle:


Este nodo devuelve una lista de zonas dentro de cada nodo de conexión.

En este ejemplo, al seleccionar un elemento de viga estructural, se devuelve una lista de zonas. La entrada es una lista de nodos de conexión, donde un nodo de conexión agrupa elementos de datos de estructura (como vigas y pilares) en una única conexión.

Dentro de un nodo de conexión, la zona representa una parte específica del elemento de datos de estructura incluido en la conexión. Los tipos de zona principales son 'end' (extremo) y 'body' (cuerpo).

La zona de extremo representa el final de un elemento de datos de estructura; aquí es donde se produce la conexión.

La zona de cuerpo hace referencia a una ubicación en un elemento de datos de estructura lejos del extremo, por ejemplo, donde se puede conectar una riostra o un armazón de barra en el alma de una viga.


___
## Archivo de ejemplo

![ConnectionNode.Zones](./AdvanceSteel.ConnectionAutomation.Nodes.ConnectionNode.Zones_img.jpg)
