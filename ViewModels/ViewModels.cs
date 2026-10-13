using AcxiomCRM.Models; using System.ComponentModel.DataAnnotations;
namespace AcxiomCRM.ViewModels;
public class LoginVm { [Required, EmailAddress] public string Email { get; set; } = ""; [Required, DataType(DataType.Password)] public string Password { get; set; } = ""; public bool RememberMe { get; set; } }
public class RegisterVm { [Required, StringLength(100)] public string FullName { get; set; } = ""; [Required, EmailAddress] public string Email { get; set; } = "";
  [Required, DataType(DataType.Password)] public string Password { get; set; } = ""; [Compare(nameof(Password))] [DataType(DataType.Password)] public string ConfirmPassword { get; set; } = ""; }
public class CustomerVm { public int CustomerId { get; set; }
  [Required(ErrorMessage="Customer Name is required."), StringLength(100)] public string CustomerName { get; set; } = "";
  [Required, EmailAddress(ErrorMessage="Enter a valid email address."), StringLength(150)] public string Email { get; set; } = "";
  [Required, RegularExpression(@"^[6-9]\d{9}$", ErrorMessage="Enter a valid phone number.")] public string Phone { get; set; } = "";
  [StringLength(100)] public string? CompanyName { get; set; } [StringLength(250)] public string? Address { get; set; } [StringLength(60)] public string? City { get; set; } [StringLength(60)] public string? State { get; set; } }
public class OpportunityVm : IValidatableObject { public int OpportunityId { get; set; }
  [Required, StringLength(150)] public string OpportunityName { get; set; } = ""; [Required] public int CustomerId { get; set; }
  [Range(typeof(decimal),"0.01","999999999",ErrorMessage="Opportunity Amount must be greater than 0.")] public decimal Amount { get; set; }
  [Range(0,100,ErrorMessage="Probability must be between 0 and 100.")] public int Probability { get; set; }
  [Required] public string Stage { get; set; } = "Qualification";
  [DataType(DataType.Date)] public DateTime ExpectedCloseDate { get; set; } = DateTime.Today;
  public IEnumerable<ValidationResult> Validate(ValidationContext c) {
    bool active = Stage != "Won" && Stage != "Lost";
    if (!Opportunity.Stages.Contains(Stage)) yield return new("Invalid stage.", new[]{nameof(Stage)});
    if (active && ExpectedCloseDate.Date < DateTime.Today) yield return new("Expected Close Date cannot be in the past.", new[]{nameof(ExpectedCloseDate)}); } }
public class CustomerDto { public int Id { get; set; } public string Code { get; set; } = ""; public string Name { get; set; } = ""; public string Email { get; set; } = ""; public string Phone { get; set; } = ""; public string Status { get; set; } = ""; }
