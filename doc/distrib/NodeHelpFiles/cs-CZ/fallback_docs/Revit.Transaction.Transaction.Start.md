## Podrobnosti
Uzel `Transaction.Start` spustí transakci v aktuálním dokumentu aplikace Revit. Transakce se používají k seskupení změn provedených v modelu aplikace Revit, aby je bylo možné společně potvrdit nebo vrátit zpět. Uzel `Transaction.Start` se obvykle používá s uzlem `Transaction.End`, pokud pracovní postup aplikace Dynamo vyžaduje explicitní řízení toho, kdy změny dokumentu aplikace Revit začínají a kdy se dokončují.

V následujícím příkladu se vytvoří úroveň a použije se jako vstup pro uzel `Transaction.Start`. Tím se spustí transakce aplikace Revit před provedením změn modelu. Po dokončení požadovaných operací aplikace Revit uzel `Transaction.End` uzavře transakci a potvrdí změny v dokumentu. Výstupem je objekt transakce, který lze předat jiným uzlům souvisejícím s transakcí.
___
## Vzorový soubor

![Transaction.Start](./Revit.Transaction.Transaction.Start_img.jpg)
