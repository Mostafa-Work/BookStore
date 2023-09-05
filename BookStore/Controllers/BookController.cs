using BookStore.Helpers;
using BookStore.Models;
using BookStore.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Runtime.CompilerServices;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BookStore.Controllers
{
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

        public async Task<IActionResult> Create ()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(BookViewModel model)
        {
            if(ModelState.IsValid)
            {
                if(model.BookCover != null)
                {
                    model.CoverImageUrl= await filesHelper.UploadFileAsync("book/cover", model.BookCover);
                }
                else
                    ModelState.AddModelError("BookCover", "please add Book Cover Image");


                if (model.BookPdf != null)
                {
                    model.BookPdfUrl = await filesHelper.UploadFileAsync("book/pdf", model.BookPdf);
                }
                else
                    ModelState.AddModelError("BookPdf", "please add Book PDF");

                model.Gallery = new List<GalleryImageViewModel>();
                if(model.GalleryFiles != null)
                {
                    foreach (var imageFile in model.GalleryFiles)
                        model.Gallery.Add(new GalleryImageViewModel()
                        {
                            Name = imageFile.FileName,
                            Url = await filesHelper.UploadFileAsync("book/gallery", imageFile)
                        }); ;
                }
                else
                    ModelState.AddModelError("GalleryFiles", "please add Gallery Images");

                if (ModelState.ErrorCount>0)
                    return View();

                await bookRepository.AddNewBookAsync(model);
                return RedirectToAction("AllBooks");
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
            await bookRepository.DeleteBookAsync(id);
            return RedirectToAction("AllBooks");
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
                if (viewModel.BookCover != null)
                {
                    viewModel.CoverImageUrl = await filesHelper.UploadFileAsync("book/cover", viewModel.BookCover);
                }

                if (viewModel.BookPdf != null)
                {
                    viewModel.BookPdfUrl = await filesHelper.UploadFileAsync("book/pdf", viewModel.BookPdf);
                }

                viewModel.Gallery = new List<GalleryImageViewModel>();

                if (viewModel.GalleryFiles != null)
                {
                    foreach (var imageFile in viewModel.GalleryFiles)
                        viewModel.Gallery.Add(new GalleryImageViewModel()
                        {
                            Name = imageFile.FileName,
                            Url = await filesHelper.UploadFileAsync("book/gallery", imageFile)
                        }); ;
                }

                await bookRepository.UpdateBookAsync(viewModel);
                return RedirectToAction("AllBooks");
            }

            return View();
        }


    }
}
