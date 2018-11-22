using System;
using System.Globalization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using PH.Babel2.Config;
using PH.Babel2.Extensions;
using PH.Babel2.Provider;
using Xunit.Abstractions;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace PH.Babel2.UnitTestProject
{
    public class BaseTestClass
    {
        protected ITestOutputHelper output;
        private readonly IServiceProvider _serviceProvider;
        public IServiceProvider ServiceProvider => _serviceProvider;

        public BaseTestClass(ITestOutputHelper output)
        {
            this.output = output;
            ConfigureNlogger();
            _serviceProvider = BuildDi();
        }

        
        private static IServiceProvider BuildDi()
        {
            var services = new ServiceCollection();

            

            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

            

            services.AddLogging((builder) => builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace));
            
            services.AddMemoryCache();
           
            /*
            IOptions<BabelLocalizationOptions> options =
                new OptionsWrapper<BabelLocalizationOptions>(new BabelLocalizationOptions()
                {
                    //CacheDuration = TimeSpan.FromMinutes(2), 
                    CacheDuration  = TimeSpan.FromMinutes(3), 
                    DefaultCulture = CultureInfo.GetCultureInfo("en-US"),
                    ResourcesPath  = ".\\Resources"
                });
            */


            

            services.AddBabel(options =>
            {
                options.DefaultCulture = CultureInfo.GetCultureInfo("en-US");
                options.ThrowExceptionOnNotFoundKey = true;
                options.CacheDuration = TimeSpan.FromHours(1);
                options.ResourcesPath = ".\\Resources";
            });


            var serviceProvider = services.BuildServiceProvider();

            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            //configure NLog
            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties =true, IncludeScopes = true});
            NLog.LogManager.Configuration = GetLogConfig();

            return serviceProvider;
        }

        private static LoggingConfiguration GetLogConfig()
        {
            // Step 1. Create configuration object 
            var config = new LoggingConfiguration();

            // Step 2. Create targets and add them to the configuration 
            var consoleTarget = new ColoredConsoleTarget();
            config.AddTarget("console", consoleTarget);

            var fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);

            // Step 3. Set target properties 
            consoleTarget.Layout = @"${pad:padding=5:inner=${level:uppercase=true}} ${date:format=HH\:mm\:ss} ${logger:shortName=true} ${message} [${event-properties:item=EventId_Id} ${ndlc:uppercase=true:separator= => } ${callsite:fileName=true:includeSourcePath=false:skipFrames=1}]";
            fileTarget.FileName  = "${basedir}/logs/log.log";

            fileTarget.Layout = @"${pad:padding=5:inner=${level:uppercase=true}} ${date:format=HH\:mm\:ss} ${message} ${exception:format=toString,Data:maxInnerExceptionLevel=10} [${event-properties:item=EventId_Id} ${ndlc:uppercase=true:separator= => } ${callsite:fileName=true:includeSourcePath=false:skipFrames=1}]";


            fileTarget.ArchiveNumbering = ArchiveNumberingMode.DateAndSequence;
            fileTarget.ArchiveEvery     = FileArchivePeriod.Day;
            //fileTarget.KeepFileOpen = true;
            fileTarget.AutoFlush                    = true;
            fileTarget.ArchiveDateFormat            = "dd-MM-yyyy";
            fileTarget.ArchiveOldFileOnStartup      = true;
            fileTarget.ArchiveFileName              = "${basedir}/wwwroot/logs/log.{#}.log.zip";
            fileTarget.EnableArchiveFileCompression = true;

            // Step 4. Define rules
            var rule1 = new LoggingRule("*", NLog.LogLevel.Debug, consoleTarget);
            config.LoggingRules.Add(rule1);

            var rule2 = new LoggingRule("*", NLog.LogLevel.Debug, fileTarget);
            config.LoggingRules.Add(rule2);

            return config;
        }
        public static void ConfigureNlogger()
        {
            
            LogManager.Configuration = GetLogConfig();

        }
    }
}