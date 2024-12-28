using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SharpStitch.Options;
using NLog.Extensions.Logging;
using NLog;
using NLog.Config;
using NLog.Targets;
using System.Diagnostics.CodeAnalysis;

namespace SharpStitch;

internal class Program
{
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(AppOptions))]
    private static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
                        
        //logging
        ConfigureNLog();
        builder.Logging.ClearProviders(); 
        builder.Logging.AddNLog();    

        //parse args
        var argsParsed = Parser.Default.ParseArguments<AppOptions>(args)
            .WithParsed(options => builder.Services.AddSingleton(options));

        if (argsParsed.Errors.Count() == 0)
        {
            //add services and start
            builder.Services.AddScoped<ImageProcessor>(); //does the image processing
            builder.Services.AddHostedService<MainAppService>(); //main app

            using IHost host = builder.Build();            
            await host.RunAsync();
        }
    }

    private static void ConfigureNLog()
    {
        var config = new LoggingConfiguration();        
        var consoleTarget = new ConsoleTarget("console")
        {
            Layout = "${message}"
        };
        config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, consoleTarget, "SharpStitch*");
        config.AddTarget(consoleTarget);
        LogManager.Configuration = config;
    }
}
