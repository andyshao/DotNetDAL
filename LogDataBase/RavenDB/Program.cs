using System;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Raven.Server.Commercial;
using Raven.Server.Config;
using Raven.Server.Config.Settings;
using Raven.Server.ServerWide;
using Raven.Server.Utils;
using Raven.Server.Utils.Cli;
using Sparrow.Logging;

namespace Raven.Server
{
    public class Program
    {
        private static readonly Logger Logger = LoggingSource.Instance.GetLogger<Program>("Raven/Server");

        public static void Main(string[] args)
        {

            string path = string.Concat(Environment.CurrentDirectory, "\\settings.json");
            var configuration = new RavenConfiguration(null, ResourceType.Server, path);

            configuration.Initialize();

            LoggingSource.Instance.SetupLogMode(configuration.Logs.Mode, configuration.Logs.Path.FullPath);
            if (Logger.IsInfoEnabled)
                Logger.Info($"Logging to {configuration.Logs.Path} set to {configuration.Logs.Mode} level.");

            if (Logger.IsOperationsEnabled)
                Logger.Operations(RavenCli.GetInfoText());

            RestartServer = () =>
            {
                ResetServerMre.Set();
                ShutdownServerMre.Set();
            };

            var rerun = false;
            RavenConfiguration configBeforeRestart = configuration;
            do
            {

                try
                {
                    using (var server = new RavenServer(configuration))
                    {
                        try
                        {
                            try
                            {
                                server.OpenPipes();
                            }
                            catch (Exception e)
                            {
                                if (Logger.IsInfoEnabled)
                                    Logger.Info("Unable to OpenPipe. Admin Channel will not be available to the user", e);
                                Console.WriteLine("Warning: Admin Channel is not available");
                            }

                            server.Initialize();

                            var prevColor = Console.ForegroundColor;
                            Console.Write("Server available on: ");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"{server.ServerStore.GetNodeHttpServerUrl()}");
                            Console.ForegroundColor = prevColor;

                            var tcpServerStatus = server.GetTcpServerStatus();
                            prevColor = Console.ForegroundColor;
                            Console.Write("Tcp listening on ");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"{string.Join(", ", tcpServerStatus.Listeners.Select(l => l.LocalEndpoint))}");
                            Console.ForegroundColor = prevColor;

                            IsRunningNonInteractive = false;
                        
                            Console.WriteLine("Server start completed");

                            Console.ReadLine();

                            Console.WriteLine("Starting shut down...");
                            if (Logger.IsInfoEnabled)
                                Logger.Info("Server is shutting down");
                        }
                        catch (Exception e)
                        {
                            if (Logger.IsOperationsEnabled)
                                Logger.Operations("Failed to initialize the server", e);
                            Console.WriteLine(e);

                        }
                    }

                    Console.WriteLine("Shutdown completed");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error during shutdown");
                    Console.WriteLine(e);
                }
                finally
                {
                    if (Logger.IsOperationsEnabled)
                        Logger.OperationsAsync("Server has shut down").Wait(TimeSpan.FromSeconds(15));
                }
            } while (rerun);

        }



        public static ManualResetEvent ShutdownServerMre = new ManualResetEvent(false);
        public static ManualResetEvent ResetServerMre = new ManualResetEvent(false);
        public static Action RestartServer;

        public static bool IsRunningNonInteractive;

        public static bool RunAsNonInteractive()
        {
            IsRunningNonInteractive = true;

            if (Logger.IsInfoEnabled)
                Logger.Info("Server is running as non-interactive.");

            Console.WriteLine("Running non-interactive.");

            if (CommandLineSwitches.LogToConsole)
                LoggingSource.Instance.EnableConsoleLogging();

            AssemblyLoadContext.Default.Unloading += s =>
            {
                LoggingSource.Instance.DisableConsoleLogging();
                if (ShutdownServerMre.WaitOne(0))
                    return; // already done
                Console.WriteLine("Received graceful exit request...");
                ShutdownServerMre.Set();
            };

            ShutdownServerMre.WaitOne();
            if (ResetServerMre.WaitOne(0))
            {
                ResetServerMre.Reset();
                ShutdownServerMre.Reset();
                return true;
            }
            return false;
        }

        private static bool RunInteractive(RavenServer server)
        {
            var configuration = server.Configuration;

            //stop dumping logs
            LoggingSource.Instance.DisableConsoleLogging();

            // set log mode to the previous original mode:
            LoggingSource.Instance.SetupLogMode(configuration.Logs.Mode, configuration.Logs.Path.FullPath);

            return new RavenCli().Start(server, Console.Out, Console.In, true);
        }

        public static void WriteServerStatsAndWaitForEsc(RavenServer server)
        {
            Console.WriteLine("Showing stats, press any key to close...");
            Console.WriteLine("    working set     | native mem      | managed mem     | mmap size         | reqs/sec       | docs (all dbs)");
            var i = 0;
            while (Console.KeyAvailable == false)
            {
                var stats = RavenCli.MemoryStatsWithMemoryMappedInfo();
                var reqCounter = server.Metrics.Requests.RequestsPerSec;

                Console.Write($"\r {((i++ % 2) == 0 ? "*" : "+")} ");

                Console.Write($" {stats.WorkingSet,-14} ");
                Console.Write($" | {stats.TotalUnmanagedAllocations,-14} ");
                Console.Write($" | {stats.ManagedMemory,-14} ");
                Console.Write($" | {stats.TotalMemoryMapped,-17} ");

                Console.Write($"| {Math.Round(reqCounter.OneSecondRate, 1),-14:#,#.#;;0} ");

                long allDocs = 0;
                foreach (var value in server.ServerStore.DatabasesLandlord.DatabasesCache.Values)
                {
                    if (value.Status != TaskStatus.RanToCompletion)
                        continue;

                    try
                    {
                        allDocs += value.Result.DocumentsStorage.GetNumberOfDocuments();
                    }
                    catch (Exception)
                    {
                        // may run out of virtual address space, or just shutdown, etc
                    }
                }

                Console.Write($"| {allDocs,14:#,#.#;;0}      ");

                for (int j = 0; j < 5 && Console.KeyAvailable == false; j++)
                {
                    Thread.Sleep(100);
                }
            }

            Console.ReadKey(true);
            Console.WriteLine();
            Console.WriteLine($"Stats halted.");
        }
    }
}
