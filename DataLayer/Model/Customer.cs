namespace DataLayer.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Customer")]
public class Customer
{
    [Key]
    public int CustomerId { get; set; }
    [MaxLength(100)] public string Name { get; set; } = "";
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
}