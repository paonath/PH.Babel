using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace PH.Babel2.Models
{
    /// <summary>
    /// Model for saving/reading localized strings
    /// </summary>
    public class BabelLocalizationFormat
    {
        [JsonIgnore]
        public DateTime LastWriteTimeUtc { get; set; }

        [JsonIgnore]
        public string Location { get; set; }

        /// <summary>
        /// Key String Name
        /// Value on default CultureInfo
        /// </summary>
        [JsonProperty(Order = 1)]
        public string Key { get; set; }


        /// <summary>
        /// Optional EventId
        /// </summary>
        [JsonProperty(Order = 2)]
        public int? EventId { get; set; }

        /// <summary>
        /// Optional EventName
        /// </summary>
        [JsonProperty(Order = 3)]
        public string EventName { get; set; }


        //[JsonProperty(Order = 4)]
        [JsonIgnore]
        public string ResourceName { get; set; }

        [JsonIgnore]
        public string AssemblyQualifiedName { get; set; }

        /// <summary>
        /// Per-CultureInfo Names translations
        /// </summary>
        [JsonProperty(Order = 4)]
        [JsonExtensionData]
        public Dictionary<string, object> Values { get; set; }


        public BabelLocalizationFormat()
        {
            Values = new Dictionary<string, object>();
        }

        public BabelLocalizationFormat(string key, Dictionary<string, string> values):this(key,(int?)null, key, values)
        {
            
        }

        public BabelLocalizationFormat(string key, int? eventId, string name, Dictionary<string, string> values)
            :this()
        {
            Key = key;
            EventId = eventId;
            EventName = name;
            Values = values.OrderBy(x => x.Key).ToDictionary(pair => pair.Key, pair => (object) pair.Value);
        }
        
        internal BabelLocalizationFormat(string key, int? eventId, string eventName, string resourceName,
                                         Dictionary<string, object> values, string assemblyFullName, DateTime? lastWriteTimeUtc = (DateTime?)null) : this()
        {
            Key          = key;
            EventId      = eventId;
            EventName    = eventName;
            ResourceName = resourceName;
            Values       = values;
            LastWriteTimeUtc = lastWriteTimeUtc.GetValueOrDefault(DateTime.MinValue.ToUniversalTime());
            AssemblyQualifiedName = assemblyFullName;
        }
    }
}