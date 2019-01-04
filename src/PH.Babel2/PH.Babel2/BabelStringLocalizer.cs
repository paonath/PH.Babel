using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using PH.Babel2.Provider;

namespace PH.Babel2
{
    public class BabelStringLocalizer : Babel , IBabelStringLocalizer
    {
        public BabelStringLocalizer(IEntryProvider entryProvider, ILogger<BabelStringLocalizer> logger, [CanBeNull] CultureInfo currentCulture = null) 
            : base(entryProvider, logger,currentCulture)
        {
        }

        [NotNull]
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var l = base.GetAllBabelLocalizedStrings(includeParentCultures);
            return l;
        }

        [NotNull]
        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return new BabelStringLocalizer(EntryProvider, Logger, culture);
        }

        public new LocalizedString this[string name] => base[name];

        public new LocalizedString this[string name, params object[] arguments] => base[name, arguments];


        public BabelStringLocalizer<TResource> GetTyped<TResource>()
        {
            return new BabelStringLocalizer<TResource>(EntryProvider, Logger, CurrentCulture);
        }

        

    }

    
    public class BabelStringLocalizer<TResource> : Babel, IBabelStringLocalizer<TResource>
    {
        private readonly string _assemblyFullName;
        private readonly string _resourceName;

        public BabelStringLocalizer(IEntryProvider entryProvider, ILogger<BabelStringLocalizer> logger, [CanBeNull] CultureInfo currentCulture = null) 
            : base(entryProvider, logger, currentCulture)
        {
            var t = typeof(TResource);
            _assemblyFullName = t.AssemblyQualifiedName;
            _resourceName = t.Name;
        }

        [NotNull]
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var l = base.GetAllBabelLocalizedStrings(includeParentCultures).Where(x => x.Resource == _resourceName)
                        .ToList();
            return l;
        }

        [NotNull]
        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return new BabelStringLocalizer<TResource>(EntryProvider, Logger, culture);
        }

        public new BabelLocalizedString<TResource> this[string name] => base.Get<TResource>(name);
        public new BabelLocalizedString<TResource> this[string name, params object[] arguments] => base.Get<TResource>(name, arguments);
        public new BabelLocalizedString<TResource> this[CultureInfo culture, string name, params object[] arguments] => base.Get<TResource>(culture,name, arguments);
        public new BabelLocalizedString<TResource> this[CultureInfo culture, string name] => base.Get<TResource>(culture,name);

        LocalizedString IStringLocalizer.this[string name] => this[name];
        LocalizedString IStringLocalizer.this[string name, params object[] arguments] => this[name, arguments];
    }
}