using System.Diagnostics;
using System.Globalization;
using Kharazmi.AspNetCore.Core.Linq;
using Kharazmi.AspNetCore.Localization.EFCore;
using Kharazmi.AspNetCore.Localization.IntegrationTests.Infrastructure;
using Kharazmi.AspNetCore.Localization.IntegrationTests.Models;
using Kharazmi.AspNetCore.Localization.IntegrationTests.Resources;
using Kharazmi.AspNetCore.Localization.Json.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Kharazmi.AspNetCore.Localization.IntegrationTests.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILocalizerService _localizerCrud;
        private readonly IEfLocalizerService _efLocalizerService;
        private readonly IStringLocalizer<MyResource> _stringLocalizer;

        public HomeController(
            ILocalizerService localizerCrud,
//            IEfLocalizerService efLocalizerService,
            IStringLocalizer<MyResource> stringLocalizer,
            IStringLocalizerFactory localizerFactory) : base(localizerFactory)
        {
            _localizerCrud = localizerCrud;
//            _efLocalizerService = efLocalizerService;
            _stringLocalizer = stringLocalizer;
        }

        public IActionResult Index()
        {
//            EfCoreTest();
            JsonTest();

            return View();
        }

        private void JsonTest()
        {
            var f = _stringLocalizer["Test01"];
            var value = L("Test02");

            InsertTest();

            f = _stringLocalizer["Test01"];
            value = L("Test02");

            DeleteTest();


            f = _stringLocalizer["Test01"];
            value = L("Test02");
        }

        private void EfCoreTest()
        {
            var f = _stringLocalizer["Test01"];
            var value = L("Test02");


            InsertTest();

            value = L("Test01");

            var list = _efLocalizerService.Get(new DataSourceRequest()
            {
                Skip = 2,
                Take = 5,
                Filter = new Filter
                {
                    Logic = "and",
                    Filters = new List<Filter>
                    {
                        new Filter
                        {
                            Field = nameof(LocalizationEntity.Key),
                            Operator = "contains",
                            Value = "Key02"
                        },
                        new Filter
                        {
                            Field = nameof(LocalizationEntity.Value),
                            Operator = "contains",
                            Value = "Key Value02"
                        },
                        new Filter
                        {
                            Field = nameof(LocalizationEntity.CultureName),
                            Operator = "contains",
                            Value = CultureInfo.CurrentCulture.Name
                        },
                        new Filter
                        {
                            Field = nameof(LocalizationEntity.Resource),
                            Operator = "contains",
                            Value = typeof(MyResource).GetComputedCacheKey("Key02", CultureInfo.CurrentCulture.Name)
                        }
                    }
                }
            });

            var list2 = _efLocalizerService.GetPaged(predicate: x => x.CultureName == CultureInfo.CurrentCulture.Name);

            var test01 = _efLocalizerService.Find(typeof(MyResource), "Test01", CultureInfo.CurrentUICulture.Name);
            var test02 = _efLocalizerService.Find(typeof(MyResource), "Test02", CultureInfo.CurrentUICulture.Name);

            DeleteTest();

            test01 = _efLocalizerService.Find(typeof(MyResource), "Test01", CultureInfo.CurrentUICulture.Name);
            test02 = _efLocalizerService.Find(typeof(MyResource), "Test02", CultureInfo.CurrentUICulture.Name);
        }

        private void DeleteTest()
        {
            _localizerCrud.Delete("Test01", CultureInfo.CurrentCulture.Name, typeof(MyResource));
            _localizerCrud.Delete("Test02", CultureInfo.CurrentCulture.Name, typeof(MyResource));
        }

        private void InsertTest()
        {
            _localizerCrud.Insert("Test01", "Test Value", CultureInfo.CurrentCulture.Name, typeof(MyResource));
            _localizerCrud.Insert("Test02", "Test Value02", CultureInfo.CurrentCulture.Name, typeof(MyResource));
            _localizerCrud.Insert("Test03", "Test Value03", CultureInfo.CurrentCulture.Name, typeof(MyResource));
            _localizerCrud.Insert("Test04", "Test Value04", CultureInfo.CurrentCulture.Name, typeof(MyResource));
            _localizerCrud.Insert("Test05", "Test Value05", CultureInfo.CurrentCulture.Name, typeof(MyResource));
            _localizerCrud.Insert("Test06", "Test Value06", CultureInfo.CurrentCulture.Name, typeof(MyResource));
            _localizerCrud.Insert("Test07", "Test Value07", CultureInfo.CurrentCulture.Name, typeof(MyResource));
            _localizerCrud.Insert("Test08", "Test Value08", CultureInfo.CurrentCulture.Name, typeof(MyResource));
            _localizerCrud.Insert("Test09", "Test Value09", CultureInfo.CurrentCulture.Name, typeof(MyResource));
            _localizerCrud.Insert("Test10", "Test Value10", CultureInfo.CurrentCulture.Name, typeof(MyResource));
            _localizerCrud.Insert("Test11", "Test Value11", CultureInfo.CurrentCulture.Name, typeof(MyResource));
            _localizerCrud.Insert("Test12", "Test Value12", CultureInfo.CurrentCulture.Name, typeof(MyResource));
            _localizerCrud.Insert("Test13", "Test Value13", CultureInfo.CurrentCulture.Name, typeof(MyResource));


            _localizerCrud.Insert("Key01", "Key Value", CultureInfo.CurrentCulture.Name, typeof(MyResource_2));
            _localizerCrud.Insert("Key02", "Key Value02", CultureInfo.CurrentCulture.Name, typeof(MyResource_2));
            _localizerCrud.Insert("Key03", "Key Value03", CultureInfo.CurrentCulture.Name, typeof(MyResource_2));
            _localizerCrud.Insert("Key04", "Key Value04", CultureInfo.CurrentCulture.Name, typeof(MyResource_2));
            _localizerCrud.Insert("Key05", "Key Value05", CultureInfo.CurrentCulture.Name, typeof(MyResource_2));
            _localizerCrud.Insert("Key06", "Key Value06", CultureInfo.CurrentCulture.Name, typeof(MyResource_2));
            _localizerCrud.Insert("Key07", "Key Value07", CultureInfo.CurrentCulture.Name, typeof(MyResource_2));
            _localizerCrud.Insert("Key08", "Key Value08", CultureInfo.CurrentCulture.Name, typeof(MyResource_2));
            _localizerCrud.Insert("Key09", "Key Value09", CultureInfo.CurrentCulture.Name, typeof(MyResource_2));
            _localizerCrud.Insert("Key10", "Key Value10", CultureInfo.CurrentCulture.Name, typeof(MyResource_2));
            _localizerCrud.Insert("Key11", "Key Value11", CultureInfo.CurrentCulture.Name, typeof(MyResource_2));
            _localizerCrud.Insert("Key12", "Key Value12", CultureInfo.CurrentCulture.Name, typeof(MyResource_2));
            _localizerCrud.Insert("Key13", "Key Value13", CultureInfo.CurrentCulture.Name, typeof(MyResource_2));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}