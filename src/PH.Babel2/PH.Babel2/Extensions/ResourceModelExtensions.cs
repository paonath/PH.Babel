using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.Options;
using PH.Babel2.Config;
using PH.Babel2.Models;

namespace PH.Babel2.Extensions
{
    public static class ResourceModelExtensions
    {

        



        public static List<BabelLocalizationFormat> ToBabelLocalizationFormats(this ResourceModel m, DateTime? lastWritetimeUtc)
        {
            var tmp = new List<BabelLocalizationFormat>();
            if (null == m)
                return tmp;
            else
            {
                foreach (var babelEntry in m.ResourceDictionary)
                {
                    foreach (var v in babelEntry.EntryValues)
                    {
                        var values = v.Values.OrderBy(x => x.Key).ToDictionary(pair => pair.Key, pair => (object) pair.Value);
                        var format = new BabelLocalizationFormat(v.Key, v.EventId, v.EventName, v.ResourceName, values,
                                                                 babelEntry.ResourceType?.AssemblyQualifiedName,
                                                                 lastWritetimeUtc);
                        format.Location = m.FileName;

                        tmp.Add(format);
                    }
                }
            }

            return tmp.OrderBy(x => x.Key).ToList();
        }
    }
}
