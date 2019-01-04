using System;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using PH.Babel2.Provider;
using Xunit;
using Xunit.Abstractions;

namespace PH.Babel2.UnitTestProject
{
    public class ReadTest : BaseTestClass
    { 
        public ReadTest(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async void TestRead()
        {
            var svc  = ServiceProvider.GetService<IEntryProvider>();
            var data = await svc.ProvideEntriesAsync();

            var dbg = data.FirstOrDefault();

            var dataFromCache = await svc.ProvideEntriesAsync();

            var dbg2 = dataFromCache.FirstOrDefault();

            var missingTranslations = svc.GetMissingTranslations();

        }

        [Fact]
        public void ReadString()
        {
            var svc = ServiceProvider.GetService<IBabel>();

            var tst0 = svc["Test 12"];
            var tst1 = svc[CultureInfo.GetCultureInfo("it-IT") , "Test 12"];
            var tst2 = svc["Test 2 {0}", Guid.NewGuid()];
            var tst3 = svc[CultureInfo.GetCultureInfo("it-IT") , "Test 2 {0}", Guid.NewGuid()];

            var tst4 = svc["Test Hell"];

            var tst5 = svc[CultureInfo.GetCultureInfo("fr-029"), "Test 2 {0}", DateTime.MaxValue];

            var tTyped = svc.Get<WriteTest>("Test 12");



            var notFound = svc["key not found"];


        }

        [Fact]
        public void ReadAllStrings()
        {
            var svc = ServiceProvider.GetService<IStringLocalizer>();

            var allStrings = svc.GetAllStrings(true);

            Assert.NotEmpty(allStrings);
        }

        [Fact]
        public void ReadStringBase()
        {
            var svc = ServiceProvider.GetService<IStringLocalizer>();

            var tst0 = svc["Test 12"];
            var notFound = svc["key not found"];
        }

        [Fact]
        public void ReadTyped()
        {
            var svc = ServiceProvider.GetService<IStringLocalizer<WriteTest>>();

            var allStrings = svc.GetAllStrings(true);

            Assert.NotEmpty(allStrings);

        }

    }
}