using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using PH.Babel2.Events;
using PH.Babel2.Models;

namespace PH.Babel2.Provider
{
    public interface IEntryProvider
    {
        bool ThrowExceptionOnNotFoundKey { get; }
        CultureInfo DefaultCulture { get; }
        bool Initialized { get; }
        IEntryProvider Initialize();
        Task<IEntryProvider> InitializeAsync();


        List<CultureInfo> GetSupportedCultures();

        event EventHandler<SourceEntriesChangedEventArgs> SourceEntriesChanged;
        event EventHandler<CacheReloadedEventArgs> CacheReloaded;

        /// <summary>
        /// Method to read all entries from a source
        /// </summary>
        /// <returns>List of <see cref="BabelLocalizationFormat">entries</see></returns>
        List<BabelLocalizationFormat> ProvideEntries();

        Task<List<BabelLocalizationFormat>> ProvideEntriesAsync();
        Dictionary<string,List<CultureInfo>> GetMissingTranslations();
    }

    public interface IEntryWriter
    {
        
        
        void Write(IEnumerable<ResourceModel> models,bool provideKeyAsDefaultCultureTranslation = true, bool indented = true);
        Task<Guid> WriteAsync(IEnumerable<ResourceModel> models,bool provideKeyAsDefaultCultureTranslation = true, bool indented = true);
    }
}