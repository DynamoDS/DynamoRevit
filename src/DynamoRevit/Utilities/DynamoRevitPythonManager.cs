using Dynamo.Utilities;
using RevitServices.Persistence;
using System.Collections;
using System.Collections.Specialized;
using Dynamo.Logging;
using PythonEngine = Dynamo.PythonServices.PythonEngine;
using Dynamo.PythonServices;
using System;
using System.IO;
using System.Linq;

namespace Revit.Elements
{
    internal static class DynamoRevitPythonManager
    {
        private static DynamoLogger Logger;

        private static Dynamo.PythonServices.EventHandlers.EvaluationStartedEventHandler OnPythonEvalStart;

        /// <summary>
        /// Setup the python engine so that it can manage Revit data
        /// </summary>
        /// <param name="engine"></param>
        private static void SetupPythonEngine(PythonEngine engine)
        {
            if (engine != null)
            {
                try
                {
                    var property = engine.GetType().GetProperty("HostDataMarshaler", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (property == null)
                    {
                        throw new Exception("Could not find a valid HostDataMarshaler property");
                    }
       
                    var hostDataMarshaler = property.GetValue(engine);
                    if (hostDataMarshaler is null)
                    {
                        var revitDataMarshaler = new DataMarshaler();
                        revitDataMarshaler.RegisterMarshaler((Revit.Elements.Element element) => element.InternalElement);
                        revitDataMarshaler.RegisterMarshaler((Category element) => element.InternalCategory);
                        property.SetValue(engine, revitDataMarshaler);

                        var method = engine.GetType().GetMethod("RegisterHostDataMarshalers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (method == null)
                        {
                            throw new Exception("Could not find a valid RegisterHostDataMarshalers method");
                        }
                        method.Invoke(engine, null);
                    }

                    (engine.OutputDataMarshaler as DataMarshaler).RegisterMarshaler((Autodesk.Revit.DB.Element element) => element.ToDSType(true));
                    engine.EvaluationFinished += OnPythonEvalFinished;
                    OnPythonEvalStart = (string code, IList bindingValues, Dynamo.PythonServices.EventHandlers.ScopeSetAction scopeSet) =>
                    {
                        hostDataMarshaler = property.GetValue(engine);
                        // Turn off element binding during python script execution
                        ElementBinder.IsEnabled = false;
                        if (hostDataMarshaler != null)
                        {
                            Func<object, object> unwrap = (hostDataMarshaler as DataMarshaler).Marshal;
                            // register UnwrapElement method
                            scopeSet("UnwrapElement", unwrap);
                        }
                    };

                    engine.EvaluationStarted += OnPythonEvalStart;
                }
                catch (FileNotFoundException ex)
                {
                    Logger.Log(ex);
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message + ex.StackTrace);
                    throw;
                }
            }
        }

        /// <summary>
        /// Cleanup all subscribed events and registered marshalers
        /// </summary>
        /// <param name="engine"></param>
        private static void CleanUpPythonEngine(PythonEngine engine)
        {
            try
            {
                if (engine != null)
                {
                    (engine.OutputDataMarshaler as DataMarshaler).UnregisterMarshalerOfType<Element>();
                    engine.EvaluationStarted -= OnPythonEvalStart;
                    engine.EvaluationFinished -= OnPythonEvalFinished;
                }
            }
            catch (FileNotFoundException ex)
            {
                Logger.Log(ex);
            }
        }

        /// <summary>
        /// Sets up new python engines registered in the available PythonEngine collection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnPythonEngineCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Console.WriteLine("Called OnPythonEngineCollectionChanged");
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    SetupPythonEngine(item as PythonEngine);
                }
            }
        }

        // Python evaluation finished event handler
        private static void OnPythonEvalFinished(EvaluationState state, string code, IList bindingValues, Dynamo.PythonServices.EventHandlers.ScopeGetAction scopeGet)
        {
            // Turn on element binding after python script execution
            ElementBinder.IsEnabled = true;
        }

        private static bool setupPython;

        internal static void SetupPython(DynamoLogger logger)
        {
            if (setupPython) return;

            // Setup engines for all existing python engines
            PythonEngineManager.Instance.AvailableEngines.ToList().ForEach(engine => SetupPythonEngine(engine));
            // Setup engines for any python engines that might be registered later on
            PythonEngineManager.Instance.AvailableEngines.CollectionChanged += OnPythonEngineCollectionChanged;

            Logger = logger;
            setupPython = true;
        }

        internal static void Cleanup()
        {
            PythonEngineManager.Instance.AvailableEngines.ToList().ForEach(engine => CleanUpPythonEngine(engine));
            PythonEngineManager.Instance.AvailableEngines.CollectionChanged -= OnPythonEngineCollectionChanged;
        }
    }
}
