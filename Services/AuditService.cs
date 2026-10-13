using AcxiomCRM.Data; using AcxiomCRM.Models; using System.Security.Claims;
namespace AcxiomCRM.Services;
public interface IAuditService { Task LogAsync(string action, string entity, string? recordId = null, string? oldV = null, string? newV = null, string? userId = null); }
public class AuditService(AppDbContext db, IHttpContextAccessor http) : IAuditService {
  public async Task LogAsync(string action, string entity, string? recordId = null, string? oldV = null, string? newV = null, string? userId = null) {
    var ctx = http.HttpContext; // never pass passwords/tokens in oldV/newV
    db.AuditLogs.Add(new AuditLog { Action = action, EntityName = entity, RecordId = recordId, OldValue = oldV, NewValue = newV,
      UserId = userId ?? ctx?.User.FindFirstValue(ClaimTypes.NameIdentifier), IpAddress = ctx?.Connection.RemoteIpAddress?.ToString() });
    await db.SaveChangesAsync(); } }
