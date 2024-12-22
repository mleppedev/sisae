using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using sisae.Data;
using static sisae.Pages.Visitantes.CreateModel;
using sisae.Services;

var builder = WebApplication.CreateBuilder(args);

// Agregar configuración desde el archivo secrets.json, opcional y se recarga si cambia
builder.Configuration.AddJsonFile("secrets.json", optional: true, reloadOnChange: true);

// Agregar servicios al contenedor.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString)
           .EnableSensitiveDataLogging()); // Esto muestra parámetros en los logs

// Filtro para mostrar páginas de excepción para desarrolladores
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configurar ASP.NET Core Identity con confirmación de cuenta requerida
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Configurar ajustes de cookies de aplicación
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login"; // Ruta a la página de inicio de sesión
    options.AccessDeniedPath = "/Identity/Account/AccessDenied"; // Ruta para acceso denegado
});

// Configurar el servicio de registro de eventos
builder.Services.AddScoped<EventLoggerService>();

// Agregar soporte para Razor Pages
builder.Services.AddRazorPages();

// Agregar soporte para Hubs
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;

    // Permitir hasta 512 KB en el mensaje
    options.MaximumReceiveMessageSize = 512 * 1024; 
});

builder.Services.AddTransient<SignalRService>(provider => 
    new SignalRService("http://localhost:5278/dashboardHub"));

// Configurar cliente HTTP para GoogleCloudVisionService
builder.Services.AddHttpClient<GoogleCloudVisionService>();

// Agregar EventLoggerService como servicio transitorio
builder.Services.AddTransient<EventLoggerService>();

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint(); // Usar punto de finalización de migraciones en desarrollo
}
else
{
    app.UseExceptionHandler("/Error"); // Manejo de excepciones con una página de error
    app.UseHsts(); // Seguridad con HSTS en producción
}

app.UseHttpsRedirection(); // Redirección a HTTPS
app.UseStaticFiles(); // Servir archivos estáticos

app.UseRouting(); // Habilitar el enrutamiento

app.UseAuthentication(); // Habilitar autenticación de usuarios
app.UseAuthorization(); // Habilitar autorización de usuarios

app.MapRazorPages(); // Mapear las Razor Pages

app.MapHub<DashboardHub>("/dashboardHub");

app.Run(); // Ejecutar la aplicación
