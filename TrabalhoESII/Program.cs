using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TrabalhoESII.Models;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSession();
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        
        var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Cookies.ContainsKey("jwtToken"))
                        {
                            context.Token = context.Request.Cookies["jwtToken"];
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireClaim("TipoUtilizadorId", "1")); // "1" = Admin
            options.AddPolicy("ManagerOnly", policy =>
                policy.RequireClaim("TipoUtilizadorId", "2")); // "1" = Admin
        });


        builder.Services.AddControllersWithViews();

        var app = builder.Build();
        
        using (var scope = app.Services.CreateScope())
        {
            var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            if (!ctx.tiposutilizadores.Any())
            {
                ctx.tiposutilizadores.AddRange(
                    new tiposutilizadores { idtipoutilizador = 1, nome = "Admin" },
                    new tiposutilizadores { idtipoutilizador = 2, nome = "UserManager" },
                    new tiposutilizadores { idtipoutilizador = 3, nome = "User" }
                );
                ctx.SaveChanges();
            }

            if (!ctx.utilizadores.Any(u => u.idtipoutilizador == 1))
            {
                ctx.utilizadores.Add(new utilizadores
                {
                    nome = "Admin",
                    nomeutilizador = "admin",
                    senha = BCrypt.Net.BCrypt.HashPassword("admin"),
                    email = "admin@moderation.com",
                    nacionalidade = "Portugal",
                    idade = 30,
                    telefone = "000000000",
                    idtipoutilizador = 1
                });
                ctx.SaveChanges();
            }
        }
        
        app.UseSession();

        app.UseRouting();
        

        app.UseStaticFiles();
        app.UseAuthentication();
        app.UseAuthorization();   
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}