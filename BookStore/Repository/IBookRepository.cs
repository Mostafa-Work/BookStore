using BookStore.Models;

namespace BookStore.Repository
{
    public interface IBookRepository
    {
        public Task<int> AddNewBookAsync(BookViewModel viewModel);
        public Task<List<BookViewModel>> GetAllBooksAsync();
        public Task<BookViewModel> GetBookByIdAsync(int id);

        public Task<List<BookViewModel>> GetTopBooksAsync(int count);
        public Task<List<BookViewModel>> SearchBooksAsync(string title, string authorName);

        public Task<int> UpdateBookAsync(BookViewModel viewModel);
        public Task<int> DeleteBookAsync(int id);

    }
}
