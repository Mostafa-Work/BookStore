﻿using BookStore.Models;
using BookStore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using BookStore.Helpers;

namespace BookStore.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly BookStoreContext context;
        private readonly IWebHostEnvironment env;
        private readonly FilesHelper filesHelper;
        public BookRepository(BookStoreContext context,IWebHostEnvironment env, FilesHelper filesHelper) 
        {
            this.context = context;
            this.env = env;
            this.filesHelper = filesHelper;
        }
        public async Task<int> AddNewBookAsync(BookViewModel viewModel)
        {
            Book newBook = new Book()
            {
                Title = viewModel.Title,
                Author = viewModel.Author,
                Description = viewModel.Description,
                LanguageId = viewModel.LanguageId,
                Category = viewModel.Category,
                CoverImageUrl = Guid.NewGuid().ToString()+"_"+viewModel.BookCover.FileName,
                BookPdfUrl = Guid.NewGuid().ToString() + "_" + viewModel.BookPdf.FileName,
                TotalPages= viewModel.TotalPages,
                CreatedOn=DateTime.UtcNow,
                UpdatedOn=DateTime.UtcNow,
            };

            newBook.Gallery =viewModel.GalleryFiles.Select(x => new GalleryImage()
            {
                Name = x.Name,
                Url = Guid.NewGuid().ToString()+"_"+x.FileName
            }).ToList();

            await context.Books.AddAsync(newBook);
            await context.SaveChangesAsync();

            //Upload New Files
            await filesHelper.UploadFileAsync(Path.Combine(env.WebRootPath, "book/cover", newBook.CoverImageUrl),
                viewModel.BookCover);
            await filesHelper.UploadFileAsync(Path.Combine(env.WebRootPath, "book/pdf", newBook.BookPdfUrl), 
                viewModel.BookPdf);
            for(int i=0;i<viewModel.GalleryFiles.Count;i++)
                await filesHelper.UploadFileAsync(Path.Combine(env.WebRootPath, "book/gallery", newBook.Gallery[i].Url), 
                    viewModel.GalleryFiles[i]);
            
            return newBook.Id;
        }

        public async Task<int> DeleteBookAsync(int id)
        {
            Book book= context.Books.Include(x=>x.Gallery).FirstOrDefault(x=>x.Id==id);
            if (book != null)
            {
                List<string> filesToBeDeleted = new List<string>();

                filesToBeDeleted.Add(Path.Combine(env.WebRootPath, "book/cover", book.CoverImageUrl));
                filesToBeDeleted.Add(Path.Combine(env.WebRootPath, "book/pdf", book.BookPdfUrl));
                foreach (var galleryImage in book.Gallery)
                    filesToBeDeleted.Add(Path.Combine(env.WebRootPath,"book/gallery",galleryImage.Url));

                context.Books.Remove(book);
                await context.SaveChangesAsync();

                foreach (var filePath in filesToBeDeleted)
                    await filesHelper.DeleteFileAsync(filePath);
                return id;
            }
            return 0;
        }

        public async Task<List<BookViewModel>> GetAllBooksAsync()
        {
            return await context.Books.Select(book => new BookViewModel()
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Description = book.Description,
                LanguageId = book.LanguageId,
                Category = book.Category,
                CoverImageUrl = book.CoverImageUrl,
                TotalPages = book.TotalPages,
                Language = book.Language.Name

            }).ToListAsync();
        }

        public async Task<BookViewModel> GetBookByIdAsync(int id)
        {

            IQueryable<Book> bookquery = context.Books.Where(x => x.Id == id);

            bookquery = bookquery.Include(x => x.Gallery).Include(x=>x.Language);

            Book x =await  bookquery.FirstOrDefaultAsync();

            return new BookViewModel()
            {
                Id = x.Id,
                Title = x.Title,
                Author = x.Author,
                Description = x.Description,
                LanguageId = x.LanguageId,
                Category = x.Category,
                BookPdfUrl = x.BookPdfUrl,
                CoverImageUrl = x.CoverImageUrl,
                TotalPages = x.TotalPages,
                Language = x.Language.Name,
                Gallery = x.Gallery.Select(x => new GalleryImageViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Url = x.Url
                }).ToList()
            };

            //return await context.Books.Where(x => x.Id == id).Select(x => new BookViewModel()
            //{
            //    Id = x.Id,
            //    Title = x.Title,
            //    Author = x.Author,
            //    Description = x.Description,
            //    LanguageId = x.LanguageId,
            //    Category = x.Category,
            //    BookPdfUrl = x.BookPdfUrl,
            //    CoverImageUrl = x.CoverImageUrl,
            //    TotalPages = x.TotalPages,
            //    Language = x.Language.Name,
            //    Gallery =x.Gallery.Where(x=>x.Id > 1).Select(x => new GalleryImageViewModel()
            //    {
            //        Id = x.Id,
            //        Name = x.Name,
            //        Url = x.Url
            //    }).ToList()
            //}).FirstOrDefaultAsync();
        }

        public async Task<List<BookViewModel>> GetTopBooksAsync(int count)
        {
            return await context.Books.Take(count).Select(book => new BookViewModel()
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Description = book.Description,
                LanguageId = book.LanguageId,
                Category = book.Category,
                CoverImageUrl = book.CoverImageUrl,
                TotalPages = book.TotalPages,
                Language = book.Language.Name

            }).ToListAsync();
        }

        public async Task<List<BookViewModel>> SearchBooksAsync(string title, string authorName)
        {
            return await context.Books.Where(x=> x.Title.Contains(title)&&x.Author==authorName).Select(book => new BookViewModel()
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Description = book.Description,
                LanguageId = book.LanguageId,
                Category = book.Category,
                CoverImageUrl = book.CoverImageUrl,
                TotalPages = book.TotalPages,
                Language = book.Language.Name

            }).ToListAsync();
        }

        public async Task<int> UpdateBookAsync(BookViewModel viewModel)
        {
            Book bookToBeUpdated = await context.Books.Include(x=>x.Gallery).Where(x=>x.Id==viewModel.Id).FirstOrDefaultAsync();
            if (bookToBeUpdated == null)
                return 0;

            List<string> filesToBeDeleted =new List<string>();

            //Check for files to be Deleted and generate new Names
            if (viewModel.BookCover != null)
            {
                filesToBeDeleted.Add(Path.Combine(env.WebRootPath, "book/cover",bookToBeUpdated.CoverImageUrl));
                bookToBeUpdated.CoverImageUrl = Guid.NewGuid().ToString()+"_"+viewModel.BookCover.FileName;
            }

            if (viewModel.BookPdf != null)
            {
                filesToBeDeleted.Add(Path.Combine(env.WebRootPath, "book/pdf", bookToBeUpdated.BookPdfUrl));
                bookToBeUpdated.BookPdfUrl = Guid.NewGuid().ToString() + "_" + viewModel.BookPdf.FileName;
            }

            if (viewModel.GalleryFiles != null)
            {
                foreach (var galleryImage in bookToBeUpdated.Gallery)
                    filesToBeDeleted.Add(Path.Combine(env.WebRootPath, "book/gallery", galleryImage.Url));

                //Remove existing images from db and adding new ones (Include+Reassigning)
                bookToBeUpdated.Gallery = viewModel.GalleryFiles.Select(x => new GalleryImage()
                {
                    Name = x.FileName,
                    Url = Guid.NewGuid().ToString() + "_" + x.FileName
                }).ToList();
            }
            //Update The Book To be Updated
            bookToBeUpdated.Author = viewModel.Author;
            bookToBeUpdated.Description = viewModel.Description;
            bookToBeUpdated.Category = viewModel.Category;
            bookToBeUpdated.UpdatedOn=DateTime.Now;
            bookToBeUpdated.Title = viewModel.Title;
            bookToBeUpdated.LanguageId = viewModel.LanguageId;
            bookToBeUpdated.TotalPages = viewModel.TotalPages;

            await context.SaveChangesAsync();

            //Delete Old Files
            foreach(string filPath in filesToBeDeleted)
                await filesHelper.DeleteFileAsync(filPath);

            //Upload New Files
            if (viewModel.BookCover != null)
                await filesHelper.UploadFileAsync(Path.Combine(env.WebRootPath, "book/cover", bookToBeUpdated.CoverImageUrl),
                    viewModel.BookCover);

            if (viewModel.BookPdf != null)
                await filesHelper.UploadFileAsync(Path.Combine(env.WebRootPath, "book/pdf", bookToBeUpdated.BookPdfUrl),
                    viewModel.BookPdf);
            if(viewModel.GalleryFiles != null)
            {
                for (int i = 0; i < viewModel.GalleryFiles.Count; i++)
                {
                    await filesHelper.UploadFileAsync(Path.Combine(env.WebRootPath, "book/gallery", bookToBeUpdated.Gallery[i].Url), 
                        viewModel.GalleryFiles[i]);
                }
            }
            return bookToBeUpdated.Id;
        }
    }
}
