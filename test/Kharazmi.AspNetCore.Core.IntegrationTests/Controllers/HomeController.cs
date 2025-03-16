using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Dispatchers;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.IntegrationTests.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Kharazmi.AspNetCore.Core.IntegrationTests.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDomainDispatcher _dispatcher;
        private readonly IDomainContextService _domainContextService;



        public HomeController(ILogger<HomeController> logger, IDomainDispatcher dispatcher,
            IDomainContextService domainContextService)
        {
            _logger = logger;
            _dispatcher = dispatcher;
            _domainContextService = domainContextService;
        }

        public async Task<IActionResult> Index()
        {
            var domainContext = await _domainContextService.GetAsync<AddUserCommand>().ConfigureAwait(false);
            var result = await _dispatcher.SendCommandAsync(new AddUserCommand("", "koorsha"), domainContext,
                CancellationToken.None).ConfigureAwait(false);

     
            
            return View();
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