## Em profundidade
`Subelement.GetAllParameters` retrieves the internal Revit parameter IDs of subelements, not the parameter display names.

In the example below, all elements in the current view are selected and filtered to show only elements that contain subelements. The subelements are then used as the input to `Subelement.GetAllParameters`. The output is a list of parameter IDs associated with each subelement. The last node shows the output being used to retrieve the parameter values.

___
## Arquivo de exemplo

![Subelement.GetAllParameters](./Revit.Elements.Subelement.GetAllParameters_img.jpg)
