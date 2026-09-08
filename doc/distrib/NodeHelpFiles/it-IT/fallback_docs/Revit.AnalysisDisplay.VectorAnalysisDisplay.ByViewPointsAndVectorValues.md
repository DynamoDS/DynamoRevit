## In profondità
`VectorAnalysisDisplay.ByViewPointsAndVectorValues`crea una visualizzazione dell'analisi vettoriale in una vista di Revit. La vista di input definisce la posizione in cui verrà visualizzata l'analisi. I punti definiscono le posizioni dei contrassegni di analisi, i vettori definiscono la direzione dell'analisi visualizzata e i valori definiscono il risultato numerico associato a ciascun punto. L'output è un elemento visualizzazione dell'analisi di Revit mostrato nella vista selezionata.

Nell'esempio seguente, vengono creati 2 punti per definire posizioni di esempio in una vista 3D di Revit. Vengono quindi creati 2 vettori per definire i dati dell'analisi direzionale in tali posizioni. I punti e i vettori vengono combinati in elenchi e utilizzati come input in `VectorAnalysisDisplay.ByViewPointsAndVectorValues` insieme ad una vista, un nome dell'analisi e una descrizione. L'output è una visualizzazione dell'analisi vettoriale mostrata nella vista di Revit selezionata.
___
## File di esempio

![VectorAnalysisDisplay.ByViewPointsAndVectorValues](./Revit.AnalysisDisplay.VectorAnalysisDisplay.ByViewPointsAndVectorValues_img.jpg)
