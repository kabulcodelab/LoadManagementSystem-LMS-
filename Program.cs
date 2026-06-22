using LoadManagementSystem_LMS_.Components;
using LoadManagementSystem_LMS_.Components.Account;
using LoadManagementSystem_LMS_.Data;
using LoadManagementSystem_LMS_.Models;
using LoadManagementSystem_LMS_.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// ================================================
// Add services to the container.
// ================================================
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ================================================
// Register application services
// ================================================
builder.Services.AddScoped<DriverService>();
builder.Services.AddScoped<VehicleService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<LoadService>();
builder.Services.AddScoped<DocumentService>();
builder.Services.AddScoped<StopService>();
builder.Services.AddMudServices();
builder.Services.AddScoped<DashboardService>();

// ================================================
// Authentication & Authorization
// ================================================
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
.AddIdentityCookies();

// ================================================
// Database Configuration
// ================================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// ================================================
// Identity Configuration with Roles
// ================================================
builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    // Disable automatic email confirmation - admin will confirm manually
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>() // Enable Role Management
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddSignInManager()
.AddDefaultTokenProviders();

// ================================================
// Email sender (no-op for development)
// ================================================
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

// ================================================
// Configure the HTTP request pipeline.
// ================================================
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();