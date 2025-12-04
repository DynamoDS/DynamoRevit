## In profondità
Questo nodo recupera una regola di Suggerimenti prestazioni specifica da Revit mediante il relativo ID regola univoco.

In questo esempio, viene selezionata la regola “Project contains unused families and types” e viene estratto il relativo RuleId. Tale ID viene convertito in una stringa e quindi passato nuovamente in PerformanceAdviserRule.ById, che identifica nuovamente e recupera la stessa regola per un ulteriore utilizzo o automazione.
___
## File di esempio

![PerformanceAdviserRule.ById](./Revit.Elements.PerformanceAdviserRule.ById_img.jpg)
