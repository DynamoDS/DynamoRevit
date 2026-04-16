## In profondità
Questo nodo filtra un elenco di ConnectionNodes verificando se il valore della forza in corrispondenza di un indice specificato rientra in un intervallo definito. I dati della forza provengono dai risultati dell'analisi strutturale o dal modello analitico di Revit e vengono filtrati in base al tipo di risultato selezionato (ad esempio, Fx, Fy, Fz, Mx, My, Mz).

In questo esempio, viene selezionato un gruppo di elementi pilastro e poi valutato in base al componente della forza Fz, utilizzando il risultato dell'analisi e la condizione di carico scelti. Solo gli elementi il cui valore Fz rientra nell'intervallo di forza specificato vengono restituiti come connessioni accettate.
___
## File di esempio

![Filter.ByAnalysisResults](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.ByAnalysisResults_img.jpg)
