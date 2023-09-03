using BookStore.Models;

namespace BookStore.Repository
{
    public interface ILanguageRepository
    {
        public Task<List<LanguageViewModel>> GetAllLanguagesAsync();
    }
}
