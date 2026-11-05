## En detalle:
`Performance Adviser Rules` recupera las reglas del asesor de rendimiento de Revit disponibles en la sesión actual. Estas reglas constituyen comprobaciones del estado del modelo que Revit puede utilizar para identificar elementos o condiciones que puedan provocar avisos o problemas de rendimiento.

En el ejemplo siguiente, se seleccionan "Muros solapados" y "La línea de separación de habitación no está unida" en el menú desplegable de `PerformanceAdviserRules` y se añaden a una lista. A continuación, esta lista se utiliza como entrada para ejecutar las reglas de rendimiento seleccionadas para el archivo actual, lo que devuelve un conjunto de mensajes de error o resultados. Otras opciones del menú incluyen reglas como "Ejemplares duplicados", "El anfitrión contiene demasiadas inserciones", "El boceto es demasiado complejo" y "El archivo de familia es demasiado grande".
___
## Archivo de ejemplo

![Performance Adviser Rules](./DSRevitNodesUI.PerformanceAdviserRules_img.jpg)
