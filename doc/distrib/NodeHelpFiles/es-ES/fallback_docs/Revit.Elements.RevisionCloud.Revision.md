## En detalle:
Este nodo extrae el elemento de revisión vinculado a una nube de revisión específica en Revit. Proporciona los datos de revisión asociados con esa nube, lo que permite a los usuarios comprobar, rastrear o validar los detalles de revisión mediante programación dentro de su proyecto.

En este ejemplo, se crea un rectángulo con controles deslizantes numéricos para la anchura y la longitud y, a continuación, se descompone en curvas y se invierte para obtener la orientación correcta. Estas curvas, junto con la vista elegida (L1_SD) y la revisión seleccionada (Seq. 2 – Not For Construction), se utilizan para generar una nube de revisión mediante el nodo RevisionCloud.ByCurve. La nube de revisión resultante se conecta al nodo RevisionCloud.Revision, que recupera y genera la revisión asociada a esa nube. De este modo, se garantiza que los usuarios puedan confirmar qué revisión está vinculada a cada nube de revisión.
___
## Archivo de ejemplo

![RevisionCloud.Revision](./Revit.Elements.RevisionCloud.Revision_img.jpg)
