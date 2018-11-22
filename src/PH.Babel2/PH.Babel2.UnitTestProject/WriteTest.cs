using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using PH.Babel2.Extensions;
using PH.Babel2.Models;
using PH.Babel2.Provider;
using Xunit;
using Xunit.Abstractions;

namespace PH.Babel2.UnitTestProject
{
    public class WriteTest : BaseTestClass
    {
        [Fact]
        public async void WriteTest1()
        {
            var svc = ServiceProvider.GetService<IEntryWriter>();
            var entries = new ResourceModel()
            {
                FileName = "Test",
                ResourceDictionary = new BabelEntry[]
                {
                    new BabelEntry()
                    {
                        ResourceType = typeof(WriteTest),
                        EntryValues = new BabelLocalizationFormat[]
                        {
                            new BabelLocalizationFormat()
                            {
                                EventId = 1, EventName = "Test 12", Key = "Test 12",
                                Values = new Dictionary<string, object>()
                                    {{"it-IT", "Prova buonissima"}, {"fr-FR", "Debug FR"}},
                            },
                            new BabelLocalizationFormat()
                            {
                                EventId = 1, EventName = "Test", Key = "Test",
                                Values = new Dictionary<string, object>()
                                    {{"it-IT", "Prova"}, {"fr-FR", "Debug FR"}},
                            },
                            new BabelLocalizationFormat()
                            {
                                EventId = 2, EventName = "Test 2 {0}", Key = "Test 2 {0}",
                                Values = new Dictionary<string, object>()
                                {
                                    {"en-US", $"Testing with Parameter {{0}}"},
                                    {"it-IT", $"Prova con parametro {{0}}  {DateTime.Now:O}"},
                                    {"fr-FR", $"Debug {{0}} FR  {DateTime.Now:O}"}
                                }
                            },
                            new BabelLocalizationFormat()
                            {
                                EventId = 64, EventName = $"Test {DateTime.Now.Ticks} {0}",
                                Key     = $"Test 3 {DateTime.Now:O} {0}",
                                Values = new Dictionary<string, object>()
                                {
                                    {"en-US", $"Testing 3 with Parameter {{0}}"},
                                    {"it-IT", "Prova 3 con parametro {0}"}, {"fr-FR", "Debug 3 {0} FR"}
                                }
                            }
                        }
                    },

                    new BabelEntry()
                    {
                        ResourceType = typeof(BabelLocalizationFormat),
                        EntryValues = new BabelLocalizationFormat[]
                        {
                            new BabelLocalizationFormat()
                            {
                                EventId = 666, EventName = "Test Hell", Key = "Test Hell",
                                Values = new Dictionary<string, object>()
                                    {{"it-IT", "Prova buonissima"}, {"fr-FR", "Debug FR"}},
                            }


                        },
                    }
                }
            };


            await svc.WriteAsync(new List<ResourceModel>() {entries});




        }

        [Fact]
        public void ResourceModelMapping()
        {
            var entries = new ResourceModel()
            {
                FileName = "Test",
                ResourceDictionary = new BabelEntry[]
                {
                    new BabelEntry()
                    {
                        ResourceType = typeof(WriteTest),
                        EntryValues = new BabelLocalizationFormat[]
                        {
                            new BabelLocalizationFormat()
                            {
                                EventId = 1, EventName = "Test 12", Key = "Test 12",
                                Values = new Dictionary<string, object>()
                                    {{"it-IT", "Prova buonissima"}, {"fr-FR", "Debug FR"}, {"fr","Main Français Test 12"}},
                            },
                            new BabelLocalizationFormat()
                            {
                                EventId = 1, EventName = "Test", Key = "Test",
                                Values = new Dictionary<string, object>()
                                    {{"it-IT", "Prova"}, {"fr-FR", "Debug FR"}},
                            },
                            new BabelLocalizationFormat()
                            {
                                EventId = 2, EventName = "Test 2 {0}", Key = "Test 2 {0}",
                                Values = new Dictionary<string, object>()
                                {
                                    {"en-US", $"Testing with Parameter {{0}}"},
                                    {"it-IT", $"Prova con parametro {{0}}  {DateTime.Now:O}"},
                                    {"fr-FR", $"Debug {{0}} FR  {DateTime.Now:O}"},
                                    {"fr","Main Français Test 2 {0}"}
                                }
                            },
                            new BabelLocalizationFormat()
                            {
                                EventId = 64, EventName = $"Test {DateTime.Now.Ticks} {0}",
                                Key     = $"Test 3 {DateTime.Now:O} {0}",
                                Values = new Dictionary<string, object>()
                                {
                                    {"en-US", $"Testing 3 with Parameter {{0}}"},
                                    {"it-IT", "Prova 3 con parametro {0}"}, {"fr-FR", "Debug 3 {0} FR"},
                                    {"fr","Main Français Testing 3 with Parameter {{0}}"}
                                }
                            }
                        }
                    },

                    new BabelEntry()
                    {
                        ResourceType = typeof(BabelLocalizationFormat),
                        EntryValues = new BabelLocalizationFormat[]
                        {
                            new BabelLocalizationFormat()
                            {
                                EventId = 666, EventName = "Test Hell", Key = "Test Hell",
                                Values = new Dictionary<string, object>()
                                    {{"it-IT", "Prova buonissima"}, {"fr-FR", "Debug FR"}, {"fr","Main Français est Hell"}},
                            }


                        },
                    }
                }
            };


            var r = ResourceModel
                .Map("test_fluent", entries.ResourceDictionary);

            var r2 = ResourceModel.Filename("test_fluent2")
                                  .Entries(new EntryList(entries.ResourceDictionary.ToList()));

            var r3 = ResourceModel.Filename("test_fluent2").Entry(new BabelEntry()).Entry(new BabelEntry());




        }

