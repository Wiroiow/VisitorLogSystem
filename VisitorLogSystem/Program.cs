using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using VisitorLogSystem.Data;
using VisitorLogSystem.Interfaces;
using VisitorLogSystem.Repositories;
using VisitorLogSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// ═══════════════════════════════════════════════════════════
// STEP 1: Add MVC Controllers with Views
// ═══════════════════════════════════════════════════════════
builder.Services.AddControllersWithViews();

// ═══════════════════════════════════════════════════════════
// STEP 2: Configure Database Connection
// ═══════════════════════════════════════════════════════════
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions.CommandTimeout(180)
    )
);

// ═══════════════════════════════════════════════════════════
// STEP 3: Configure Cookie Authentication
// ═══════════════════════════════════════════════════════════
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.Cookie.Name = "VisitorLogSystem.Auth";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

// ═══════════════════════════════════════════════════════════
// STEP 4: Register Dependency Injection Services
// ⚠️ THIS IS WHERE THE FIX IS!
// ═══════════════════════════════════════════════════════════

// ✅ Visitor Management Services (REQUIRED!)
builder.Services.AddScoped<IVisitorRepository, VisitorRepository>();
builder.Services.AddScoped<IVisitorService, VisitorService>();

// ✅ Authentication Services (REQUIRED!)
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

// ✅ Admin Services (REQUIRED!)
builder.Services.AddScoped<IUserManagementService, UserManagementService>();

builder.Services.AddScoped<IRoomVisitRepository, RoomVisitRepository>();
builder.Services.AddScoped<IRoomVisitService, RoomVisitService>();

builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

// ═══════════════════════════════════════════════════════════
// STEP 5: Configure Logging
// ═══════════════════════════════════════════════════════════
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// ═══════════════════════════════════════════════════════════
// BUILD THE APPLICATION
// ═══════════════════════════════════════════════════════════
var app = builder.Build();

// ═══════════════════════════════════════════════════════════
// STEP 6: Seed Database (Create admin if not exists)
// ═══════════════════════════════════════════════════════════
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Ensure database is created
        context.Database.EnsureCreated();

        // Check if admin exists
        if (!context.Users.Any(u => u.Username == "admin"))
        {
            var adminUser = new VisitorLogSystem.Models.User
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Role = "Admin",
                CreatedAt = DateTime.Now
            };

            context.Users.Add(adminUser);
            context.SaveChanges();

            Console.WriteLine("✅ Admin user created! Username: admin, Password: admin123");
        }
        else
        {
            Console.WriteLine("ℹ️ Admin user already exists");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error during database initialization: {ex.Message}");
    }
}

// ═══════════════════════════════════════════════════════════
// STEP 7: Configure HTTP Request Pipeline
// ═══════════════════════════════════════════════════════════

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// ⚠️ CRITICAL: Order matters!
app.UseAuthentication(); // First
app.UseAuthorization();  // Second

// ═══════════════════════════════════════════════════════════
// STEP 8: Map Controller Routes
// ═══════════════════════════════════════════════════════════
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

// ═══════════════════════════════════════════════════════════
// STEP 9: Run Application
// ═══════════════════════════════════════════════════════════
Console.WriteLine("🚀 Application started successfully!");
Console.WriteLine("📊 Default route: Dashboard/Index");
Console.WriteLine("🔐 Admin login: admin / admin123");

app.Run();