## 详细
此节点从指定的原点和方向将光线投射到链接的 Revit 模型中，然后跟踪其从链接图元反弹的连续反弹。每次反弹都表示光线与链接模型中的几何图形相交的一个点，最多达到定义的最大反射次数。

在此示例中，选择链接的图元，并将该图元的位置与方向、最大反弹数和视图一起用作“LinkElement.ByRayBounce”的原点输入。输出是点和链接的图元。
___
## 示例文件

![LinkElement.ByRayBounce](./Revit.Elements.LinkElement.ByRayBounce_img.jpg)
