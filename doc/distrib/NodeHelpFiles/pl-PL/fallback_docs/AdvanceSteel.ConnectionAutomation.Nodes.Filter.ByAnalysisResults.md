## Informacje szczegółowe
Ten węzeł filtruje listę węzłów połączeń (ConnectionNodes), sprawdzając, czy wartość siły o określonym indeksie mieści się w zdefiniowanym zakresie. Dane siły pochodzą z wyników analizy konstrukcji lub z modelu analitycznego programu Revit i są filtrowane według wybranego typu wyniku (np. Fx, Fy, Fz, Mx, My, Mz).

W tym przykładzie zestaw elementów słupa jest wybierany i oceniany na podstawie komponentu siły Fz przy użyciu wybranego wyniku analizy i przypadku obciążenia. Tylko te elementy, których wartość Fz mieści się w określonym zakresie sił, są zwracane jako akceptowane połączenia.
___
## Plik przykładowy

![Filter.ByAnalysisResults](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.ByAnalysisResults_img.jpg)
