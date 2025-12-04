## 详细
此节点针对一组 Revit 图元运行特定的 Performance Adviser 规则。

在此示例中，Performance Adviser 规则检查是否“视图剪裁已禁用”。结果将传递到 FailureMessage.FailingElements 节点，该节点输出模型中未通过此检查的特定图元。此工作流使用户可以更轻松地跟踪和修复导致问题的确切图元。

___
## 示例文件

![PerformanceAdviserRule.Execute](./Revit.Elements.PerformanceAdviserRule.Execute_img.jpg)
