using BookStore.ConfigModels;
using BookStore.Models;
using BookStore.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace BookStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBookRepository _bookRepository;
        private readonly CustomConfig1 customConfig1;
        private readonly CustomConfig2 customConfig2;

        public HomeController(ILogger<HomeController> logger, IBookRepository bookRepository,
            IOptions<CustomConfig1> customConfig1Option, IOptions<CustomConfig2> customConfig2Option)
        {
            _logger = logger;
            _bookRepository = bookRepository;
            customConfig1 = customConfig1Option.Value;
            customConfig2 = customConfig2Option.Value;
        }

        public async Task<IActionResult> Index()
        {

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}