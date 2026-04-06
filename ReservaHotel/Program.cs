using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReservaHotel.Data;
using ReservaHotel.Models; // Asegúrate de tener esta línea para ApplicationUser
using Rotativa.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración de la Base de Datos
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 2. CONFIGURACIÓN DE IDENTITY (CORREGIDA)
// Cambiamos IdentityUser por ApplicationUser y agregamos .AddRoles<IdentityRole>()
builder.Services.AddDefaultIdentity<ApplicationUser>(options => {
    options.SignIn.RequireConfirmedAccount = false; // Cambiado a false para facilitar pruebas en el lab
    options.Password.RequireDigit = false;          // Opcional: hace las contraseñas más simples para el proyecto
    options.Password.RequiredLength = 4;
})
    .AddRoles<IdentityRole>() // ¡IMPORTANTE para el Dashboard de Admin!
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

// 3. Configuración de Rotativa (PDF)
// Asegúrate de que la carpeta "Rotativa" exista en wwwroot
//RotativaConfiguration.Setup(builder.Environment.WebRootPath, "Rotativa");

var app = builder.Build();

// 4. Pipeline de HTTP
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Importante para cargar CSS/JS de Bootstrap

app.UseRouting();

app.UseAuthentication(); // ¡DEBE ir antes de Authorization!
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();