using BookStore.Models;
using BookStore.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BookStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBookRepository _bookRepository;

        public HomeController(ILogger<HomeController> logger, IBookRepository bookRepository)
        {
            _logger = logger;
            _bookRepository = bookRepository;
        }

        public async Task<IActionResult> Index()
        {
            //BookViewModel model = await _bookRepository.GetBookByIdAsync(2);
            //string s = "";
            //foreach (var image in model.Gallery)
            //    s += " " + image.Name;
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