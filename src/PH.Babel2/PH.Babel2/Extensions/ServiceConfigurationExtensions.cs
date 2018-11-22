using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PH.Babel2.Config;
using PH.Babel2.Provider;

namespace PH.Babel2.Extensions
{
    public static class ServiceConfigurationExtensions
    {
        public static IServiceCollection AddBabel(this IServiceCollection services,
                                                  Action<PH.Babel2.Config.BabelLocalizationOptions> option)
        {
            if(null == services)
                throw new ArgumentNullException(nameof(services));
            if(null == option)
                throw new ArgumentNullException(nameof(option));
            
            services.Configure(option);
            return ServiceConfigurationExtensions.AddBabel(services);
        }

        /*
        public static IServiceCollection AddBabel(this IServiceCollection services, PH.Babel2.Config.BabelLocalizationOptions opt)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddScoped(typeof(JsonBabelEntryProvider), p =>
            {
                return new JsonBabelEntryProvider(p.GetService<ILogger<BabelStringLocalizer>>(), opt, p.GetService<IMemoryCache>() );
            });

            services.AddScoped(typeof(IEntryWriter), p => p.GetRequiredService<JsonBabelEntryProvider>());


            services.AddScoped(typeof(IEntryProvider), p => p.GetRequiredService<JsonBabelEntryProvider>());


            services.AddScoped(typeof(BabelStringLocalizer),  p => new BabelStringLocalizer(p.GetService<IEntryProvider>(),
                                                                                            p.GetService<ILogger<BabelStringLocalizer>>()));

            services.AddScoped(typeof(IBabel), p => p.GetRequiredService<BabelStringLocalizer>());
                              
            services.AddScoped(typeof(IStringLocalizer), p => p.GetRequiredService<BabelStringLocalizer>());


            return services;
        }
        */
        public static IServiceCollection AddBabel(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton(typeof(JsonBabelEntryProvider), p =>
            {
                var provider = new JsonBabelEntryProvider(p.GetService<ILogger<BabelStringLocalizer>>(), p.GetService<IOptions<BabelLocalizationOptions>>(), p.GetService<IMemoryCache>() );

                var l = provider.SupportedResources;

                return provider;
            });

            services.AddSingleton(typeof(IEntryWriter), p => p.GetRequiredService<JsonBabelEntryProvider>());


            services.AddSingleton(typeof(IEntryProvider), p => p.GetRequiredService<JsonBabelEntryProvider>());


            services.AddScoped(typeof(BabelStringLocalizer),  p => new BabelStringLocalizer(p.GetService<IEntryProvider>(),
                                                                                            p.GetService<ILogger<BabelStringLocalizer>>()));

            services.AddScoped(typeof(IBabel), p => p.GetRequiredService<BabelStringLocalizer>());
                              
            services.AddScoped(typeof(IStringLocalizer), p => p.GetRequiredService<BabelStringLocalizer>());

            //services.Add(new ServiceDescriptor())

            //var scanTypes = JsonBabelEntryProvider.ScanEntriesForServiceInjection()
                
            return services;
        }
    }
}