using Mit_Oersted.DummyDataConsole.Models;
using Mit_Oersted.DummyDataConsole.Parsers;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mit_oersted.DummyDataConsole
{
    internal enum ExitCode
    {
        Success = 0,
        UnknownError = 10
    }

    internal class Program
    {
        private static int Main(string[] args)
        {
            bool waitWhenFinished = ArgumentParser.GetArgument(args, "pause", true);
            ExitCode exitCode = ExitCode.Success;

            try
            {
                MainAsync(args).Wait();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UnhandledException");
                exitCode = ExitCode.UnknownError;
            }

            if (waitWhenFinished)
            {
                Console.WriteLine("Please press a key to continue.");
                Console.ReadLine();
            }

            return (int)exitCode;
        }

        private static async Task MainAsync(string[] args)
        {
            InitializeLog(ArgumentParser.GetArgument(args, "debug", false));

            Log.Information($"Executing Mit_oersted.DummyDataConsole '{ Assembly.GetEntryAssembly().GetName().Version }'");

            string jsonFilePath = ReadArgument(args, "jsonFilePath", isRequired: true, promptUserForValue: true);

            var doWork = new Mit_Oersted.DummyDataConsole.Tasks.DoWork();

            DummyDataUserModel[] tmpModel = await doWork.ReadDummyDataTask(jsonFilePath);

            List<object> objects = doWork.SortDummmyDataTask(tmpModel);

            doWork.HandelDummmyDataTask(objects);

            Log.Information("Done updating data.");
        }

        private static string ReadArgument(string[] args, string key, bool isRequired, bool promptUserForValue = false)
        {
            string value = ArgumentParser.GetArgument(args, key, null);

            if (!string.IsNullOrEmpty(value) && Regex.Match(value, @"^\{\{\w+\}\}$").Success) { throw new Exception($"Invalid /{ key }:<value> argument provided: '{ value }'"); }

            if (isRequired && string.IsNullOrEmpty(value))
            {
                if (promptUserForValue)
                {
                    Log.Information($"Please enter a value for '{ key }':");
                    value = ReadValueFromConsole();
                    if (!string.IsNullOrEmpty(value)) return value;
                }

                throw new Exception($"No /{ key }:<value> argument provided");
            }
            else return value;
        }

        private static string ReadValueFromConsole()
        {
            return Console.ReadLine();
        }

        private static void InitializeLog(bool isDebug)
        {
            var path = Path.Combine(Environment.CurrentDirectory, "out", "log");
            if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }

            if (isDebug)
            {
                Log.Logger = new LoggerConfiguration()
                                  .MinimumLevel.Debug()
                                  .WriteTo.Console()
                                  .WriteTo.File($"{ path }\\log_{ DateTime.Now:yyyy-MM-dd_HH-mm}.log")
                                  .CreateLogger();
            }
            else
            {
                Log.Logger = new LoggerConfiguration()
                                  .WriteTo.Console()
                                  .WriteTo.File($"{ path }\\log_{ DateTime.Now:yyyy-MM-dd_HH-mm}.log")
                                  .CreateLogger();
            }
        }
    }
}
