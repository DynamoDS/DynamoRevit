## En detalle:
De forma similar a `Revit.Elements.FamilyType.ByFamilyAndName`, `Revit.Elements.FamilyType.ByFamilyNameAndTypeName` devuelve la definición del tipo de familia del documento actual (si está disponible). Esto es parecido a `Revit.Elements.FamilyType.ByFamilyAndName`. Sin embargo, en lugar de utilizar una definición de familia, este nodo se basa en la entrada de cadena para ambos valores. Si el tipo de familia no está disponible en el documento actual, se devuelve un valor nulo.

En el ejemplo siguiente, se devuelve un tipo de familia de puertas, 36" x 84", de la familia "Puerta-De_paso-Simple-Paneles_planos".
___
## Archivo de ejemplo

![FamilyType.ByFamilyNameAndTypeName](./Revit.Elements.FamilyType.ByFamilyNameAndTypeName_img.jpg)
