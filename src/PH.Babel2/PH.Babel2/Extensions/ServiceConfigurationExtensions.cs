using System;
using JetBrains.Annotations;
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
        [NotNull]
        public static IServiceCollection AddBabel([NotNull] this IServiceCollection services,
                                                  [NotNull] Action<PH.Babel2.Config.BabelLocalizationOptions> option)
        {
            if(null == services)
                throw new ArgumentNullException(nameof(services));
            if(null == option)
                throw new ArgumentNullException(nameof(option));
            
            services.Configure(option);
            return ServiceConfigurationExtensions.AddBabel(services);
        }

        

        [NotNull]
        public static IServiceCollection AddBabel([NotNull] this IServiceCollection services)
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