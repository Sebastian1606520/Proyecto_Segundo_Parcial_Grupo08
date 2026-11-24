using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MoldeMVC_Core.Data;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MoldeMVC_Core.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//***************** MongoDB: datos de la aerolinea*************************************
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDb"));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddSingleton(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings.DatabaseName);
});

builder.Services.AddSingleton<AerolineaMongoContext>();


//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();

//******************** Configurar la autenticación *********************************
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // Desactivar los requisitos de la contraseña
    options.Password.RequireDigit = false; // No requiere dígitos
    options.Password.RequireLowercase = false; // No requiere minúsculas
    options.Password.RequireUppercase = false; // No requiere mayúsculas
    options.Password.RequireNonAlphanumeric = false; // No requiere caracteres especiales
    options.Password.RequiredLength = 3; // Longitud mínima de 3 caracteres

    // No requerimos la confirmación de cuenta para iniciar sesión
    options.SignIn.RequireConfirmedAccount = false;

    // Configuramos la validación de correo
    options.User.RequireUniqueEmail = false;
}).AddEntityFrameworkStores<ApplicationDbContext>();
//*******************************************************************************
builder.Services.AddControllersWithViews();

var app = builder.Build();

//************** Aplicar Migraciones Automáticamente **********************
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate(); // Aplica las migraciones pendientes
    }
    catch (Exception ex)
    {
        // Registra errores en el log en caso de que falle
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}
//***********************************************************************

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
