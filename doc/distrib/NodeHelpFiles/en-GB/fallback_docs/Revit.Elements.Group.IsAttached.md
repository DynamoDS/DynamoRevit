## In Depth
Returns whether a given group element in your Revit project is attached to a parent Group.  In simpler terms, it checks if a Group is part of a larger, nested Group. This node is useful when you need to identify and manage Groups that are organized within other Groups. For example, you might use it to:
• Filter Groups: Isolate Groups that are not part of other Groups
• Manage nested Groups: Understand the structure of nested Groups and process them accordingly.
• Control element modification: Avoid making direct changes to elements within a Group that is attached to a parent group, as this might disrupt the parent Group's structure.
• Automate tasks: Dynamically manage and manipulate Groups based on whether they are attached or not.
Essentially, the Group.IsAttached node helps you to understand the relationship between Groups in your Revit model and build Dynamo workflows that account for this hierarchy.

In the example below, all Model Groups are collected from the active Revit document as the input.  The outputs are True of False.  True results state that the Group has attachments (nesting).  False results state that the Group does NOT have attachments (nesting).

___
## Example File

![Group.IsAttached](./Revit.Elements.Group.IsAttached_img.jpg)
