using System;
using System.Collections.Generic;
using System.Text;

namespace PH.Babel2.Exception
{
    public class BabelException : System.Exception
    {
        public BabelException()
        {
            
        }

        public BabelException(string message)
            :base(message)
        {
            
        }

        public BabelException(string message, System.Exception innerException)
            :base(message, innerException)
        {
            
        }
    }

    public class BabelKeyNotFoundException : BabelException
    {
        
        public BabelKeyNotFoundException(string key, System.Exception innerException, string searchedLocations)
            :base($"Key {key} not found", innerException)
        {
            Key = key;
            SearchedLocations = searchedLocations;
        }

        public BabelKeyNotFoundException(string key, string searchedLocations)
            :base($"Key {key} not found")
        {
            Key = key;
            SearchedLocations = searchedLocations;
        }

        public string Key { get; }
        public string SearchedLocations { get; }
    }
}
