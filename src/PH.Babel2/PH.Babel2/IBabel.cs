using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace PH.Babel2
{
    public interface IBabel
    {
        CultureInfo CurrentCulture { get; }
        CultureInfo DefaultCulture  { get; }

        List<CultureInfo> SupportedCultures { get; }

        BabelLocalizedString<TResource> Get<TResource>(string name);
        

        BabelLocalizedString<TResource> Get<TResource>(string name, params object[] arguments);
        

        BabelLocalizedString<TResource> Get<TResource>(CultureInfo culture, string name);
        

        BabelLocalizedString<TResource> Get<TResource>(CultureInfo culture, string name, params object[] arguments);
        

        BabelLocalizedString this[string name] { get; }
        BabelLocalizedString this[CultureInfo culture, string name] { get; }
        BabelLocalizedString this[Type resourceType,CultureInfo culture, string name] { get; }
        

        BabelLocalizedString this[string name, params object[] arguments] { get; }
        BabelLocalizedString this[CultureInfo culture, string name, params object[] arguments] { get; }
        BabelLocalizedString this[Type resourceType,CultureInfo culture, string name, params object[] arguments] { get; }

        

    }
}