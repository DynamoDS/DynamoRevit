## 深入資訊
This node retrieves the Zone Type for a given Advance Steel Zone element.  In Advance Steel’s Connection Automation framework, zones define distinct regions of a steel member (such as the start, body, or end) where connection features can be applied.  The Zone Type identifies which of these regions a particular zone represents, essential when automating joint placement, cutbacks, or stiffener locations.

In this example, several elements are selected, the structural data is extracted, the connection node are exposed and used to identify all the zones that apply to that connection node.  These zones are the input to Zone.ZoneType node where the output defines the region of the steel member (start, body, or end).
___
## 範例檔案

![Zone.ZoneType](./AdvanceSteel.ConnectionAutomation.Nodes.Zone.ZoneType_img.jpg)
