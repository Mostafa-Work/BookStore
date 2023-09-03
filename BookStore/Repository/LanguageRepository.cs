using BookStore.Data;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Repository
{
    public class LanguageRepository : ILanguageRepository
    {
        private readonly BookStoreContext context;
        public LanguageRepository(BookStoreContext context)
        {
            this.context = context;
        }
        public async Task<List<LanguageViewModel>> GetAllLanguagesAsync()
        {
            return await context.Languages.Select(x => new LanguageViewModel()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            }).ToListAsync();
        }
    }
}
