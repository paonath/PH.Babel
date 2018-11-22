using System;
using System.Globalization;
using Microsoft.Extensions.Localization;
using PH.Babel2.Exception;

namespace PH.Babel2.Config
{
    /// <summary>
    /// Options for Localization
    /// </summary>
    public class BabelLocalizationOptions : LocalizationOptions
    {
        /// <summary>
        /// Duration of cache
        /// </summary>
        public TimeSpan CacheDuration { get; set; } = TimeSpan.FromHours(12);

        /// <summary>
        /// Default <see cref="CultureInfo"/> (en-US)
        /// </summary>
        public CultureInfo DefaultCulture { get; set; } = new CultureInfo("en-US");

        /// <summary>
        /// If True IBabel trow a <see cref="BabelKeyNotFoundException"/> on not found key, otherwise return key same as <see cref="IStringLocalizer"/>.
        /// 
        /// Default False
        /// </summary>
        public bool ThrowExceptionOnNotFoundKey { get; set; } = false;
    }
}