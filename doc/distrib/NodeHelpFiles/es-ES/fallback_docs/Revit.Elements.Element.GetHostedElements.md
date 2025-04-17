## En detalle:
Si se ha especificado un elemento que admite el alojamiento de elementos (por ejemplo, muros), `Element.GetHostedElements` devuelve los elementos que dependen del elemento especificado. Por defecto, se devuelven los ejemplares de familia alojados en el elemento. `Element.GetHostedElements` ofrece la opción de incluir otros tipos de elementos alojados.

Entre estos, se incluyen los siguientes:
- Huecos
- Elementos alojados en anfitriones unidos
- Muros incrustados (por ejemplo, muros cortina)
- Inserciones incrustadas compartidas

En el ejemplo siguiente, se recopilan todos los elementos de muro de L2. A continuación, se comprueba si los elementos de muro están alojados con `Element.GetHostedElements`. Posteriormente, esta lista se utiliza para crear dos listas, Muros con puertas y Muros sin puertas.
___
## Archivo de ejemplo

![Element.GetHostedElements](./Revit.Elements.Element.GetHostedElements_img.jpg)
