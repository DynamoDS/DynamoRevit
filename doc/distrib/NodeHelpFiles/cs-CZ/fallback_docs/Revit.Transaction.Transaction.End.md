## Podrobnosti
Uzel `Transaction.End` dokončí aktuální transakci aplikace Dynamo a vrátí zadaný prvek. V aplikaci Revit transakce představují změny provedené v dokumentu aplikace Revit. Jakmile dojde ke změně, lze ji vidět pomocí povoleného tlačítka Zpět. Pomocí uzlu `Transaction.End` mohou uživatelé přidávat kroky do grafu aplikace Dynamo a vytvářet akci vrácení zpět pro každý krok, kde se použije uzel `Transaction.End`.

V následujícím příkladu je do dokumentu aplikace Revit umístěna instance rodiny. K dokončení umístění před otočením instance rodiny pomocí uzlu `FamilyInstance.SetRotation` se zavolá uzel `Transaction.End`.

___
## Vzorový soubor

![Transaction.End](./Revit.Transaction.Transaction.End_img.jpg)
