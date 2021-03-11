using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using SerilogWeb.Classic.Enrichers;
using System;

namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var applicationName = "Mit-Oersted";
                var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown";

                Log.Logger = new LoggerConfiguration()
#if DEBUG
                                .MinimumLevel.Debug()
#else
                                .MinimumLevel.Information()
#endif
                                .Enrich.WithMachineName()
                    .Enrich.With<HttpRequestIdEnricher>()
                    .Enrich.With<HttpRequestRawUrlEnricher>()
                    .Enrich.With<HttpRequestTypeEnricher>()
                    .Enrich.With<HttpRequestUrlReferrerEnricher>()
                    .Enrich.With<HttpRequestUrlEnricher>()
                    .Enrich.With<HttpRequestUserAgentEnricher>()
                    .Enrich.With<UserNameEnricher>()
                    .Enrich.WithProperty("ApplicationName", applicationName)
                    .Enrich.WithProperty("EnvironmentName", environmentName)
                    .WriteTo.Console()
                    .CreateLogger();

                IHost host = CreateHostBuilder(args).Build();

                host.Run();
            }
            catch (Exception ex)
            {
                if (Log.Logger == null || Log.Logger.GetType().Name == "SilentLogger")
                {
                    Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .WriteTo.Console()
                        .CreateLogger();
                }

                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                    .CaptureStartupErrors(true);
                })
                .UseSerilog();
    }
}
