using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Dynamo.Configuration;
using Dynamo.Models;
using Dynamo.Storage.Upload;
using DynamoShapeManager;
using Greg;

namespace Reach.WebSocket
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var options = new Options();
            CommandLine.Parser.Default.ParseArguments(args, options);

            var geometryFactoryPath = string.Empty;
            var preloaderLocation = string.Empty;

            PreloadShapeManager(ref geometryFactoryPath, ref preloaderLocation, options.LoadShapeManagerFromRoot);

            var pathResolver = new ReachPathResolver(preloaderLocation);

            if (!options.Verbose)
            {
                // Hide all output
                Console.SetOut(TextWriter.Null);
                Console.SetError(TextWriter.Null);
            }

            Func<DynamoModel> rdm = () =>
            {
                var model = DynamoModel.Start(
                    new Reach.StartConfiguration()
                    {
                        IsHeadless = true,
                        PathResolver = pathResolver,
                        GeometryFactoryPath = geometryFactoryPath,
                        Preferences = new PreferenceSettings()
                        {
                            IsAnalyticsReportingApproved = false,
                            IsUsageReportingApproved = false,
                            IsFirstRun = false,
                            CustomPackageFolders = new List<string>() {Whitelist.WhitelistPackageDirectory}
                        }
                    });

                return model;
            };

            var h = new EventWaitHandle(false, EventResetMode.ManualReset);
            var startParams = new WebSocketServer.StartParams()
            {
                DynamoModelSource = rdm,
                WaitHandle = options.AutoExit ? h : null,
                TokenValidatorAddress = options.TokenValidatorAddress,
                DisableTokenValidator = options.DisableTokenValidator
            };

            Whitelist.UpdateWhitelist(NodeFilter.GetPackageManagerUrl());

            using (var ws = new WebSocketServer(startParams))
            {
                h.WaitOne();
            }

            // this allows a daemon to know that autoexit is
            // the reason the process exited, even if the system
            // is exiting smoothly
            if (options.AutoExit)
            {
                return 1;
            }

            return 0;
        }

        private static bool IsLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 128);
            }
        }

        private static void PreloadShapeManager(ref string geometryFactoryPath, ref string preloaderLocation, bool loadShapeManagerFromRoot = false)
        {
            // note: if running on Linux, the ProtoGeometry.config file must be properly set
            //       to discover LibG.ProtoInterface.dll and the LibG & ASM shared libraries must
            //       be discoverable
            // internally searches for installed autodesk product
            if (IsLinux) return;

            var exePath = Assembly.GetExecutingAssembly().Location;
            var rootFolder = Path.GetDirectoryName(exePath);

            var versions = new[]
            {
               LibraryVersion.Version223, LibraryVersion.Version222, LibraryVersion.Version221
            };

            var preloader = loadShapeManagerFromRoot? 
                new Preloader(rootFolder, rootFolder, LibraryVersion.Version223) : 
                new Preloader(rootFolder, versions);

            preloader.Preload();
            geometryFactoryPath = preloader.GeometryFactoryPath;
            preloaderLocation = preloader.PreloaderLocation;
        }
    }
}
