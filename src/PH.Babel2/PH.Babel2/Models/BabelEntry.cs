using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace PH.Babel2.Models
{
    public class EntryList
    {
        public EntryList():this(new List<BabelEntry>())
        {
            
        }
        public EntryList(List<BabelEntry> entries)
        {
            _entries = entries;
        }


        private readonly List<BabelEntry> _entries;

        public List<BabelEntry> Entries => GetEntries();

        private List<BabelEntry> GetEntries()
        {
            return _entries.OrderBy(x => x.OrderField).ToList();
        }

        public EntryList MapEntries(IEnumerable<BabelEntry> entries)
        {
            _entries.AddRange(entries);
            return this;
        }

        public EntryList Entry(BabelEntry e)
        {
            return MapEntries(new List<BabelEntry>() {e});
        }

        public EntryList Entry(Type resourceType, IEnumerable<BabelLocalizationFormat> values)
        {
            return Entry(new BabelEntry()
                             {ResourceType = resourceType, EntryValues = values.OrderBy(x => x.Key).ToArray()});
        }
    }

    public class BabelEntry
    {
        [JsonProperty(Order = 1)]
        public Type ResourceType { get; set; }  
        [JsonProperty(Order = 2)]
        public BabelLocalizationFormat[] EntryValues { get; set; }

        [JsonIgnore]
        internal string OrderField => $"{ResourceType?.FullName}_BabelEntry";
    }
}