        [Fact]
        public void WritingNewDataWillRealoadCache()
        {
             var svc = ServiceProvider.GetService<IEntryWriter>();
            var entries = new ResourceModel()
            {
                FileName = "Test",
                ResourceDictionary = new BabelEntry[]
                {
                    new BabelEntry()
                    {
                        ResourceType = typeof(WriteTest),
                        EntryValues = new BabelLocalizationFormat[]
                        {
                            new BabelLocalizationFormat()
                            {
                                EventId = 1, EventName = "Test 12", Key = "Test 12",
                                Values = new Dictionary<string, object>()
                                    {{"it-IT", "Prova buonissima"}, {"fr-FR", "Debug FR"}, {"fr","Main Français Test 12"}},
                            },
                            new BabelLocalizationFormat()
                            {
                                EventId = 1, EventName = "Test", Key = "Test",
                                Values = new Dictionary<string, object>()
                                    {{"it-IT", "Prova"}, {"fr-FR", "Debug FR"}},
                            },
                            new BabelLocalizationFormat()
                            {
                                EventId = 2, EventName = "Test 2 {0}", Key = "Test 2 {0}",
                                Values = new Dictionary<string, object>()
                                {
                                    {"en-US", $"Testing with Parameter {{0}}"},
                                    {"it-IT", $"Prova con parametro {{0}}  {DateTime.Now:O}"},
                                    {"fr-FR", $"Debug {{0}} FR  {DateTime.Now:O}"},
                                    {"fr","Main Français Test 2 {0}"}
                                }
                            },
                            new BabelLocalizationFormat()
                            {
                                EventId = 64, EventName = $"Test {DateTime.Now.Ticks} {0}",
                                Key     = $"Test 3 {DateTime.Now:O} {0}",
                                Values = new Dictionary<string, object>()
                                {
                                    {"en-US", $"Testing 3 with Parameter {{0}}"},
                                    {"it-IT", "Prova 3 con parametro {0}"}, {"fr-FR", "Debug 3 {0} FR"},
                                    {"fr","Main Français Testing 3 with Parameter {{0}}"}
                                }
                            }
                        }
                    },

                    new BabelEntry()
                    {
                        ResourceType = typeof(BabelLocalizationFormat),
                        EntryValues = new BabelLocalizationFormat[]
                        {
                            new BabelLocalizationFormat()
                            {
                                EventId = 666, EventName = "Test Hell", Key = "Test Hell",
                                Values = new Dictionary<string, object>()
                                    {{"it-IT", "Prova buonissima"}, {"fr-FR", "Debug FR"}, {"fr","Main Français est Hell"}},
                            }


                        },
                    }
                }
            };


            svc.Write(new List<ResourceModel>() {entries});

            var svcReader = ServiceProvider.GetService<IEntryProvider>();
            
            bool checkCacheRealoaded = false;
            
            svcReader.Initialize();


            var entry2 = new ResourceModel()
            {
                FileName = "test2",
                ResourceDictionary = new BabelEntry[]
                {
                    new BabelEntry()
                    {
                        ResourceType = null,
                        EntryValues = new BabelLocalizationFormat[]
                        {
                            new BabelLocalizationFormat("test-null",
                                                        new Dictionary<string, string>() {{"it-IT", "test-null"}}),
                        }
                    }
                }
            };

            svcReader.CacheReloaded += (sender, args) =>
            {
                checkCacheRealoaded = true;
            };


            svc.Write(new List<ResourceModel>() {entry2});
            System.Threading.Thread.Sleep(150);



            while (checkCacheRealoaded == false)
            {
                
                System.Threading.Thread.Sleep(150);

            }

            

            Assert.True(checkCacheRealoaded);

        }


        public WriteTest(ITestOutputHelper output) : base(output)
        {
        }
    }
}
