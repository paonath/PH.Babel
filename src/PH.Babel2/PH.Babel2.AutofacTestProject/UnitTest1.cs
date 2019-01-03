using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PH.Babel2.Autofac;
using PH.Babel2.Config;
using Xunit;
//using PH.Babel2.Autofac;

namespace PH.Babel2.AutofacTestProject
{
    public class UnitTest
    {
        private readonly IServiceCollection Services;

        public UnitTest()
        {
            var services = new ServiceCollection();

            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

            services.AddLogging((builder) => builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace));
            
            services.AddMemoryCache();

            Services = services;
        }

        [Fact]
        public void Register()
        {
            Services.AddScoped(typeof(IOptions<BabelLocalizationOptions>), p => new OptionsWrapper<BabelLocalizationOptions>(new BabelLocalizationOptions()
            {
                ResourcesPath = ".\\Resources"
            }));

            var builder = new ContainerBuilder();

            builder.Populate(Services);

            builder
                .Register(x => new PH.Babel2.Provider.JsonBabelEntryProvider(x.Resolve<ILogger<BabelStringLocalizer>>(),
                                                                             x.Resolve<IOptions<BabelLocalizationOptions>>(), 
                                                                             x.Resolve<IMemoryCache>())
                         )
                .AsSelf()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.Register(x => new BabelStringLocalizer(x.Resolve<PH.Babel2.Provider.IEntryProvider>(),
                                                           x.Resolve<ILogger<BabelStringLocalizer>>()))
                   .AsSelf()
                   .AsImplementedInterfaces()
                   .InstancePerLifetimeScope();

            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var svc = scope.Resolve<IBabel>();
                var t   = svc["Test 12"];

            }
        }

        [Fact]
        public void Test1()
        {
            var builder = new ContainerBuilder();

            builder.Populate(Services);

            

            builder.AddBabel(new OptionsWrapper<BabelLocalizationOptions>(new BabelLocalizationOptions(){ ResourcesPath = ".\\TestResource"}));

            
            //containerBuilder.RegisterBabel();

            // Creating a new AutofacServiceProvider makes the container
            // available to your app using the Microsoft IServiceProvider
            // interface so you can use those abstractions rather than
            // binding directly to Autofac.
            var container       = builder.Build();
            var serviceProvider = new AutofacServiceProvider(container);

            using (var scope = container.BeginLifetimeScope())
            {
                var svc = scope.Resolve<IBabel>();
                var t = svc["Test 12"];

            }

        }
    }
}
