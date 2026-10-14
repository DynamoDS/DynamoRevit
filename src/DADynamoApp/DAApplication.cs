
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using System.Diagnostics;
using System.Reflection;

namespace DADynamoApp
{
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class DAApplication : IExternalDBApplication
    {
        private string ParentPath;
        private string CurrentDirectory;
        private readonly string PythonDllFolder = "pythonDependencies";

        private DAEntrypoint daEntryPoint;

        public ExternalDBApplicationResult OnShutdown(ControlledApplication application)
        {
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.ProcessExit -= CurrentDomain_ProcessExit;
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
            return daEntryPoint.OnShutdown(application);
        }

        public ExternalDBApplicationResult OnStartup(ControlledApplication application)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            CurrentDirectory = Directory.GetCurrentDirectory();
            ParentPath = Directory.GetParent(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)).FullName;

            Console.WriteLine("<<!>> Starting to load DAEntrypoint");

            daEntryPoint ??= new DAEntrypoint();

            return daEntryPoint.OnStartup(application);
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

        private Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
        {
            return DynamoRevitAssemblyResolver.ResolveDynamoAssembly(ParentPath, [Path.Combine(CurrentDirectory, PythonDllFolder)], args);
        }
    }
}