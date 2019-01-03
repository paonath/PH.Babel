using System;
using System.Globalization;
using JetBrains.Annotations;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace PH.Babel2
{
    /// <summary>
    /// Babel Localized String : a combine of <see cref="LocalizedString"/> and <see cref="EventId"/>
    /// </summary>
    public class BabelLocalizedString : LocalizedString
    {
        

        /// <summary>
        /// The <see cref="CultureInfo"/> name of found entry. Empty if not found.
        /// </summary>
        public string Culture { get; set; }

        /// <summary>
        /// UTC Last Write on source
        /// </summary>
        public DateTime? LastWriteTimeUtc { get; set; }

        /// <summary>
        /// Optional <see cref="EventId"/> for logging purpose
        /// </summary>
        public  EventId EventId { get; private set; }

        public string SourceValue { get; internal set; }
        

        public string Resource { get; set; }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="eventIdValue"></param>
        /// <param name="eventName"></param>
        public BabelLocalizedString(string culture, string name, string value, int eventIdValue = 1, string eventName = "") : base(name, value)
        {
            Culture = culture;
            EventId = new EventId(eventIdValue, eventName);
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name">Key of <see cref="LocalizedString"/></param>
        /// <param name="value">Value of <see cref="LocalizedString"/></param>
        /// <param name="resourceNotFound">True if entry not found</param>
        /// <param name="eventIdValue">EventId</param>
        /// <param name="eventName">Event Name</param>
        public BabelLocalizedString(string culture,string name, string value, bool resourceNotFound, int eventIdValue = 1, string eventName = "") : base(name, value, resourceNotFound)
        {
            Culture = culture;
            EventId = new EventId(eventIdValue, eventName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Key of <see cref="LocalizedString"/></param>
        /// <param name="value">Value of <see cref="LocalizedString"/></param>
        /// <param name="resourceNotFound">True if entry not found</param>
        /// <param name="searchedLocation">Location of transaltions</param>
        /// <param name="eventIdValue">EventId</param>
        /// <param name="eventName">Event Name</param>
        public BabelLocalizedString(string culture,string name, string value, bool resourceNotFound, string searchedLocation, int eventIdValue = 1, string eventName = "") : base(name, value, resourceNotFound, searchedLocation)
        {
            Culture = culture;
            EventId = new EventId(eventIdValue, eventName);
        }

        [NotNull]
        internal BabelLocalizedString<TResource> ToTypedString<TResource>()
        {
            return new BabelLocalizedString<TResource>(Culture,Name,Value, ResourceNotFound, SearchedLocation, EventId.Id, EventId.Name )
            {
                LastWriteTimeUtc = LastWriteTimeUtc,
                SourceValue = SourceValue,
            };
        }
    }
    
    
    public class BabelLocalizedString<TResource> : BabelLocalizedString
    {
        public Type ResourceType { get; internal set; }

        private void SetType()
        {
            var t = typeof(TResource);
            ResourceType = t;
        }

        public BabelLocalizedString(string culture,string name, string value, int eventIdValue = 1, string eventName = "")
            : base(culture,name, value, eventIdValue, eventName)
        {
            SetType();
        }

        public BabelLocalizedString(string culture,string name, string value, bool resourceNotFound, int eventIdValue = 1, string eventName = "") 
            : base(culture,name, value, resourceNotFound, eventIdValue, eventName)
        {
            SetType();
        }

        public BabelLocalizedString(string culture,string name, string value, bool resourceNotFound, string searchedLocation, int eventIdValue = 1, string eventName = "") 
            : base(culture,name, value, resourceNotFound, searchedLocation, eventIdValue, eventName)
        {
            SetType();
        }
    }
}