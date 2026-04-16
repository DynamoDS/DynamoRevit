## En detalle:
Este nodo extrae los bucles de curva (normalmente arcos y líneas) que conforman el perímetro visible de una nube de revisión. Cada segmento de la nube se representa como un objeto de curva (normalmente un arco) que corresponde a la forma de "burbuja" de la marca de revisión en una vista o un plano.

En este ejemplo, se crea un rectángulo mediante controles deslizantes numéricos para definir sus dimensiones y luego se descompone en curvas y se invierte para establecer la orientación. Estas curvas, junto con una vista (Plano de emplazamiento) y una revisión (Seq. 2 – Not For Construction) seleccionadas, se utilizan para generar una nube de revisión con el nodo RevisionCloud.ByCurve. La nube de revisión creada se conecta al nodo RevisionCloud.Curves, que extrae y muestra las curvas de definición de esa nube. Esto ayuda a los usuarios a verificar la geometría de la nube de revisión y proporciona flexibilidad para reutilizarla o aumentar la automatización.
___
## Archivo de ejemplo

![RevisionCloud.Curves](./Revit.Elements.RevisionCloud.Curves_img.jpg)
