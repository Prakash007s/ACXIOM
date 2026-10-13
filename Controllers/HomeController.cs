using AcxiomCRM.Data; using AcxiomCRM.Models; using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore; using System.Security.Claims;
namespace AcxiomCRM.Controllers;
[Authorize] public class HomeController(AppDbContext db) : Controller {
  public async Task<IActionResult> Index() {
    var uid = User.FindFirstValue(ClaimTypes.NameIdentifier); bool all = User.IsInRole(Roles.Admin) || User.IsInRole(Roles.Manager); // server-side scoping
    var cu = db.Customers.Where(c => all || c.CreatedBy == uid); var le = db.Leads.Where(l => all || l.AssignedTo == uid); var op = db.Opportunities.Where(o => all || o.AssignedTo == uid);
    var ld = await le.GroupBy(l => l.Status).Select(g => new { g.Key, N = g.Count() }).ToDictionaryAsync(x => x.Key, x => x.N);
    var pl = await op.GroupBy(o => o.Stage).Select(g => new { g.Key, N = g.Count() }).ToDictionaryAsync(x => x.Key, x => x.N);
    ViewBag.Cards = new Dictionary<string, object> {
      ["Total Customers"] = await cu.CountAsync(), ["Total Leads"] = await le.CountAsync(),
      ["Open Leads"] = await le.CountAsync(l => l.Status != "Lost" && l.Status != "Converted" && l.Status != "Unqualified"),
      ["Total Opportunities"] = await op.CountAsync(), ["Open Opportunities"] = await op.CountAsync(o => o.Status == "Open"),
      ["Won"] = await op.CountAsync(o => o.Stage == "Won"), ["Lost"] = await op.CountAsync(o => o.Stage == "Lost"),
      ["Pipeline Value"] = (await op.Where(o => o.Status == "Open").SumAsync(o => (decimal?)o.Amount) ?? 0).ToString("N2") };
    ViewBag.LeadLabels = Lead.Statuses; ViewBag.LeadData = Lead.Statuses.Select(s => ld.GetValueOrDefault(s));
    ViewBag.PipeLabels = Opportunity.Stages; ViewBag.PipeData = Opportunity.Stages.Select(s => pl.GetValueOrDefault(s));
    return View(); }
  [AllowAnonymous] public IActionResult Error() => Content("An error occurred."); }
