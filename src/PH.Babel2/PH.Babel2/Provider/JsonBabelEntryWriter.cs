using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PH.Babel2.Config;
using PH.Babel2.Models;

namespace PH.Babel2.Provider
{
    public class JsonBabelEntryWriter : IEntryWriter
    {
        protected readonly DirectoryInfo ResourceDirectory;
        protected readonly BabelLocalizationOptions Options;
        protected readonly ILogger<BabelStringLocalizer> Logger;

        public JsonBabelEntryWriter(ILogger<BabelStringLocalizer> logger, BabelLocalizationOptions options)
        {
            Logger           = logger;
            Options           = options;
            ResourceDirectory = new DirectoryInfo(options.ResourcesPath);
            if (!ResourceDirectory.Exists)
                ResourceDirectory.Create();


        }


        public void Write(IEnumerable<ResourceModel> models, bool provideKeyAsDefaultCultureTranslation = true,
                          bool indented = true)
        {
            var t = WriteAsync(models, provideKeyAsDefaultCultureTranslation, indented);
            t.Wait();
            
        }

        private ResourceModel[] PrepareModels(IEnumerable<ResourceModel> models,
                                              bool provideKeyAsDefaultCultureTranslation)
        {
            if (!provideKeyAsDefaultCultureTranslation)
                return models.ToArray();

            var l = new List<ResourceModel>();
            foreach (var resourceModel in models)
            {
                var r = new ResourceModel()
                {
                    FileName = resourceModel.FileName
                };
                var vll = new List<BabelEntry>();

                foreach (var babelEntry in resourceModel.ResourceDictionary)
                {
                    var e = new BabelEntry()
                    {
                        ResourceType = babelEntry.ResourceType
                    };
                    var vl = new List<BabelLocalizationFormat>();
                    foreach (var v in babelEntry.EntryValues)
                    {
                        if (!v.Values.ContainsKey(Options.DefaultCulture.Name))
                            v.Values.Add(Options.DefaultCulture.Name, v.Key);

                        vl.Add(v);

                    }

                    e.EntryValues = vl.OrderBy(x => x.Key).ToArray();
                    vll.Add(e);
                }

                r.ResourceDictionary = vll.OrderBy(x => x.OrderField).ToArray();

                l.Add(r);
            }

            return l.OrderBy(x => x.FileName).ToArray();

        }

        private string JsonSerialize(ResourceModel model, bool indented)
        {
            var format = Formatting.Indented;
            if (!indented)
                format = Formatting.None;

            var serialized = JsonConvert.SerializeObject(model, format);

            return serialized;
        }

        public async Task<Guid> WriteAsync(IEnumerable<ResourceModel> models,
                                           bool provideKeyAsDefaultCultureTranslation = true, bool indented = true)
        {
            var modelsArray = PrepareModels(models, provideKeyAsDefaultCultureTranslation);

            foreach (var resourceModel in modelsArray)
            {
                var json  = JsonSerialize(resourceModel, indented);
                var fName = $"{ResourceDirectory.FullName}{Path.DirectorySeparatorChar}{resourceModel.FileName}";
                if (!fName.EndsWith(".json"))
                    fName = $"{fName}.json";

                using (var sw = new StreamWriter(fName))
                {
                    await sw.WriteAsync(json);
                }
            }

            return Guid.NewGuid();
        }
    }
}