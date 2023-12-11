using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMS_Project.DataLayer.Model;

[Table("Invoice")]
public sealed class Invoice
{
    [Key] public int InvoiceId { get; set; }
    [Required]
    public int OrderId { get; set; }
    [Required]
    public int RateId { get; set; }
    [Required]
    public int Quantity { get; set; }
    [Required]
    public double Amount { get; set; }
    [Required]
    public DateTime InvoiceDate { get; set; }

    // Foreign key for Customer
    public int CustomerId { get; set; }

    // Navigation properties
    public Order? Order { get; set; } // Assuming an Invoice is associated with one Order
    public Rate? Rates { get; set; }

    // Navigation property for Customer
    public Customer? Customer { get; set; }
}