## En detalle:
This node retrieves the transformation matrix applied to a Revit Link Element within the host model.
In other words, it returns the position, rotation, and scaling transform that maps the linked element’s coordinate system into the host Revit project’s coordinate system.
This is useful when you need to align, analyze, or manipulate geometry between linked models.

In this example, all Revit linked elements at level L3 are selected and input into LinkElement.LinkTransform.  The output is the position, rotation and scaling transform of the linked element.
___
## Archivo de ejemplo

![LinkElement.LinkTransform](./Revit.Elements.LinkElement.LinkTransform_img.jpg)
