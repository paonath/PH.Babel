using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PH.Babel2.Config;
using PH.Babel2.Events;
using PH.Babel2.Extensions;
using PH.Babel2.Models;

namespace PH.Babel2.Provider
{
    public class JsonBabelEntryProvider : JsonBabelEntryWriter,  IEntryProvider , IDisposable
    {
        //private List<FileSystemWatcher> _fileSystemWatchers;
        private FileSystemWatcher _fileSystemWatcher;
        public bool Disposed { get; private set; }
        private readonly IMemoryCache _babelCache;
        private const string cacheKey = "PH_Babel2";

        public JsonBabelEntryProvider(ILogger<BabelStringLocalizer> logger, [NotNull] IOptions<BabelLocalizationOptions> options, IMemoryCache babelCache)
        :this(logger,options.Value,babelCache)
        {
        }

        public JsonBabelEntryProvider(ILogger<BabelStringLocalizer> logger,[NotNull] BabelLocalizationOptions options, IMemoryCache babelCache):base(logger, options)
        {
            _babelCache = babelCache;
            Disposed = false;
            _fileSystemWatcher = new FileSystemWatcher(options.ResourcesPath);
            _fileSystemWatcher.Changed += WatcherOnChanged;
            Initialized = false;
            SupportedResources = new List<Type>();

        }


        public bool ThrowExceptionOnNotFoundKey => Options.ThrowExceptionOnNotFoundKey;

        public CultureInfo DefaultCulture => Options.DefaultCulture;

        public bool Initialized { get; private set; }

        [NotNull]
        public IEntryProvider Initialize()
        {
            var t = InitializeAsync();
            t.Wait();
            return t.Result;
        }

        [ItemNotNull]
        public async Task<IEntryProvider> InitializeAsync()
        {
            _fileSystemWatcher.EnableRaisingEvents = true;

            var t = await ReGenerateCacheAsync();
            Initialized = true;
            return this;
        }

        [NotNull]
        internal List<CultureInfo> GetSupportedCultures(out List<BabelLocalizationFormat> entries)
        {
            entries = ProvideEntries();
            if(entries.Count == 0)
                return new List<CultureInfo>();


            var e = entries.SelectMany(x => x.Values.Keys).Distinct().ToArray();
            if(e.Length == 0)
                return new List<CultureInfo>();

            return e.Select(CultureInfo.GetCultureInfo).ToList();
        }

        public List<CultureInfo> GetSupportedCultures()
        {
            return GetSupportedCultures(out var entries);

        }

        public event EventHandler<SourceEntriesChangedEventArgs> SourceEntriesChanged;
        public event EventHandler<CacheReloadedEventArgs> CacheReloaded;

        public List<BabelLocalizationFormat> ProvideEntries()
        {
            var t = ProvideEntriesAsync();
            t.Wait();
            return t.Result;

        }

        public List<Type> SupportedResources; 


        [ItemNotNull]
        internal async Task<Tuple<List<BabelLocalizationFormat>,List<FileInfo>>> ReGenerateCacheAsync()
        {
            var files      = ResourceDirectory.GetFiles("*.json");
            var l          = new List<Tuple<ResourceModel,DateTime>>();
            var resultFiles = new List<FileInfo>();

            foreach (var fileInfo in files)
            {
                if (fileInfo.Exists)
                {
                    using (var reader = File.OpenText(fileInfo.FullName))
                    {
                        var fileText = await reader.ReadToEndAsync();
                        try
                        {
                            ResourceModel model = JsonConvert.DeserializeObject<ResourceModel>(fileText);
                            if (null != model)
                            {
                                model.FileName = fileInfo.Name;

                                l.Add(new Tuple<ResourceModel, DateTime>(model, fileInfo.LastWriteTimeUtc));

                                
                                SupportedResources =
                                    model.ResourceDictionary.Where(x => x.ResourceType != null)
                                         .Select(x => x.ResourceType).Distinct().OrderBy(x => x.Name).ToList();

                                
                               
                            }

                            resultFiles.Add(fileInfo);
                        }
                        catch 
                        {
                        }
                    }
                }

                
            }
            

            var data = ProvideFromEntries(l);
            _babelCache.Set(cacheKey, data, Options.CacheDuration);

            EventHandler<CacheReloadedEventArgs> handler = CacheReloaded;
            handler?.Invoke(this, new CacheReloadedEventArgs());

            return new Tuple<List<BabelLocalizationFormat>, List<FileInfo>>(data, resultFiles);
        }

        internal async Task<List<BabelLocalizationFormat>> ProvideEntriesAsyncFromFileSystem()
        {
            var res = await ReGenerateCacheAsync();
            return res.Item1;
        }

        public async Task<List<BabelLocalizationFormat>> ProvideEntriesAsync()
        {
            if (!Initialized)
                await InitializeAsync();


            if (_babelCache.TryGetValue(cacheKey, out List<BabelLocalizationFormat> results))
                return results;
            else
                return await ProvideEntriesAsyncFromFileSystem();
        }

        [NotNull]
        public Dictionary<string, List<CultureInfo>> GetMissingTranslations()
        {
            var allCultures = GetSupportedCultures(out var entryList);
            var d = new Dictionary<string, List<CultureInfo>>();

            if (entryList.Count == 0)
            {
                Logger.LogWarning("Translation is empty");
                return d;
            }

            foreach (var e in entryList)
            {
                var l = new List<CultureInfo>();
                foreach (var c in allCultures)
                {
                    if(!e.Values.ContainsKey(c.Name))
                        l.Add(c);
                }
                if(l.Count > 0)
                    d.Add(e.Key, l);
            }

            return d;
        }

        private void WatcherOnChanged(object sender, [NotNull] FileSystemEventArgs e)
        {
            var f = new FileInfo(e.FullPath);
            var arg = new SourceEntriesChangedEventArgs()
                {BaseArgs = e, FileChangedFullName = e.FullPath, UtcFileLastWrite = f.LastWriteTimeUtc};

            OnSourceEntriesChanged(arg);
        }

        protected virtual void OnSourceEntriesChanged(SourceEntriesChangedEventArgs e)
        {
            var t = ReGenerateCacheAsync();
            t.Wait();

            EventHandler<SourceEntriesChangedEventArgs> handler = SourceEntriesChanged;
            handler?.Invoke(this, e);
        }


        [NotNull]
        public List<BabelLocalizationFormat> ProvideFromEntries([NotNull] IEnumerable<Tuple<ResourceModel,DateTime>> models)
        {
            var tmp = new List<BabelLocalizationFormat>();
            foreach (var resourceModel in models)
            {
                tmp.AddRange(resourceModel.Item1.ToBabelLocalizationFormats(resourceModel.Item2));
            }

            return tmp.OrderBy(x => x.ResourceName).ThenBy(x => x.Key).ToList();
        }

        internal static List<Type>  ScanEntriesForServiceInjection(ILogger<BabelStringLocalizer> logger,
                                                            [NotNull] IOptions<BabelLocalizationOptions> options,
                                                            IMemoryCache babelCache)
        {
            var provider = new JsonBabelEntryProvider(logger, options, babelCache);
            provider.Initialize();

            return provider.SupportedResources;

        }

        

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !Disposed)
            {
                _fileSystemWatcher?.Dispose();
                Disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}