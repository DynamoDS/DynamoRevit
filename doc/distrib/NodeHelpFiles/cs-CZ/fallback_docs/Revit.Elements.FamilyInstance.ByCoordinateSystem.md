## Podrobnosti
Umožňuje umístit prvek aplikace Revit FamilyInstance do projektu aplikace Revit s požadovaným typem FamilyType a jeho souřadnicovým systémem.

V tomto příkladu je zadán konkrétní typ rodiny a bod souřadnicového systému a je umístěna instance rodiny.
Obvyklý případ použití zahrnuje vytvoření souřadnicového systému založeného na Revit základním bodu projektu a jeho následné otočení tak, aby odpovídalo otočení pozemku. Tento transformovaný souřadnicový systém pak můžete vložit do uzlu FamilyInstance.ByCoordinateSystem a umístit instance rodiny do jejich zamýšlených skutečných umístění s ohledem na orientace pozemku.

Další informace o souřadnicových systémech naleznete níže.
https://help.autodesk.com/view/RVT/2025/CSY/?guid=GUID-68611F67-ED48-4659-9C3B-59C5024CE5F2
___
## Vzorový soubor

![FamilyInstance.ByCoordinateSystem](./Revit.Elements.FamilyInstance.ByCoordinateSystem_img.jpg)
