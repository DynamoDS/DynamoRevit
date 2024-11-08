## Informacje szczegółowe
Każdy widok w programie Revit ma początek. Węzeł `View.Origin` zwraca tę wartość jako punkt Dynamo. Zgodnie z dokumentacją interfejsu API programu Revit początek rzutu nie jest znaczący („The origin of a plan view is not meaningful”). W tym kontekście wartość udostępniana przez węzeł `View.Origin` zależy od użytkownika końcowego.

W poniższym przykładzie zwracany jest początek aktywnego widoku i wybranego widoku 3D.
___
## Plik przykładowy

![View.Origin](./Revit.Elements.Views.View.Origin_img.jpg)
