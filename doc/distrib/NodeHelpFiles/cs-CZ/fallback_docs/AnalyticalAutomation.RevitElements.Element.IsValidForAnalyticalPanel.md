## Podrobnosti
This node evaluates the selected element to determine if it is suitable for generating an analytical panel. The optional checkOpenings input specifies whether openings within the element should be included in the validity check. When set to true, openings are considered as part of the evaluation.

In this example, the element is defined by its Element ID using the Element.ById node and provided to Element.IsValidForAnalyticalPanel. The outputs include a Boolean indicating if the element is valid and an exception message describing any issues preventing its use.
___
## Vzorový soubor

![Element.IsValidForAnalyticalPanel](./AnalyticalAutomation.RevitElements.Element.IsValidForAnalyticalPanel_img.jpg)
