using GitHubRepoAdmin.Business.Contract;
using GitHubRepoAdmin.Business.Services;
using GitHubRepoAdmin.Domain.Interfaces;
using GitHubRepoAdmin.Infra.ApiGitHub;
using GitHubRepoAdmin.Infra.Contract;
using GitHubRepoAdmin.Infra.Database;
using GitHubRepoAdmin.Infra.Middlewares;
using GitHubRepoAdmin.Infra.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SpaServices.AngularCli; // Importante!

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<HttpClient>();
builder.Services.AddScoped<IGitHubApi, GitHubApi>();
builder.Services.AddScoped<IGitHubApiBusiness, GitHubApiBusiness>();
builder.Services.AddScoped<IFavoritesRepository, FavoritesRepository>();
builder.Services.AddScoped<IFavoritesBusiness, FavoritesBusiness>();
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlite(configuration.GetConnectionString("SQLite")));
builder.Services.AddCors(policyBuilder =>
    policyBuilder.AddDefaultPolicy(policy =>
        policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod())
);

builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "ClientApp/dist"; // onde o Angular gera o build
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseMiddleware(typeof(GlobalExceptionHandling));

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSpaStaticFiles(); // serve arquivos do build do Angular em produ��o

app.UseRouting();
app.UseCors();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

// Config SPA (Angular)
app.UseSpa(spa =>
{
    spa.Options.SourcePath = "ClientApp";

    if (app.Environment.IsDevelopment())
    {
        spa.UseAngularCliServer(npmScript: "start"); // Roda npm start autom�tico no dev
    }
});

app.Run();
