## Em profundidade
Esse nó recupera o plano de referência associado a um determinado elemento do plano do esboço. Isso ajuda a identificar ou reusar o mesmo plano de referência para criar ou modificar a geometria.

Neste exemplo, um plano é definido e, em seguida, conectado ao nó SketchPlane.ByPlane, que gera um plano de esboço correspondente. Esse plano de esboço é usado como uma entrada para SketchPlane.ElementPlaneReference, em que a saída pode ser usada para cotas, alinhamentos, restrições ou outras operações que requerem uma referência do Revit.

___
## Arquivo de exemplo

![SketchPlane.ElementPlaneReference](./Revit.Elements.SketchPlane.ElementPlaneReference_img.jpg)
