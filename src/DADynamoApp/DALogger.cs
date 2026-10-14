using Dynamo.Configuration;
using Dynamo.Graph.Nodes;
using Dynamo.Logging;
using ProtoCore;
using System.Reflection;
using System.Text.Json;

namespace DADynamoApp;

/// <summary>
/// Logger for DynamoRevit Design Automation runs. Writes all log messages to stdout
/// with a UTC timestamp. Also provides node output serialization for profiling.
/// </summary>
public class DALogger : DynamoLogger
{
    // Cached reflection members for Player's WatchNodeHandler methods
    private static MethodInfo? getNodeValueMethod;
    private static MethodInfo? processWatchTreeMethod;

    private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault
    };

    public DALogger(string logDirectory)
        : base(new DebugSettings(), logDirectory, false, true, false)
    {
        Console.WriteLine($"DALogger initialized. LogPath: {this.LogPath}");
    }

    protected override void Log(string message, LogLevel level, bool reportModification)
    {
        Console.WriteLine($"{DateTime.UtcNow:u} : {message}");
        base.Log(message, level, reportModification);
    }

    /// <summary>
    /// Serializes node output values by getting data from the runtime mirror
    /// and using Player's serialization logic via reflection.
    /// </summary>
    public string SerializeNodeOutputs(NodeModel node, Dynamo.Engine.EngineController engineController, int maxLength = 2000)
    {
        try
        {
            var variableName = node.AstIdentifierForPreview.Value;
            if (string.IsNullOrEmpty(variableName)) return string.Empty;

            var runtimeMirror = engineController.GetMirror(variableName);
            if (runtimeMirror == null) return string.Empty;

            var mirrorData = runtimeMirror.GetData();
            if (mirrorData == null) return string.Empty;

            var valueContainer = GetNodeValueStringFromMirrorData(mirrorData);
            if (valueContainer == null) return string.Empty;

            var valueString = JsonSerializer.Serialize(valueContainer, jsonOptions);
            if (string.IsNullOrEmpty(valueString)) return string.Empty;

            if (valueString.Length > maxLength)
                valueString = valueString.Substring(0, maxLength) + $"... (truncated from {valueString.Length} chars)";

            return valueString;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error serializing node outputs: {ex}");
            return string.Empty;
        }
    }

    private static object? GetNodeValueStringFromMirrorData(object mirrorData)
    {
        try
        {
            var obj = GetNodeValueViaReflection(mirrorData);
            if (obj == null) return null;

            if (obj is System.Collections.IList list && list.Count == 0)
                return new { Value = "Empty List" };

            if (obj is System.Collections.IDictionary dict && dict.Count == 0)
                return new { Value = "Empty Dictionary" };

            return ProcessWatchTreeViaReflection(obj);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetNodeValueStringFromMirrorData: {ex}");
            return null;
        }
    }

    private static object? GetNodeValueViaReflection(object mirrorData)
    {
        if (getNodeValueMethod == null)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "DynamoPlayer.Workflows")
                ?? throw new InvalidOperationException("DynamoPlayer.Workflows assembly not found");

            var type = assembly.GetType("DynamoPlayer.WatchNodeHandler")
                ?? throw new InvalidOperationException("WatchNodeHandler type not found");

            getNodeValueMethod = type.GetMethod("GetNodeValue", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                ?? throw new InvalidOperationException("GetNodeValue method not found");
        }

        try
        {
            return getNodeValueMethod.Invoke(null, [mirrorData]);
        }
        catch (TargetInvocationException ex)
        {
            throw new InvalidOperationException($"GetNodeValue invocation failed: {ex.InnerException?.Message}", ex.InnerException);
        }
    }

    private static object? ProcessWatchTreeViaReflection(object obj)
    {
        if (processWatchTreeMethod == null)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "DynamoPlayer.Workflows")
                ?? throw new InvalidOperationException("DynamoPlayer.Workflows assembly not found");

            var type = assembly.GetType("DynamoPlayer.WatchNodeHandler")
                ?? throw new InvalidOperationException("WatchNodeHandler type not found");

            processWatchTreeMethod = type.GetMethod("ProcessWatchTree", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                ?? throw new InvalidOperationException("ProcessWatchTree method not found");
        }

        try
        {
            return processWatchTreeMethod.Invoke(null, [obj, null]);
        }
        catch (TargetInvocationException ex)
        {
            throw new InvalidOperationException($"ProcessWatchTree invocation failed: {ex.InnerException?.Message}", ex.InnerException);
        }
    }

    /// <summary>
    /// Gets runtime warnings for a specific node as a JSON string.
    /// </summary>
    internal static string GetNodeMessages(RuntimeStatus runtimeStatus, Guid nodeId)
    {
        var warnings = new List<string>();

        foreach (var warning in runtimeStatus.Warnings)
        {
            if (warning.GraphNodeGuid == nodeId)
                warnings.Add(warning.Message);
        }

        return JsonSerializer.Serialize(new { warnings = warnings.Count > 0 ? warnings : null }, jsonOptions);
    }
}
