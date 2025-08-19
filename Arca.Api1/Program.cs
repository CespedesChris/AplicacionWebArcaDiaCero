using Arca.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);
//-----------------------------------------------------------------------
// REGISTRAR REPOSITORIOS ANTES DE BUILD
builder.Services.AddScoped<SemillaRepository>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connectionString = config.GetConnectionString("DefaultConnection");
    return new SemillaRepository(connectionString);
});
builder.Services.AddScoped<UsuarioRepository>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connectionString = config.GetConnectionString("DefaultConnection");
    return new UsuarioRepository(connectionString);
});
builder.Services.AddScoped<EspecieRepository>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connectionString = config.GetConnectionString("DefaultConnection");
    return new EspecieRepository(connectionString);
});


//FIN DE REGISTRO DE REPOSITORIOS
//-----------------------------------------------------------------------


//------------------------------------------
// AGREGAR CONTROLADORES -------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();