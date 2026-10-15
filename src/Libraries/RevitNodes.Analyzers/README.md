# NodeObsolete Analyzer

This Roslyn analyzer detects compile-time usage of types, methods, or properties marked with the `[NodeObsolete]` attribute from DynamoVisualProgramming dependencies.

## Purpose

When DynamoVisualProgramming marks APIs as obsolete using the `[NodeObsolete]` attribute, this analyzer ensures that your codebase gets compiler warnings (or errors) when you use those obsolete APIs, helping you migrate to newer alternatives before they're removed.

## How It Works

The analyzer:
1. Scans your code during compilation
2. Detects when you use symbols (classes, methods, properties) that have the `[NodeObsolete]` attribute
3. The attribute can be defined in external assemblies (like DynamoVisualProgramming packages)
4. Reports warnings or errors at the usage location

## Diagnostic IDs

- **DN001**: Warning when using a node/method/property marked with `[NodeObsolete]`
- **DN002**: Error when using a node/method/property marked with `[NodeObsolete(IsError = true)]`

## Integration

The analyzer is automatically included when you build the `RevitNodes` project. It will analyze all code that references DynamoVisualProgramming assemblies.

## Example

If DynamoVisualProgramming has:

```csharp
[NodeObsolete("Use NewMethod instead")]
public static void OldMethod() { }
```

And your code uses it:

```csharp
OldMethod(); // This will generate a compiler warning: "OldMethod is obsolete: Use NewMethod instead"
```

## Supported Usage Patterns

The analyzer detects:
- Direct type references: `var x = new ObsoleteType();`
- Method invocations: `ObsoleteMethod();`
- Property access: `var x = ObsoleteProperty;`
- Member access: `obj.ObsoleteMember`

## Attribute Detection

The analyzer searches for `NodeObsolete` attributes in these namespaces:
- `Autodesk.DesignScript.Runtime.NodeObsoleteAttribute`
- `Autodesk.DesignScript.Runtime.NodeObsolete`
- `Dynamo.Graph.Nodes.NodeObsoleteAttribute`
- `Dynamo.Graph.Nodes.NodeObsolete`
- And other common locations

It also checks:
- Attributes on the symbol itself
- Attributes on containing types (for methods/properties)
- Attributes on base types (inherited attributes)

