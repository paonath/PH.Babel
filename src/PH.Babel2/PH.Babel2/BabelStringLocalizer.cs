using System.Collections.Generic;
using System.Globalization;
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
    }
}