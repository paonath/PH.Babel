using System;
using System.Collections.Generic;
using Autofac;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PH.Babel2.Config;

namespace PH.Babel2.Autofac
{
    public class Test
    {
        public Test()
        {
            var b = new ContainerBuilder();
           
        }
    }

    public static class AutofacBabelExtensions
    {
        public static void AddBabel(this ContainerBuilder builder,IOptions<BabelLocalizationOptions> babelOptions)
        {
            if(null == builder)
                throw new ArgumentNullException(nameof(builder));

            builder.Register(c => babelOptions)
                   .AsSelf()
                   .InstancePerLifetimeScope();

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



        }

    }


}
