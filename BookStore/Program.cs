using BookStore.ConfigModels;
using BookStore.Data;
using BookStore.Helpers;
using BookStore.Models;
using BookStore.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<BookStoreContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")
    ));

builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<ILanguageRepository, LanguageRepository>();
builder.Services.AddScoped<FilesHelper, FilesHelper>();



builder.Services.Configure<CustomConfig1>(customConfig1 => 
{
    customConfig1.CustomConfigNumber = 1;
    customConfig1.CustomConfigName = "Test";
    customConfig1.CustomConfigDescription = "Test Description";
});

builder.Services.Configure<CustomConfig2>(builder.Configuration.GetSection("CustomConfig2"));


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
