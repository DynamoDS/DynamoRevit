## 详细
此节点在 Revit 模型中执行光线反弹分析。它从给定的原点开始沿指定的方向矢量传播，当光线与模型中的图元相交时追踪光线的路径。当光线投射到表面上时，它会根据允许的反弹次数继续从该表面反弹，从而模拟灯光、可见性或路径反射行为。

在此示例中，选择了一个图元，其位置用于节点 RayBounce.ByOriginDirection 的输入原点，以及方向、最大反弹数和视图。
___
## 示例文件

![RayBounce.ByOriginDirection](./Revit.References.RayBounce.ByOriginDirection_img.jpg)
