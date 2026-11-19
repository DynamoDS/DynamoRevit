using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RevitNodes.Analyzers
{
    /// <summary>
    /// Analyzer that detects usage of types, methods, or properties marked with [NodeObsolete] attribute
    /// from DynamoVisualProgramming dependencies and reports compiler warnings.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NodeObsoleteAnalyzer : DiagnosticAnalyzer
    {
        // Diagnostic IDs
        public const string DiagnosticId = "DN001";
        public const string ErrorDiagnosticId = "DN002";

        // Diagnostic categories
        private const string Category = "Usage";

        // Diagnostic descriptors
        private static readonly DiagnosticDescriptor WarningRule = new DiagnosticDescriptor(
            DiagnosticId,
            "Node is obsolete",
            "{0}",
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "The node marked with [NodeObsolete] should not be used.");

        private static readonly DiagnosticDescriptor ErrorRule = new DiagnosticDescriptor(
            ErrorDiagnosticId,
            "Node is obsolete (error)",
            "{0}",
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "The node marked with [NodeObsolete] should not be used.");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(WarningRule, ErrorRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.IdentifierName);
            context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
            context.RegisterSyntaxNodeAction(AnalyzeMemberAccess, SyntaxKind.SimpleMemberAccessExpression);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var identifierName = (IdentifierNameSyntax)context.Node;

            // Get the symbol for the identifier
            var symbolInfo = context.SemanticModel.GetSymbolInfo(identifierName, context.CancellationToken);
            if (symbolInfo.Symbol == null)
                return;

            CheckAndReportObsolete(symbolInfo.Symbol, identifierName.GetLocation(), context);
        }

        private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;
            var symbolInfo = context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken);
            
            if (symbolInfo.Symbol != null)
            {
                CheckAndReportObsolete(symbolInfo.Symbol, invocation.GetLocation(), context);
            }
        }

        private static void AnalyzeMemberAccess(SyntaxNodeAnalysisContext context)
        {
            var memberAccess = (MemberAccessExpressionSyntax)context.Node;
            var symbolInfo = context.SemanticModel.GetSymbolInfo(memberAccess, context.CancellationToken);
            
            if (symbolInfo.Symbol != null)
            {
                CheckAndReportObsolete(symbolInfo.Symbol, memberAccess.Name.GetLocation(), context);
            }
        }

        private static void CheckAndReportObsolete(ISymbol symbol, Location location, SyntaxNodeAnalysisContext context)
        {
            var nodeObsoleteAttribute = GetNodeObsoleteAttribute(symbol, context.Compilation);
            if (nodeObsoleteAttribute == null)
                return;

            var message = GetObsoleteMessage(nodeObsoleteAttribute);
            var isError = GetIsError(nodeObsoleteAttribute);
            var symbolName = GetSymbolDisplayName(symbol);

            var diagnosticMessage = string.IsNullOrEmpty(message)
                ? $"{symbolName} is obsolete."
                : $"{symbolName} is obsolete: {message}";

            var rule = isError ? ErrorRule : WarningRule;
            var diagnostic = Diagnostic.Create(
                rule,
                location,
                diagnosticMessage);

            context.ReportDiagnostic(diagnostic);
        }

        private static INamedTypeSymbol GetNodeObsoleteAttributeType(Compilation compilation)
        {
            // Try multiple possible namespaces where NodeObsolete might be defined
            var possibleNames = new[]
            {
                "Autodesk.DesignScript.Runtime.NodeObsoleteAttribute",
                "Autodesk.DesignScript.Runtime.NodeObsolete",
                "Dynamo.Graph.Nodes.NodeObsoleteAttribute",
                "Dynamo.Graph.Nodes.NodeObsolete",
                "NodeObsoleteAttribute",
                "NodeObsolete"
            };

            foreach (var name in possibleNames)
            {
                var type = compilation.GetTypeByMetadataName(name);
                if (type != null)
                    return type;
            }

            return null;
        }

        private static AttributeData GetNodeObsoleteAttribute(ISymbol symbol, Compilation compilation)
        {
            var nodeObsoleteAttributeType = GetNodeObsoleteAttributeType(compilation);
            if (nodeObsoleteAttributeType == null)
                return null;

            // Check the symbol itself
            foreach (var attribute in symbol.GetAttributes())
            {
                if (IsNodeObsoleteAttribute(attribute, nodeObsoleteAttributeType))
                {
                    return attribute;
                }
            }

            // Check the containing type if symbol is a method or property
            if (symbol.ContainingType != null)
            {
                foreach (var attribute in symbol.ContainingType.GetAttributes())
                {
                    if (IsNodeObsoleteAttribute(attribute, nodeObsoleteAttributeType))
                    {
                        return attribute;
                    }
                }
            }

            // Check base types/interfaces for inherited attributes
            if (symbol.ContainingType != null)
            {
                var baseType = symbol.ContainingType.BaseType;
                while (baseType != null)
                {
                    foreach (var attribute in baseType.GetAttributes())
                    {
                        if (IsNodeObsoleteAttribute(attribute, nodeObsoleteAttributeType))
                        {
                            return attribute;
                        }
                    }
                    baseType = baseType.BaseType;
                }
            }

            return null;
        }

        private static bool IsNodeObsoleteAttribute(AttributeData attribute, INamedTypeSymbol expectedType)
        {
            if (attribute.AttributeClass == null)
                return false;

            // Direct type match
            if (SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, expectedType))
                return true;

            // Name match (handles cases where the type might be from a different assembly)
            if (attribute.AttributeClass.Name == "NodeObsoleteAttribute" || 
                attribute.AttributeClass.Name == "NodeObsolete")
                return true;

            // Check if it derives from or implements the expected type
            var currentType = attribute.AttributeClass;
            while (currentType != null)
            {
                if (SymbolEqualityComparer.Default.Equals(currentType, expectedType))
                    return true;
                currentType = currentType.BaseType;
            }

            return false;
        }

        private static string GetObsoleteMessage(AttributeData attribute)
        {
            // Check constructor arguments first
            if (attribute.ConstructorArguments.Length > 0)
            {
                var arg = attribute.ConstructorArguments[0];
                if (arg.Kind == TypedConstantKind.Primitive && arg.Value is string message)
                {
                    return message;
                }
            }

            // Check named arguments for Message property
            foreach (var namedArg in attribute.NamedArguments)
            {
                if (namedArg.Key == "Message" && namedArg.Value.Kind == TypedConstantKind.Primitive)
                {
                    if (namedArg.Value.Value is string message)
                    {
                        return message;
                    }
                }
            }

            return string.Empty;
        }

        private static bool GetIsError(AttributeData attribute)
        {
            // Check named arguments for IsError property
            foreach (var namedArg in attribute.NamedArguments)
            {
                if (namedArg.Key == "IsError" && namedArg.Value.Kind == TypedConstantKind.Primitive)
                {
                    if (namedArg.Value.Value is bool isError)
                    {
                        return isError;
                    }
                }
            }

            return false;
        }

        private static string GetSymbolDisplayName(ISymbol symbol)
        {
            // Use the symbol's display format for better readability
            if (symbol is IMethodSymbol method)
            {
                return $"{method.ContainingType?.Name ?? ""}.{method.Name}";
            }
            if (symbol is IPropertySymbol property)
            {
                return $"{property.ContainingType?.Name ?? ""}.{property.Name}";
            }
            if (symbol is IFieldSymbol field)
            {
                return $"{field.ContainingType?.Name ?? ""}.{field.Name}";
            }
            if (symbol is INamedTypeSymbol type)
            {
                return type.Name;
            }

            return symbol.Name;
        }
    }
}

