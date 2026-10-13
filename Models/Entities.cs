using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace AcxiomCRM.Models;
public static class Roles { public const string Admin="Admin", Manager="Manager", Sales="SalesExecutive"; }
public class AppUser : IdentityUser { [MaxLength(100)] public string FullName { get; set; } = ""; public bool IsActive { get; set; } = true; }
public class Customer {
  public int CustomerId { get; set; } [MaxLength(20)] public string CustomerCode { get; set; } = "";
  [MaxLength(100)] public string CustomerName { get; set; } = ""; [MaxLength(150)] public string Email { get; set; } = "";
  [MaxLength(15)] public string Phone { get; set; } = ""; [MaxLength(100)] public string? CompanyName { get; set; }
  [MaxLength(250)] public string? Address { get; set; } [MaxLength(60)] public string? City { get; set; } [MaxLength(60)] public string? State { get; set; }
  [MaxLength(20)] public string Status { get; set; } = "Active"; public DateTime CreatedDate { get; set; } = DateTime.UtcNow; public string? CreatedBy { get; set; } }
public class Lead {
  public int LeadId { get; set; } [MaxLength(20)] public string LeadCode { get; set; } = ""; [MaxLength(100)] public string LeadName { get; set; } = "";
  [MaxLength(150)] public string? Email { get; set; } [MaxLength(15)] public string? Phone { get; set; } [MaxLength(100)] public string? CompanyName { get; set; }
  [MaxLength(50)] public string? Source { get; set; } [MaxLength(20)] public string Status { get; set; } = "New"; public decimal ExpectedValue { get; set; }
  public DateTime CreatedDate { get; set; } = DateTime.UtcNow; public string? AssignedTo { get; set; }
  public static readonly string[] Statuses = { "New","Contacted","Qualified","Unqualified","Converted","Lost" }; }
public class Opportunity {
  public int OpportunityId { get; set; } [MaxLength(150)] public string OpportunityName { get; set; } = ""; public int CustomerId { get; set; } public Customer? Customer { get; set; }
  public int? LeadId { get; set; } public decimal Amount { get; set; } [MaxLength(20)] public string Stage { get; set; } = "Qualification";
  public int Probability { get; set; } public DateTime ExpectedCloseDate { get; set; } [MaxLength(20)] public string Status { get; set; } = "Open";
  public DateTime CreatedDate { get; set; } = DateTime.UtcNow; public string? AssignedTo { get; set; }
  public static readonly string[] Stages = { "Qualification","Proposal","Negotiation","Won","Lost" }; }
public class FollowUp { public int FollowUpId { get; set; } public int? CustomerId { get; set; } public int? LeadId { get; set; } public DateTime FollowUpDate { get; set; }
  [MaxLength(30)] public string FollowUpType { get; set; } = "Call"; [MaxLength(500)] public string? Remarks { get; set; } [MaxLength(20)] public string Status { get; set; } = "Planned"; public string? AssignedTo { get; set; } }
public class Activity { public int ActivityId { get; set; } [MaxLength(20)] public string ActivityType { get; set; } = "Call"; [MaxLength(150)] public string Subject { get; set; } = "";
  public string? Description { get; set; } public DateTime ActivityDate { get; set; } public int? CustomerId { get; set; } public int? LeadId { get; set; } public string? AssignedTo { get; set; } [MaxLength(20)] public string Status { get; set; } = "Open"; }
public class AuditLog { public int AuditLogId { get; set; } public string? UserId { get; set; } [MaxLength(50)] public string Action { get; set; } = ""; [MaxLength(50)] public string EntityName { get; set; } = "";
  public string? RecordId { get; set; } public string? OldValue { get; set; } public string? NewValue { get; set; } public DateTime CreatedDate { get; set; } = DateTime.UtcNow; [MaxLength(45)] public string? IpAddress { get; set; } }
