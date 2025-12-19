## En detalle:
Este nodo recupera el tipo de zona de un elemento de zona de Advance Steel determinado. En la estructura de automatización de la conexión de Advance Steel, las zonas definen distintas regiones de una barra de acero (como el inicio, el cuerpo o el final) donde se pueden aplicar operaciones de conexión. El tipo de zona identifica las regiones que representa una zona concreta, lo que resulta esencial a la hora de automatizar la colocación de juntas, las reducciones o las ubicaciones de los rigidizadores.

En este ejemplo, se seleccionan varios elementos, se extraen los datos estructurales, se expone el nodo de conexión y se utiliza este para identificar todas las zonas que se aplican a ese nodo de conexión. Estas zonas son la entrada al nodo Zone.ZoneType donde la salida define la región de la barra de acero (inicio, cuerpo o final).
___
## Archivo de ejemplo

![Zone.ZoneType](./AdvanceSteel.ConnectionAutomation.Nodes.Zone.ZoneType_img.jpg)
