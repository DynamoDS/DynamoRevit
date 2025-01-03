## En detalle:
`FamilyInstance.Location` devuelve un punto de Dynamo para el ejemplar de familia especificado. Si no hay ninguna ubicación, se devuelve un valor nulo. `FamilyInstance.Location` funciona en elementos basados en puntos y no devolverá una ubicación para elementos basados en curvas en Revit como, por ejemplo, un elemento de viga o muro.

En el ejemplo siguiente, se recopilan todos los ejemplares de la familia de puertas en la vista actual del documento actual. A continuación, se devuelven las ubicaciones de las puertas con `FamilyInstance.Location`.

En el caso de este ejemplo, las puertas de muro cortina devuelven un valor nulo. Las ubicaciones de paneles de muro cortina están disponibles a través de nodos de panel de muro cortina.
___
## Archivo de ejemplo

![FamilyInstance.Location](./Revit.Elements.FamilyInstance.Location_img.jpg)
