## 详细
“Space.CenterBoundary”返回表示给定空间的中心线边界的嵌套列表。在返回的列表中，第一个子列表表示最外层的曲线，而后续列表表示空间内的循环。中心边界位于 Revit 空间中横跨所有图层的墙中心线上。有关墙定位线的详细信息，请参见此帮助 [文章](https://help.autodesk.com/view/RVT/2024/CHS/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89)。

如果给定了未定界或未放置的空间，则返回空值。

在下面的示例中，所有空间都收集自当前文档和所选视图。然后返回中心边界。
___
## 示例文件

![Space.CenterBoundary](./Revit.Elements.Space.CenterBoundary_img.jpg)
