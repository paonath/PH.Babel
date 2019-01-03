using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace PH.Babel2.Models
{
    public class ResourceModel
    {
        [JsonIgnore]
        public string FileName { get; set; }
        public BabelEntry[] ResourceDictionary { get; set; }

        public ResourceModel()
            :this(null,new BabelEntry[0])
        {
            
        }

        internal ResourceModel(string fileName, BabelEntry[] entries)
        {
            FileName = fileName;
            ResourceDictionary = entries;
        }

        [NotNull]
        public static ResourceModel Map(string fileName, BabelEntry[] entries)
        {
            return  new ResourceModel(fileName,entries);
        }
        [NotNull]
        public static ResourceModel Map(string fileName, [NotNull] IEnumerable<BabelEntry> entries)
        {
            return Map(fileName, entries.OrderBy(x => x.OrderField).ToArray());
        }

        [NotNull]
        public static ResourceModel Filename(string fileName)
        {
            return Map(fileName, new BabelEntry[0]);
        }

        [NotNull]
        public ResourceModel Entries([NotNull] IEnumerable<BabelEntry> entries)
        {
            var l = ResourceDictionary.ToList();
            l.AddRange(entries);
            ResourceDictionary = l.OrderBy(x => x.OrderField).ToArray();
            return this;
        }

        [NotNull]
        public ResourceModel Entry(BabelEntry entry)
        {
            return Entries(new List<BabelEntry>() {entry});
        }

        [NotNull]
        public ResourceModel Entries([NotNull] EntryList l)
        {
            return Entries(l.Entries);
        }

    }
}