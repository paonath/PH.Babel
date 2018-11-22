# PH.Babel2

A netstandard2.0 utility based on IStringLocalizer and Json

## Code Examples

**Setup configuration using ServiceCollection**
```c#
services.AddLogging();
services.AddMemoryCache();

services.AddBabel(options =>
{
    options.DefaultCulture = CultureInfo.GetCultureInfo("it-IT"); //Default culture
    options.CacheDuration = TimeSpan.FromHours(1); //Cache duration
    options.ResourcesPath = ".\\Resources"; //Directory path to read json resource...
});
```

**A Json Resource File**

*ResourceType* is the AssemblyQualifiedName of a Resource-Type, or Null. 

```json
{
  "ResourceDictionary": [
    {
      "ResourceType": "PH.Babel2.Models.BabelLocalizationFormat, PH.Babel2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
      "EntryValues": [
        {
          "Key": "the key name", 
          "EventId": 1,
          "EventName": "the name of the event, same as key",
          "it-IT": "italian localization",
          "fr-FR": "french loc.",
          "fr": "Main Français loc",
          "en-US": "U.S. loc"
        },
        {
          "Key": "Hell", 
          "EventId": 666,
          "EventName": "Testing Hell",
          "it-IT": "Inferno",
          "fr-FR": "Enfer",
          "fr": "enfer",
          "en-US": "Hell"
        }
        ,
        {
          "Key": "Key with Parameter {0}", 
          "EventId": 2,
          "EventName": "Key with Parameter",
          "it-IT": "Hai inserito il valore '{0}' ",
          "fr-FR": "vous avez entré la valeur '{0}' ",
          "en-US": "You have entered the value '{0}' "
        }
      ]
    },
    {
      "ResourceType": "...",
      "EntryValues": [
        {
          ...
        },
        {
         ...
        },
        
      ]
    }
  ]
}
```

**Read Example**
```c#
var svc = ServiceProvider.GetService<IBabel>();
var tst0 = svc["Hell"];

```

```c#
var svc = ServiceProvider.GetService<IStringLocalizer>();
var tst0 = svc["Hell"];

```
