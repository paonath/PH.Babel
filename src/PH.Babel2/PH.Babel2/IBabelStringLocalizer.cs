using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Localization;

namespace PH.Babel2
{
    public interface IBabelStringLocalizer : IBabel, IStringLocalizer
    {
        

        new BabelLocalizedString this[string name] { get; }
        new BabelLocalizedString this[CultureInfo culture, string name] { get; }
        new BabelLocalizedString this[Type resourceType,CultureInfo culture, string name] { get; }
        

        new BabelLocalizedString this[string name, params object[] arguments] { get; }
        new BabelLocalizedString this[CultureInfo culture, string name, params object[] arguments] { get; }
        new BabelLocalizedString this[Type resourceType,CultureInfo culture, string name, params object[] arguments] { get; }
    }

    public interface IBabelStringLocalizer<TResource> : IBabelBase, IStringLocalizer<TResource>
    {
        new BabelLocalizedString<TResource> this[string name] { get; }
        new BabelLocalizedString<TResource> this[CultureInfo culture, string name] { get; }
        new BabelLocalizedString<TResource> this[string name, params object[] arguments] { get; }
        new BabelLocalizedString<TResource> this[CultureInfo culture, string name, params object[] arguments] { get; }

    }
}