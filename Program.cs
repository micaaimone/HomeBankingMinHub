using HomeBankingMinHub.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

//le agrego un servicio que agrega un contexto a mi aplicacion a mi builder 
builder.Services.AddDbContext<HomeBankingContext>(
    options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HomeBankingConexion"))
    );
//con esto mi app ya esta conectada con mi base de datos 

    var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<HomeBankingContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ha ocurrido un error al enviar la información a la base de datos!");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
