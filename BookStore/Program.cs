using BookStore.Data;
using BookStore.Helpers;
using BookStore.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddDbContext<BookStoreContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")
    ));

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<ILanguageRepository, LanguageRepository>();
builder.Services.AddScoped<FilesHelper, FilesHelper>();


#if DEBUG
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
#endif

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
