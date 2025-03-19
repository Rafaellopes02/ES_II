using Microsoft.EntityFrameworkCore;
using TrabalhoESII.Models;

var builder = WebApplication.CreateBuilder(args);

// Configurar o DbContext com PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Verificar conexão com a base de dados
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        if (dbContext.Database.CanConnect())
        {
            Console.WriteLine("Ligação à base de dados bem-sucedida!");
        }
        else
        {
            Console.WriteLine("Falha ao conectar à base de dados.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao conectar à base de dados: {ex.Message}");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();