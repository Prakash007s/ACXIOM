using AcxiomCRM.Data; using AcxiomCRM.Models; using AcxiomCRM.Services; using AcxiomCRM.ViewModels; using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore; using System.Security.Claims;
namespace AcxiomCRM.Controllers.Api;
[ApiController, Route("api/customers"), Authorize, IgnoreAntiforgeryToken]
public class CustomersApiController(AppDbContext db, IAuditService audit) : ControllerBase {
  string Uid => User.FindFirstValue(ClaimTypes.NameIdentifier)!; bool All => User.IsInRole(Roles.Admin) || User.IsInRole(Roles.Manager);
  static CustomerDto Map(Customer c) => new() { Id = c.CustomerId, Code = c.CustomerCode, Name = c.CustomerName, Email = c.Email, Phone = c.Phone, Status = c.Status };
  [HttpGet] public async Task<ActionResult<IEnumerable<CustomerDto>>> Get() => (await db.Customers.Where(c => All || c.CreatedBy == Uid).Take(500).ToListAsync()).Select(Map).ToList();
  [HttpGet("{id:int}")] public async Task<ActionResult<CustomerDto>> Get(int id) { var c = await db.Customers.FirstOrDefaultAsync(x => x.CustomerId == id && (All || x.CreatedBy == Uid)); return c == null ? NotFound() : Map(c); }
  [HttpPost] public async Task<ActionResult<CustomerDto>> Post(CustomerVm m) {
    if (await db.Customers.AnyAsync(c => c.Email == m.Email || c.Phone == m.Phone)) return Conflict(new { error = "Duplicate email or phone." });
    var c = new Customer { CustomerCode = "C" + DateTime.UtcNow.Ticks % 100000000, CustomerName = m.CustomerName, Email = m.Email, Phone = m.Phone, CompanyName = m.CompanyName, Address = m.Address, City = m.City, State = m.State, CreatedBy = Uid };
    db.Customers.Add(c); await db.SaveChangesAsync(); await audit.LogAsync("Create", "Customer", c.CustomerId.ToString(), null, c.Email);
    return CreatedAtAction(nameof(Get), new { id = c.CustomerId }, Map(c)); }
  [HttpPut("{id:int}")] public async Task<IActionResult> Put(int id, CustomerVm m) {
    var c = await db.Customers.FirstOrDefaultAsync(x => x.CustomerId == id && (All || x.CreatedBy == Uid)); if (c == null) return NotFound();
    if (await db.Customers.AnyAsync(x => x.CustomerId != id && (x.Email == m.Email || x.Phone == m.Phone))) return Conflict(new { error = "Duplicate email or phone." });
    (c.CustomerName, c.Email, c.Phone, c.CompanyName, c.Address, c.City, c.State) = (m.CustomerName, m.Email, m.Phone, m.CompanyName, m.Address, m.City, m.State);
    await db.SaveChangesAsync(); await audit.LogAsync("Update", "Customer", id.ToString()); return NoContent(); }
  [HttpDelete("{id:int}"), Authorize(Roles = "Admin,Manager")] public async Task<IActionResult> Delete(int id) { var c = await db.Customers.FindAsync(id); if (c == null) return NotFound();
    c.Status = "Inactive"; await db.SaveChangesAsync(); await audit.LogAsync("Delete", "Customer", id.ToString()); return NoContent(); } }
