using AcxiomCRM.Data; using AcxiomCRM.Models; using AcxiomCRM.Services; using AcxiomCRM.ViewModels; using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore; using System.Security.Claims;
namespace AcxiomCRM.Controllers;
[Authorize] public class OpportunitiesController(AppDbContext db, IAuditService audit) : Controller {
  string Uid => User.FindFirstValue(ClaimTypes.NameIdentifier)!; bool All => User.IsInRole(Roles.Admin) || User.IsInRole(Roles.Manager);
  public async Task<IActionResult> Index(string? stage) { var q = db.Opportunities.Include(o => o.Customer).Where(o => All || o.AssignedTo == Uid);
    if (!string.IsNullOrEmpty(stage)) q = q.Where(o => o.Stage == stage); return View(await q.OrderByDescending(o => o.CreatedDate).Take(200).ToListAsync()); }
  public async Task<IActionResult> Create() { await Load(); return View(new OpportunityVm()); }
  [HttpPost] public async Task<IActionResult> Create(OpportunityVm m) {
    if (!await db.Customers.AnyAsync(c => c.CustomerId == m.CustomerId && (All || c.CreatedBy == Uid))) ModelState.AddModelError(nameof(m.CustomerId), "Select a valid customer.");
    if (!ModelState.IsValid) { await Load(); return View(m); }
    var o = new Opportunity { OpportunityName = m.OpportunityName, CustomerId = m.CustomerId, Amount = m.Amount, Probability = m.Probability, Stage = m.Stage,
      ExpectedCloseDate = m.ExpectedCloseDate, Status = m.Stage == "Won" || m.Stage == "Lost" ? m.Stage : "Open", AssignedTo = Uid };
    db.Opportunities.Add(o); await db.SaveChangesAsync(); await audit.LogAsync("Create", "Opportunity", o.OpportunityId.ToString(), null, $"{o.OpportunityName}|{o.Amount}|{o.Stage}");
    return RedirectToAction(nameof(Index)); }
  async Task Load() { ViewBag.Customers = await db.Customers.Where(c => All || c.CreatedBy == Uid).OrderBy(c => c.CustomerName).ToListAsync(); } }
