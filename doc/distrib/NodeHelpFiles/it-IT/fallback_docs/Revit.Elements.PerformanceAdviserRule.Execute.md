## In profondità
Questo nodo esegue una regola di Suggerimenti prestazioni su un gruppo di elementi di Revit.

In questo esempio, la regola di Suggerimenti prestazioni verifica se “View clipping is disabled”. I risultati vengono passati al nodo FailureMessage.FailingElements, che restituisce gli elementi specifici del modello che non hanno superato questo controllo. Questo workflow consente agli utenti di rintracciare e correggere più facilmente gli elementi responsabili dei problemi.

___
## File di esempio

![PerformanceAdviserRule.Execute](./Revit.Elements.PerformanceAdviserRule.Execute_img.jpg)
