using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMS_Project.DataLayer.Model;

[Table("Invoice")]
public sealed class Invoice
{
    [Key] public int InvoiceId { get; set; }

    public int OrderId { get; set; }
    public double Amount { get; set; }
    public string CustomerName { get; set; }
    public string Origin { get; set; }
    public string Destination { get; set; }
    public double Quantity { get; set; }
    public JobType? JobType { get; set; }
    public VanType VanType { get; set; }
    public DateTime DateCompleted { get; set; }

    public DateTime InvoiceDate { get; set; }

    // Foreign key for Customer
    public int CustomerId { get; set; }

    // Navigation properties
    public Order? Order { get; set; } // Assuming an Invoice is associated with one Order

    // Navigation property for Customer
    public Customer? Customer { get; set; }
}