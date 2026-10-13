## En detalle:
`SpecType.ByTypeId` convierte una cadena de tipo TypeId de especificación de Revit en un objeto `SpecType`. Los tipos de especificación definen el tipo de datos que representa un parámetro en Revit, como longitud, área, ángulo, material, fuerza, texto, número entero y muchos otros. Se utiliza habitualmente al crear parámetros compartidos, parámetros de proyecto o definiciones de parámetros en las versiones más recientes de Revit, en las que Autodesk ha sustituido el antiguo sistema `ParameterType` por los TypeId de Forge.

En el ejemplo siguiente, se proporciona a `SpecType.ByTypeId` una cadena que representa un TypeId de especificación de Revit. El resultado es un objeto `SpecType` que puede utilizarse posteriormente al crear una nueva definición de parámetro para que Revit comprenda el tipo de datos que debe almacenar el parámetro.
___
## Archivo de ejemplo

![SpecType.ByTypeId](./Revit.Elements.SpecType.ByTypeId_img.jpg)
