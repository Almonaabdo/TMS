using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TMS_Project.DataLayer.Model;

[Table("Invoice")]
public class Invoice
{
    [Key] public int InvoiceId { get; set; }
    public int OrderId { get; set; }
    public int RateId { get; set; }
    public int Quantity { get; set; }
    public decimal Amount { get; set; }
    public DateTime InvoiceDate { get; set; }

    // Foreign key for Customer
    public int CustomerId { get; set; }

    // Navigation properties
    public virtual Order Order { get; set; } // Assuming an Invoice is associated with one Order
    public virtual Rates Rates { get; set; }

    // Navigation property for Customer
    public virtual Customer Customer { get; set; }
}