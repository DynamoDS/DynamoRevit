## Informacje szczegółowe
Ten węzeł wyodrębnia element rewizji połączony z określoną chmurką rewizji w programie Revit. Udostępnia on dane rewizji skojarzone z tą chmurką, umożliwiając użytkownikom sprawdzanie, śledzenie i weryfikowanie szczegółów rewizji w ramach projektu.

W tym przykładzie zostaje utworzony prostokąt za pomocą suwaków Number Slider określających szerokość i długość, a następnie zostaje on rozbity na krzywe i odwrócony w celu uzyskania odpowiedniej orientacji. Te krzywe, wraz z wybranym widokiem (L1_SD) i wybraną rewizją (Seq. 2 – Not For Construction), są używane do wygenerowania chmurki rewizji za pomocą węzła RevisionCloud.ByCurve. Wynikowa chmurka rewizji jest połączona z węzłem RevisionCloud.Revision, który pobiera i zwraca rewizję skojarzoną z tą chmurką. Dzięki temu użytkownicy mogą potwierdzić, która rewizja jest powiązana z daną chmurką rewizji.
___
## Plik przykładowy

![RevisionCloud.Revision](./Revit.Elements.RevisionCloud.Revision_img.jpg)
