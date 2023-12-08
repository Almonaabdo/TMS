using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TMS_Project.DataLayer.Model;

namespace TMS_Project.DataLayer.Model;

[Table("Customer")]
public sealed class Customer
{
    [Key] public int CustomerId { get; set; }
    
    public int UserId { get; set; }
    [MaxLength(100)] public string Name { get; set; } = "";
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }

    // Navigation property
    public User? User { get; set; }
    public List<Order>? Orders { get; set; }
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}