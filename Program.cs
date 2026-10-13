using AcxiomCRM.Data; using AcxiomCRM.Models; using AcxiomCRM.Services; using Microsoft.AspNetCore.Identity; using Microsoft.EntityFrameworkCore; using Microsoft.AspNetCore.Mvc;
var b = WebApplication.CreateBuilder(args);
b.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(b.Configuration.GetConnectionString("Default")));
b.Services.AddIdentity<AppUser, IdentityRole>(o => {
  o.Password.RequiredLength = 8; o.Password.RequireDigit = o.Password.RequireUppercase = o.Password.RequireLowercase = o.Password.RequireNonAlphanumeric = true;
  o.Lockout.MaxFailedAccessAttempts = 5; o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); o.Lockout.AllowedForNewUsers = true; o.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
b.Services.ConfigureApplicationCookie(o => { o.LoginPath = "/Account/Login"; o.AccessDeniedPath = "/Account/Denied"; o.Cookie.HttpOnly = true;
  o.Cookie.SecurePolicy = CookieSecurePolicy.Always; o.ExpireTimeSpan = TimeSpan.FromMinutes(30); o.SlidingExpiration = true;
  o.Events.OnRedirectToLogin = c => { if (c.Request.Path.StartsWithSegments("/api")) c.Response.StatusCode = 401; else c.Response.Redirect(c.RedirectUri); return Task.CompletedTask; };
  o.Events.OnRedirectToAccessDenied = c => { if (c.Request.Path.StartsWithSegments("/api")) c.Response.StatusCode = 403; else c.Response.Redirect(c.RedirectUri); return Task.CompletedTask; }; });
b.Services.AddHttpContextAccessor(); b.Services.AddScoped<IAuditService, AuditService>();
b.Services.AddControllersWithViews(o => o.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));
var app = b.Build();
if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Home/Error"); app.UseHsts(); }
app.UseHttpsRedirection(); app.UseStaticFiles(); app.UseAuthentication(); app.UseAuthorization();
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
using (var s = app.Services.CreateScope()) { // seed roles + admin
  var sp = s.ServiceProvider; await sp.GetRequiredService<AppDbContext>().Database.MigrateAsync();
  var rm = sp.GetRequiredService<RoleManager<IdentityRole>>(); var um = sp.GetRequiredService<UserManager<AppUser>>();
  foreach (var r in new[]{Roles.Admin, Roles.Manager, Roles.Sales}) if (!await rm.RoleExistsAsync(r)) await rm.CreateAsync(new IdentityRole(r));
  var email = app.Configuration["Seed:AdminEmail"]!;
  if (await um.FindByEmailAsync(email) == null) { var u = new AppUser { UserName = email, Email = email, FullName = "Administrator", EmailConfirmed = true };
    if ((await um.CreateAsync(u, app.Configuration["Seed:AdminPassword"]!)).Succeeded) await um.AddToRoleAsync(u, Roles.Admin); } }
app.Run();
