using DesignAutomationFramework;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using System.Diagnostics;
using System.Runtime.Loader;
using RevitServices.Persistence;
using RevitServices.Elements;

namespace DADynamoApp
{
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class DAEntrypoint : IExternalDBApplication
    {
        public static List<IUpdater> Updaters = new List<IUpdater>();
        public ExternalDBApplicationResult OnShutdown(ControlledApplication application)
        {
            AppDomain.CurrentDomain.AssemblyResolve -= DynamoPathResolver.ResolveAssembly;
            return ExternalDBApplicationResult.Succeeded;
        }


        private static void CurrentDomain_ProcessExit(object? sender, EventArgs e)
        {
            Process proc = Process.GetCurrentProcess();
            Console.WriteLine($"Dynamo exiting with Peak physical memory {proc.PeakWorkingSet64} bytes");
            if (proc.HasExited)
            {
                Console.WriteLine($"Dynamo exiting with code {proc.ExitCode}");
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine($"Unhandled exception: {e}");
        }


        public ExternalDBApplicationResult OnStartup(ControlledApplication application)
        {
            AppDomain.CurrentDomain.AssemblyResolve += DynamoPathResolver.ResolveAssembly;
            AssemblyLoadContext.Default.Resolving += DynamoPathResolver.Default_Resolving;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

            Console.WriteLine("<<!>> Starting to load D4DA");

            Console.WriteLine("Loaded assemblies:");
            foreach (var item in AppDomain.CurrentDomain.GetAssemblies())
            {
                var name = item?.GetName()?.Name;
                if (name.Contains("Revit", StringComparison.OrdinalIgnoreCase) || item.Location.Contains("Revit", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine(name);
                }

            }

            try
            {
                RevitServices.Transactions.TransactionManager.SetupManager(new RevitServices.Transactions.AutomaticTransactionStrategy());
                ElementBinder.IsEnabled = true;

                RevitServicesUpdater.Initialize(Updaters);

                DesignAutomationBridge.DesignAutomationReadyEvent += DesignAutomationBridge_DesignAutomationReadyEvent;

                Console.WriteLine("<<!>> D4DA Loaded");

                return ExternalDBApplicationResult.Succeeded;
            }
            catch (Exception ex)
            {
                return ExternalDBApplicationResult.Failed;
            }
        }

        private void DesignAutomationBridge_DesignAutomationReadyEvent(object? sender, DesignAutomationReadyEventArgs e)
        {
            var da = new DynamoADApp();
            da.HandleDesignAutomationReadyEvent(sender, e);
        }
    }
}