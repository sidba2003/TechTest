using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Westwind.AspNetCore.Markdown;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services
    .AddDataAccess()
    .AddDomainServices()
    .AddMarkdown()
    .AddControllersWithViews();

var app = builder.Build();

app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseDefaultFiles();

app.UseRouting();
app.UseAuthorization();

// Map API controllers
app.MapControllers();

// Serve React app for any non-API route
app.MapFallbackToFile("react-app/dist/index.html");

app.Run();
