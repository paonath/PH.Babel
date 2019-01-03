using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using PH.Babel2.Exception;
using PH.Babel2.Models;
using PH.Babel2.Provider;

namespace PH.Babel2
{
    public class Babel : IBabel
    {
        protected readonly IEntryProvider EntryProvider;
        protected readonly ILogger<BabelStringLocalizer> Logger;

        public CultureInfo CurrentCulture { get; protected set; }
        public CultureInfo DefaultCulture => EntryProvider.DefaultCulture;

        public List<CultureInfo> SupportedCultures => EntryProvider.GetSupportedCultures();


        public Babel(IEntryProvider entryProvider, ILogger<BabelStringLocalizer> logger, [CanBeNull] CultureInfo currentCulture = null)
        {
            EntryProvider = entryProvider;
            Logger         = logger;
            CurrentCulture = currentCulture ?? CultureInfo.CurrentUICulture;

        }

        [CanBeNull]
        private string GetAssemblyName<TResource>()
        {
            var t = typeof(TResource);
            return t.AssemblyQualifiedName;
        }

        protected virtual BabelLocalizedString<TResource> GetStringFromEmpty<TResource>(CultureInfo info, string keyName, params object[] arguments)
        {
            return GetStringFromEmpty(info, keyName, arguments).ToTypedString<TResource>();
        }

        protected virtual BabelLocalizedString GetStringFromEmpty([CanBeNull] CultureInfo info,string keyName, [CanBeNull] params object[] arguments)
        {
            var allLocations = EntryProvider.ProvideEntries().Select(x => x.Location).Distinct().ToArray();
            var locations    = string.Join(", ", allLocations);


            if (EntryProvider.ThrowExceptionOnNotFoundKey)
            {
                Logger.LogWarning($"Babel Options are set for trow exception on not found key: empty key '{keyName}'");
                ArgumentOutOfRangeException ex = new ArgumentOutOfRangeException(nameof(keyName), keyName,$"Key not found - key '{keyName}', culture '{info?.Name}'");
                throw new BabelKeyNotFoundException(keyName, ex, locations);
            }
            else
            {
                
                var culture = CurrentCulture;
                if (null != info)
                    culture = info;

                var vl = keyName;
                if (null != arguments && arguments.Length > 0)
                    vl = string.Format(culture,keyName, arguments);

                return new BabelLocalizedString("",keyName, vl, true, locations);
            }

            

        }

        protected virtual BabelLocalizedString<TResource> GetStringFromFormat<TResource>(
            CultureInfo info, BabelLocalizationFormat f)
        {
            return GetStringFromFormat(info, f).ToTypedString<TResource>();

        }

        protected virtual BabelLocalizedString<TResource> GetStringFromFormat<TResource>(
            CultureInfo info, BabelLocalizationFormat f, params object[] arguments)
        {
            return GetStringFromFormat(info, f, arguments).ToTypedString<TResource>();
        }


       

        protected virtual BabelLocalizedString GetStringFromFormat(CultureInfo info, BabelLocalizationFormat f, params object[] arguments)
        {
            var culture = CurrentCulture;
            if (null != info)
                culture = info;

            if (f.Values.ContainsKey(culture.Name))
            {
                var    vl    = f.Values[culture.Name];
                string value = vl.ToString();
                if (null != arguments && arguments.Length > 0)
                {
                    value = string.Format(culture, vl.ToString(), arguments);
                }

                string srcValue = value;

                if (f.Values.ContainsKey(DefaultCulture.Name))
                {
                    srcValue = f.Values[DefaultCulture.Name].ToString();
                    if (null != arguments && arguments.Length > 0)
                    {
                        srcValue = string.Format(culture, srcValue.ToString(), arguments);
                    }
                }
                var ls = new BabelLocalizedString(culture.Name,f.Key, value, false, f.Location, f.EventId.GetValueOrDefault(), f.EventName)
                {
                    SourceValue      = srcValue,
                    LastWriteTimeUtc = f.LastWriteTimeUtc,
                    Resource         = f.ResourceName,
                    
                };

                return ls;
            }
            else
            {
                Logger.LogWarning($"No translations for culture '{culture.Name}' and key '{f.Key}'");

                if (culture.LCID != culture.Parent.LCID && culture.LCID != DefaultCulture.LCID)
                {
                    return GetStringFromFormat(culture.Parent, f, arguments);
                }
                else
                {
                    if (culture.LCID == DefaultCulture.LCID)
                    {
                        
                        return GetStringFromEmpty(culture, f.Key, arguments);
                    }
                    else
                    {
                        return GetStringFromFormat(DefaultCulture, f, arguments);
                    }
                }

                 
                
            }



        }

