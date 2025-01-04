using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using sisae.Data;
using sisae.Services;
using System.Globalization;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Agregar configuración desde el archivo secrets.json
builder.Configuration.AddJsonFile("secrets.json", optional: true, reloadOnChange: true);

// Configurar DbContext con cadena de conexión de base de datos
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString)
           .EnableSensitiveDataLogging()); // Muestra parámetros en los logs

// Configurar ASP.NET Core Identity con roles (Solo se agrega una vez)
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Filtro para mostrar páginas de excepción para desarrolladores
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configurar ajustes de cookies de aplicación
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// Configurar el servicio de registro de eventos
builder.Services.AddScoped<EventLoggerService>();

// Configurar localización
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddControllersWithViews()
    .AddDataAnnotationsLocalization()
    .AddViewLocalization();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { new CultureInfo("es"), new CultureInfo("en") };
    options.DefaultRequestCulture = new RequestCulture("es");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

// Configurar Razor Pages con localización
builder.Services.AddRazorPages()
    .AddViewLocalization();

// Configurar SignalR
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.MaximumReceiveMessageSize = 512 * 1024; // Hasta 512 KB en un mensaje
});

// Configurar SignalRService con endpoint
builder.Services.AddTransient<SignalRService>(provider =>
    new SignalRService("http://localhost:5278/dashboardHub"));

// Configurar cliente HTTP para GoogleCloudVisionService
builder.Services.AddHttpClient<GoogleCloudVisionService>();

var emailSettings = builder.Configuration.GetSection("EmailSettings");
builder.Services.AddTransient<IEmailSender>(provider =>
    new SmtpEmailSender(
        smtpServer: emailSettings["SmtpServer"] ?? throw new ArgumentNullException("SmtpServer"),
        port: int.Parse(emailSettings["Port"] ?? "587"),
        username: emailSettings["Username"] ?? throw new ArgumentNullException("Username"),
        password: emailSettings["Password"] ?? throw new ArgumentNullException("Password")
    ));

var app = builder.Build();

// Configurar soporte para localización (español e inglés)
var supportedCultures = new[]
{
    new CultureInfo("es"),
    new CultureInfo("en")
};
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("es"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint(); // Punto de finalización de migraciones en desarrollo
}
else
{
    app.UseExceptionHandler("/Error"); // Manejo de excepciones
    app.UseHsts(); // Uso de HSTS en producción
}

app.UseHttpsRedirection(); // Redirección a HTTPS
app.UseStaticFiles(); // Servir archivos estáticos

app.UseRouting(); // Habilitar enrutamiento

app.UseAuthentication(); // Habilitar autenticación
app.UseAuthorization(); // Habilitar autorización

app.MapRazorPages(); // Mapear las Razor Pages
app.MapControllers();
app.MapFallbackToFile("index.html");
app.MapHub<DashboardHub>("/dashboardHub"); // Mapear el hub para SignalR

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    await SeedData.Initialize(scope.ServiceProvider, userManager);
}

app.Use(async (context, next) =>
{
    context.Items["Configuration"] = builder.Configuration;
    await next.Invoke();
});

app.Run(); // Ejecutar la aplicación
