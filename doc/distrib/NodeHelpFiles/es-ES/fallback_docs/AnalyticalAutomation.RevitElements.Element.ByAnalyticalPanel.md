## En detalle:
Este nodo crea un elemento físico de muro o suelo a partir de un panel analítico o actualiza un elemento físico existente que ya está conectado al panel analítico. El elemento físico puede heredar la geometría, los parámetros y los huecos del panel analítico, en función de la configuración de entrada. Cuando el panel analítico no tiene ningún elemento físico asociado, el nodo crea uno nuevo basado en el modelo analítico.

En este ejemplo, se selecciona un panel de chapa de cubierta analítico del modelo ACO Supermarket. Ese panel analítico se conecta a este nodo y los valores booleanos de entrada muestran el comportamiento por defecto para actualizar la geometría y los parámetros, e incluir huecos. A continuación, el nodo crea o actualiza el elemento físico de cubierta desde el panel analítico.
___
## Archivo de ejemplo

![Element.ByAnalyticalPanel](./AnalyticalAutomation.RevitElements.Element.ByAnalyticalPanel_img.jpg)
