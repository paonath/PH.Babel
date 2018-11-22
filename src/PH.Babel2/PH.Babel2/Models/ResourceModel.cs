using System.Collections.Generic;
using System.Linq;
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

        public static ResourceModel Map(string fileName, BabelEntry[] entries)
        {
            return  new ResourceModel(fileName,entries);
        }
        public static ResourceModel Map(string fileName, IEnumerable<BabelEntry> entries)
        {
            return Map(fileName, entries.OrderBy(x => x.OrderField).ToArray());
        }

        public static ResourceModel Filename(string fileName)
        {
            return Map(fileName, new BabelEntry[0]);
        }

        public ResourceModel Entries(IEnumerable<BabelEntry> entries)
        {
            var l = ResourceDictionary.ToList();
            l.AddRange(entries);
            ResourceDictionary = l.OrderBy(x => x.OrderField).ToArray();
            return this;
        }

        public ResourceModel Entry(BabelEntry entry)
        {
            return Entries(new List<BabelEntry>() {entry});
        }

        public ResourceModel Entries(EntryList l)
        {
            return Entries(l.Entries);
        }

    }
}