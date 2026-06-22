using Capa.Negocio;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddAuthentication(options => {
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options => {
    options.LoginPath = "/Login";
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
})
.AddGoogle(options => {
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "dummy-google-client-id";
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "dummy-google-client-secret";
});

var frontendProdUrl = builder.Configuration["FrontendUrl"];

builder.Services.AddCors(options => {
    options.AddPolicy("CorsPolicy", policy => {
        var origins = new List<string> { "http://localhost:5173", "http://localhost:5174" };
        
        if (!string.IsNullOrEmpty(frontendProdUrl))
        {
            origins.Add(frontendProdUrl);
        }

        policy.WithOrigins(origins.ToArray())
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddScoped<EquipoNegocio>();
builder.Services.AddScoped<UsuarioNegocio>();
builder.Services.AddScoped<PrestamoNegocio>();
builder.Services.AddScoped<CategoriaNegocio>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.UseCors("CorsPolicy");

app.UseSession();

app.Use(async (context, next) =>
{
    var headerId = context.Request.Headers["X-Usuario-ID"].FirstOrDefault();
    var headerNombre = context.Request.Headers["X-Usuario-Nombre"].FirstOrDefault();
    var headerRol = context.Request.Headers["X-Usuario-Rol"].FirstOrDefault();

    if (!string.IsNullOrEmpty(headerId) && int.TryParse(headerId, out int id))
    {
        context.Session.SetInt32("UsuarioID", id);
    }
    if (!string.IsNullOrEmpty(headerNombre))
    {
        context.Session.SetString("UsuarioNombre", headerNombre);
    }
    if (!string.IsNullOrEmpty(headerRol))
    {
        context.Session.SetString("UsuarioRol", headerRol);
    }

    await next.Invoke();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
