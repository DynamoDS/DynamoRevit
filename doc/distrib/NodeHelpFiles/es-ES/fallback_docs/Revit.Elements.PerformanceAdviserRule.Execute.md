## En detalle:
Este nodo ejecuta una regla específica del asesor de rendimiento en un conjunto de elementos de Revit.

En este ejemplo, la regla del asesor de rendimiento comprueba si "la delimitación de vista está desactivada". Los resultados se transfieren al nodo FailureMessage.FailingElements, que genera los elementos específicos del modelo que no han superado esta comprobación. Este flujo de trabajo facilita a los usuarios el seguimiento y la corrección de los elementos exactos responsables de los problemas.

___
## Archivo de ejemplo

![PerformanceAdviserRule.Execute](./Revit.Elements.PerformanceAdviserRule.Execute_img.jpg)
