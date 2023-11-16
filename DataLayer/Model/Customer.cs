using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataLayer.Model;

namespace TMS_Project.DataLayer.Model;

[Table("Customer")]
public class Customer
{
    [Key] public int CustomerId { get; set; }
    [MaxLength(100)] public string Name { get; set; } = "";
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }

    // Navigation property
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}