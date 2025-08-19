using Arca.Data.Repositories;
using System.Net.Http;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Drawing;
using QuestPDF.Elements;

var builder = WebApplication.CreateBuilder(args);
// Al inicio de tu método ExportarPdf o en Program.cs antes de generar PDFs:
QuestPDF.Settings.License = LicenseType.Community;

// Add services to the container.
builder.Services.AddControllersWithViews();


// HttpClient con BaseAddress desde appsettings
//var apiBase = builder.Configuration["ApiBaseUrl"];
//builder.Services.AddHttpClient("ArcaApi", client =>
//{
  //  client.BaseAddress = new Uri("https://localhost:7029/api/");
//});

// Registrar HttpClientFactory //----esto lo agregue el domungo viendo a ver si sirve el API    
builder.Services.AddHttpClient("ArcaApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7029/api/"); // URL donde corre Arca.Api
});


// Registrar repositorio en MVC DI
builder.Services.AddScoped<SemillaRepository>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connectionString = config.GetConnectionString("DefaultConnection");
    return new SemillaRepository(connectionString);
});


builder.Services.AddScoped<ReporteProgramadoRepository>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var cs = config.GetConnectionString("DefaultConnection");
    return new ReporteProgramadoRepository(cs);
});


builder.Services.AddScoped<EmailService>();


// Se agregó para LOGIN
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // duración de la sesión
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();
app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
//pattern: "{controller=Usuario}/{action=Index}/{id?}");
//app.MapControllers();
app.Run();
