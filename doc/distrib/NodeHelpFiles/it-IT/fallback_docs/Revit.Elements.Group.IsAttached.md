## In profondità
Indica se un determinato elemento del gruppo nel progetto di Revit è associato ad un gruppo principale. In termini più semplici, controlla se un gruppo fa parte di un gruppo nidificato più grande. Questo nodo è utile quando è necessario identificare e gestire gruppi organizzati all'interno di altri gruppi. È ad esempio possibile utilizzarlo per:
• Filtrare i gruppi: isolare i gruppi che non fanno parte di altri gruppi.
• Gestire i gruppi nidificati: comprendere la struttura dei gruppi nidificati ed elaborarli di conseguenza.
• Controllare la modifica degli elementi: evitare di apportare modifiche dirette agli elementi all'interno di un gruppo associato ad un gruppo principale, in quanto ciò potrebbe interrompere la struttura del gruppo principale.
• Automatizzare le attività: gestire e manipolare dinamicamente i gruppi in base al fatto che siano associati o meno.
Essenzialmente, il nodo Group.IsAttached aiuta a comprendere la relazione tra i gruppi nel modello di Revit e a creare workflow di Dynamo che tengano conto di questa gerarchia.

Nell'esempio seguente, tutti i gruppi di modelli vengono raccolti dal documento di Revit attivo come input. Gli output sono True o False. I risultati True indicano che il gruppo dispone di associazioni (nidificazione). I risultati False indicano che il gruppo NON dispone di associazioni (nidificazione).

___
## File di esempio

![Group.IsAttached](./Revit.Elements.Group.IsAttached_img.jpg)
