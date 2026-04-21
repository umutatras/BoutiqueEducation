using BoutiqueEducation.API.Filters;
using BoutiqueEducation.API.Hubs;
using BoutiqueEducation.API.Middlewares;
using BoutiqueEducation.Business.Extensions;
using BoutiqueEducation.Business.Settings;
using BoutiqueEducation.DataAccess.Context;
using BoutiqueEducation.DataAccess.Extensions;
using BoutiqueEducation.DataAccess.Seed;
using BoutiqueEducation.Entity.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// CORS - Angular dev server + SignalR WebSocket geçişi için zorunlu
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "http://localhost:56968", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // SignalR için AllowCredentials şarttır
    });
});


// 1. AppSettings - Options Pattern Kaydı
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<GoogleAuthSettings>(builder.Configuration.GetSection("GoogleAuth"));

// DbContext Kaydı
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Identity Kayıtları
builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// 3. Extension Metotlar ile Katmanlı Dependency Injection
builder.Services.AddDataAccessServices();
builder.Services.AddBusinessServices();

// 4. JWT Authentication Ayarları
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

// Production'da JWT Secret'ı Environment Variable'dan oku (güvenlik)
var jwtSecret = builder.Configuration["JWT_SECRET_KEY"]
    ?? jwtSettings?.SecretKey
    ?? throw new InvalidOperationException("JWT SecretKey yapılandırılmamış!");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings!.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
    };
    // SignalR WebSocket bağlantıları için query string'den token okuma
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chathub"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

// SignalR Kaydı (Chat için)
builder.Services.AddSignalR();

// FluentValidation vs ModelState.IsValid cakismasini onlemek icin default davranisi kapatiyoruz
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>(); // Tum API'lere Filter uzerinden FluentValidation eklendi
});
// OpenAPI / Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

// Seed: Veritabanını migration sonrası test verileriyle doldur
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();  // Migration yoksa uygular

        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
        await DatabaseSeeder.SeedAsync(userManager, roleManager, dbContext);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Seed sırasında hata oluştu.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Boutique Education API");
    });
}

app.UseStaticFiles(); // wwwroot/uploads klasöründeki dosyaları sun

app.UseHttpsRedirection();

// CORS - Authentication'dan ÖNCE gelmelidir
app.UseCors("AngularPolicy");

// Sıralama Önemlidir: Önce Authentication (Kimlik) sonra Authorization (Yetki)
app.UseAuthentication();
app.UseAuthorization();

// AuditLogMiddleware'i Authentication'dan hemen sonra koyuyoruz ki giriş yapanın ID'sini çözebilsin
app.UseMiddleware<AuditLogMiddleware>();

app.MapControllers();

// SignalR endpoint'i
app.MapHub<ChatHub>("/chathub");

app.Run();
