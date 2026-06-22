using LoadManagementSystem_LMS_.Models;
using System.ComponentModel.DataAnnotations;

public class Customer
{
    public int Id { get; set; }

    public string CustomerCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [RegularExpression(@"^\+1[0-9]{10}$",
    ErrorMessage = "Phone Number must be in format +1XXXXXXXXXX")]
    public string PhoneNumber { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public ICollection<Load> Loads { get; set; } = new List<Load>();
}