        public BabelLocalizedString<TResource> Get<TResource>(string name)
        {
            
            var res = GetAssemblyName<TResource>();
            var format = EntryProvider.ProvideEntries()
                                       .FirstOrDefault(x => x.AssemblyQualifiedName == res && x.Key.ToLower() == name.ToLower());
            if (null == format)
                return GetStringFromEmpty<TResource>(null, name, null);

            return GetStringFromFormat<TResource>(null, format, null);
        }

        internal BabelLocalizedString InternalGet(string name, [NotNull] List<BabelLocalizationFormat> data)
        {
            var format = data.FirstOrDefault(x => x.Key.ToLower() == name.ToLower());
            if (null == format)
                return GetStringFromEmpty(null, name, null);

            return GetStringFromFormat(null, format, null);
        }

        public BabelLocalizedString Get(string name)
        {
            return InternalGet(name, EntryProvider.ProvideEntries());
        }


        public BabelLocalizedString<TResource> Get<TResource>(string name, params object[] arguments)
        {
            var res = GetAssemblyName<TResource>();
            var format = EntryProvider.ProvideEntries()
                                       .FirstOrDefault(x => x.AssemblyQualifiedName == res && x.Key.ToLower() == name.ToLower());
            if (null == format)
                return GetStringFromEmpty<TResource>(null, name, arguments);

            return GetStringFromFormat<TResource>(null, format, arguments);
        }
 
        public BabelLocalizedString Get(string name, params object[] arguments)
        {
            var format = EntryProvider.ProvideEntries()
                                       .FirstOrDefault(x => x.Key.ToLower() == name.ToLower());
            if (null == format)
                return GetStringFromEmpty(null, name, arguments);

            return GetStringFromFormat(null, format, arguments);
        }

        public BabelLocalizedString<TResource> Get<TResource>(CultureInfo culture, string name)
        {
            var res = GetAssemblyName<TResource>();
            var format = EntryProvider.ProvideEntries()
                                       .FirstOrDefault(x => x.AssemblyQualifiedName == res && x.Key.ToLower() == name.ToLower());
            if (null == format)
                return GetStringFromEmpty<TResource>(culture, name, null);

            return GetStringFromFormat<TResource>(culture, format, null);
        }
 
        public BabelLocalizedString Get(CultureInfo culture, string name)
        {
            var format = EntryProvider.ProvideEntries()
                                       .FirstOrDefault(x => x.Key.ToLower() == name.ToLower());
            if (null == format)
                return GetStringFromEmpty(culture, name, null);

            return GetStringFromFormat(culture, format, null);
        }

        public BabelLocalizedString<TResource> Get<TResource>(CultureInfo culture, string name, params object[] arguments)
        {
            var res = GetAssemblyName<TResource>();
            var format = EntryProvider.ProvideEntries()
                                       .FirstOrDefault(x => x.AssemblyQualifiedName == res && x.Key.ToLower() == name.ToLower());
            if (null == format)
                return GetStringFromEmpty<TResource>(culture, name, arguments);

            return GetStringFromFormat<TResource>(culture, format, arguments);
        }

        public BabelLocalizedString Get(CultureInfo culture, string name, params object[] arguments)
        {
            var format = EntryProvider.ProvideEntries()
                                       .FirstOrDefault(x =>  x.Key.ToLower() == name.ToLower());
            if (null == format)
                return GetStringFromEmpty(culture, name, arguments);

            return GetStringFromFormat(culture, format, arguments);
        }

        public BabelLocalizedString Get([NotNull] Type resourceType, CultureInfo culture, string name, params object[] arguments)
        {
            var res = resourceType.AssemblyQualifiedName;

            var format = EntryProvider.ProvideEntries()
                                       .FirstOrDefault(x => x.AssemblyQualifiedName == res && x.Key.ToLower() == name.ToLower());
            if (null == format)
                return GetStringFromEmpty(culture, name, arguments);

            return GetStringFromFormat(culture, format, arguments);
        }


        [NotNull]
        public IEnumerable<BabelLocalizedString> GetAllBabelLocalizedStrings(bool includeParentCultures)
        {
            var localization = EntryProvider.ProvideEntries();
            var d = localization.Select(x => x.Key).Select(x => InternalGet(x, localization)).OrderBy(x => x.Name)
                                .ToList();
            return d;
        }



        public BabelLocalizedString this[string name] => Get(name);
        public BabelLocalizedString this[CultureInfo culture, string name] => Get(culture, name);
        public BabelLocalizedString this[string name, params object[] arguments] => Get(name, arguments);
        public BabelLocalizedString this[CultureInfo culture, string name, params object[] arguments] => Get(culture, name, arguments);


        public BabelLocalizedString this[Type resourceType, CultureInfo culture, string name] =>
            Get(resourceType, culture, name);

        public BabelLocalizedString
            this[Type resourceType, CultureInfo culture, string name, params object[] arguments] =>
            Get(resourceType, culture, name, arguments);

    }
}