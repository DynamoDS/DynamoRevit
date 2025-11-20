## Informacje szczegółowe
This node takes a numeric length value and a unit type identifier, converting the input value into the active Revit project’s length units. The output is a double value representing the converted result.

In this example, a number slider provides a length value, and a unit (for example, Meters) is selected to obtain its Unit.TypeId string. Both are connected to the UnitsUtilities.ConvertToCurrentProjectLengthUnit node, which returns the converted length value based on the project’s unit settings.
___
## Plik przykładowy

![UnitsUtilities.ConvertToCurrentProjectLengthUnit](./AnalyticalAutomation.Utilities.UnitsUtilities.ConvertToCurrentProjectLengthUnit_img.jpg)
