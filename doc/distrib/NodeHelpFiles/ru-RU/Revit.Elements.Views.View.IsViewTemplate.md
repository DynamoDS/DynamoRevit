## In Depth
`View.IsViewTemplate` determines if the input view is a view template. In Revit, a view template is technically a view and `View.IsViewTemplate` allows for the filtering of these views.

In the example below, all plan views are collected from the current Revit file. The plan views are then separated into 2 lists, view templates and plan views.
___
## Example File

![View.IsViewTemplate](./Revit.Elements.Views.View.IsViewTemplate_img.jpg)