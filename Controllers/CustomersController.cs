using AcxiomCRM.Data; using AcxiomCRM.Models; using AcxiomCRM.Services; using AcxiomCRM.ViewModels; using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore; using System.Security.Claims;
namespace AcxiomCRM.Controllers;
[Authorize] public class CustomersController(AppDbContext db, IAuditService audit) : Controller {
  string Uid => User.FindFirstValue(ClaimTypes.NameIdentifier)!; bool All => User.IsInRole(Roles.Admin) || User.IsInRole(Roles.Manager);
  IQueryable<Customer> Scope => db.Customers.Where(c => All || c.CreatedBy == Uid);
  public async Task<IActionResult> Index(string? q) { var qs = Scope; if (!string.IsNullOrWhiteSpace(q)) qs = qs.Where(c => c.CustomerName.Contains(q) || c.Email.Contains(q) || c.Phone.Contains(q) || (c.CompanyName ?? "").Contains(q));
    ViewBag.Q = q; return View(await qs.OrderByDescending(c => c.CreatedDate).Take(200).ToListAsync()); }
  public IActionResult Create() => View("Form", new CustomerVm());
  public async Task<IActionResult> Edit(int id) { var c = await Scope.FirstOrDefaultAsync(x => x.CustomerId == id); if (c == null) return NotFound();
    return View("Form", new CustomerVm { CustomerId = c.CustomerId, CustomerName = c.CustomerName, Email = c.Email, Phone = c.Phone, CompanyName = c.CompanyName, Address = c.Address, City = c.City, State = c.State }); }
  [HttpPost] public async Task<IActionResult> Save(CustomerVm m) {
    if (await db.Customers.AnyAsync(c => c.CustomerId != m.CustomerId && c.Email == m.Email)) ModelState.AddModelError(nameof(m.Email), "Email already exists.");
    if (await db.Customers.AnyAsync(c => c.CustomerId != m.CustomerId && c.Phone == m.Phone)) ModelState.AddModelError(nameof(m.Phone), "Phone already exists.");
    if (!ModelState.IsValid) return View("Form", m);
    Customer? c; string? old = null;
    if (m.CustomerId == 0) { c = new Customer { CustomerCode = "C" + DateTime.UtcNow.Ticks % 100000000, CreatedBy = Uid }; db.Customers.Add(c); }
    else { c = await Scope.FirstOrDefaultAsync(x => x.CustomerId == m.CustomerId); if (c == null) return Forbid(); old = $"{c.CustomerName}|{c.Email}|{c.Phone}"; }
    (c.CustomerName, c.Email, c.Phone, c.CompanyName, c.Address, c.City, c.State) = (m.CustomerName, m.Email, m.Phone, m.CompanyName, m.Address, m.City, m.State);
    await db.SaveChangesAsync(); await audit.LogAsync(m.CustomerId == 0 ? "Create" : "Update", "Customer", c.CustomerId.ToString(), old, $"{c.CustomerName}|{c.Email}|{c.Phone}");
    return RedirectToAction(nameof(Index)); }
  [HttpPost, Authorize(Roles = "Admin,Manager")] public async Task<IActionResult> Delete(int id) { var c = await db.Customers.FindAsync(id); if (c == null) return NotFound();
    c.Status = "Inactive"; await db.SaveChangesAsync(); await audit.LogAsync("Delete", "Customer", id.ToString(), "Active", "Inactive"); return RedirectToAction(nameof(Index)); } }
