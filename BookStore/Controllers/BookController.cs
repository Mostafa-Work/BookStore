using BookStore.Helpers;
using BookStore.Models;
using BookStore.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Runtime.CompilerServices;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BookStore.Controllers
{
    [Authorize]
    public class BookController : Controller
    {
        private readonly IBookRepository bookRepository;
        private readonly FilesHelper filesHelper;
        public BookController(IBookRepository bookRepository,FilesHelper filesHelper) 
        {
            this.bookRepository = bookRepository;
            this.filesHelper = filesHelper;
        }


        public async Task<IActionResult> AllBooks()
        {
            List<BookViewModel> models = await bookRepository.GetAllBooksAsync();
            return View(models);
        }

        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Create ()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(BookViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                if(viewModel.BookCover == null)
                    ModelState.AddModelError("BookCover", "please add Book Cover Image");

                if (viewModel.BookPdf == null)
                    ModelState.AddModelError("BookPdf", "please add Book PDF");

                if(viewModel.GalleryFiles == null)
                    ModelState.AddModelError("GalleryFiles", "please add Gallery Images");

                if (ModelState.ErrorCount>0)
                    return View();

                if (await bookRepository.AddNewBookAsync(viewModel) != 0)
                    return RedirectToAction("AllBooks");
                else
                    throw new Exception("Error Occured while adding new book");
            }

            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            BookViewModel model = await bookRepository.GetBookByIdAsync(id);
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if(await bookRepository.DeleteBookAsync(id)!=0)
                return RedirectToAction("AllBooks");

            throw new Exception("Error Occured While Deleting");
        }

        public async Task<IActionResult> Update(int id)
        {
            BookViewModel model = await bookRepository.GetBookByIdAsync(id);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(BookViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if(await bookRepository.UpdateBookAsync(viewModel)!=0)
                    return RedirectToAction("AllBooks");

                throw new Exception("Error occured While Updating");
            }
            return View();
        }
    }
}
