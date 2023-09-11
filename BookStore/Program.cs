using BookStore.ConfigModels;
using BookStore.Data;
using BookStore.Helpers;
using BookStore.Models;
using BookStore.Repository;
using BookStore.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();

#if DEBUG
    builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
#endif

builder.Services.AddDbContext<BookStoreContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("Default"),
                sqlServerOptions =>
                {
                    sqlServerOptions.CommandTimeout(3600);
                    sqlServerOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
                }));

//builder.Services.AddDbContext<BookStoreContext>(options => 
//    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")
//    ));

builder.Services.AddIdentity<ApplicationUser,IdentityRole>().AddEntityFrameworkStores<BookStoreContext>().AddDefaultTokenProviders();


builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<ILanguageRepository, LanguageRepository>();
builder.Services.AddScoped<FilesHelper, FilesHelper>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationUserClaimsPrincipalFactory>();


builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 5;
    options.Password.RequiredUniqueChars = 1;

    options.SignIn.RequireConfirmedEmail = true;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(20);
    options.Lockout.MaxFailedAccessAttempts = 4;
});

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromMinutes(5);
});

builder.Services.Configure<CustomConfig1>(customConfig1 => 
{
    customConfig1.CustomConfigNumber = 1;
    customConfig1.CustomConfigName = "Test";
    customConfig1.CustomConfigDescription = "Test Description";
});
builder.Services.Configure<SMTPConfig>(builder.Configuration.GetSection("SMTPConfig"));

builder.Services.Configure<CustomConfig2>(builder.Configuration.GetSection("CustomConfig2"));


//////////////////////////////////////////////////((((((((((((((MiddleWares)))))))))))))))//////////////////////////////////////////////////
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
