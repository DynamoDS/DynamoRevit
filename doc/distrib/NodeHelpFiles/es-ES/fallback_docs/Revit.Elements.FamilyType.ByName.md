## En detalle:
`FamilyType.ByName` intentará recuperar el tipo de familia especificado del nombre indicado en el documento actual. Si el tipo de familia no está disponible en el documento actual, se devolverá un valor nulo.

Nota: `FamilyType.ByName` busca las definiciones de tipo de familia en orden de creación de la familia principal (por ID de elemento). Si varias familias principales presentan una definición de tipo con el mismo nombre, se devolverá la primera que se encuentre. Para obtener una búsqueda más concisa de tipos de familia, utilice `FamilyType.ByFamilyAndName` o `FamilyType.ByFamilyNameAndTypeName`.

En el ejemplo siguiente, se devuelve un tipo de familia de puertas, 36" x 84", de la familia "Puerta-De_paso-Simple-Paneles_planos".
___
## Archivo de ejemplo

![FamilyType.ByName](./Revit.Elements.FamilyType.ByName_img.jpg)
