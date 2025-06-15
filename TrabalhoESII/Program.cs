using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TrabalhoESII.Models;

var builder = WebApplication.CreateBuilder(args);

// ───── Serviços ────────────────────────────────────────────────

// Sessões
builder.Services.AddSession();

// DbContext (PostgreSQL)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey     = new SymmetricSecurityKey(key),
            ValidateIssuer       = false,
            ValidateAudience     = false
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                if (ctx.Request.Cookies.ContainsKey("jwtToken"))
                    ctx.Token = ctx.Request.Cookies["jwtToken"];
                return Task.CompletedTask;
            }
        };
    });

// Políticas de autorização
builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("AdminOnly",   p => p.RequireClaim("TipoUtilizadorId", "1"));
    opts.AddPolicy("ManagerOnly", p => p.RequireClaim("TipoUtilizadorId", "2"));
});

// MVC + Razor
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ───── Pipeline ────────────────────────────────────────────────

app.UseSession();

app.UseRouting();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
