using AcxiomCRM.Models; using AcxiomCRM.Services; using AcxiomCRM.ViewModels; using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Identity; using Microsoft.AspNetCore.Mvc;
namespace AcxiomCRM.Controllers;
public class AccountController(UserManager<AppUser> um, SignInManager<AppUser> sm, IAuditService audit) : Controller {
  [HttpGet] public IActionResult Login() => View(); [HttpGet] public IActionResult Register() => View(); public IActionResult Denied() => View();
  [HttpPost] public async Task<IActionResult> Login(LoginVm m) {
    if (!ModelState.IsValid) return View(m);
    var u = await um.FindByEmailAsync(m.Email);
    if (u is { IsActive: true }) { var r = await sm.PasswordSignInAsync(u, m.Password, m.RememberMe, lockoutOnFailure: true);
      if (r.Succeeded) { await audit.LogAsync("Login", "User", u.Id, userId: u.Id); return RedirectToAction("Index", "Home"); }
      await audit.LogAsync(r.IsLockedOut ? "Lockout" : "FailedLogin", "User", u.Id, userId: u.Id);
      if (r.IsLockedOut) { ModelState.AddModelError("", "Account locked. Try again later."); return View(m); } }
    else await audit.LogAsync("FailedLogin", "User", null, newV: m.Email);
    ModelState.AddModelError("", "Invalid login attempt."); return View(m); }
  [HttpPost] public async Task<IActionResult> Register(RegisterVm m) {
    if (!ModelState.IsValid) return View(m);
    var u = new AppUser { UserName = m.Email, Email = m.Email, FullName = m.FullName };
    var r = await um.CreateAsync(u, m.Password);
    if (!r.Succeeded) { foreach (var e in r.Errors) ModelState.AddModelError("", e.Description); return View(m); }
    await um.AddToRoleAsync(u, Roles.Sales); await audit.LogAsync("Register", "User", u.Id, userId: u.Id);
    await sm.SignInAsync(u, false); return RedirectToAction("Index", "Home"); }
  [Authorize, HttpPost] public async Task<IActionResult> Logout() { await audit.LogAsync("Logout", "User"); await sm.SignOutAsync(); return RedirectToAction(nameof(Login)); } }
