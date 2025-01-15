## Informacje szczegółowe
Węzeł `Space.IsPointInsideSpace` sprawdza, czy dany punkt znajduje się wewnątrz danej przestrzeni. Może to być przydatne podczas przypisywania wartości znaczników do elementów w programie Revit.

W poniższym przykładzie pobierane są wszystkie urządzenia nawiewne w danym widoku w bieżącym dokumencie programu Revit. Ich lokalizacje punktów są następnie porównywane z przestrzeniami w danym widoku za pomocą węzła `Space.IsPointInsideSpace`. Przy użyciu zarządzania listami opracowywane są listy podrzędne na potrzeby odfiltrowania urządzeń nawiewnych, które występują w przestrzeniach. Aby uzyskać więcej informacji na temat korzystania z funkcji List@Level, zobacz ten [artykuł](https://primer2.dynamobim.org/5_essential_nodes_and_concepts/5-4_designing-with-lists/3-lists-of-lists#list-level).
___
## Plik przykładowy

![Space.IsPointInsideSpace](./Revit.Elements.Space.IsPointInsideSpace_img.jpg)
