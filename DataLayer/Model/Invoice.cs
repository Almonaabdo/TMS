using System.ComponentModel.DataAnnotations;

namespace DataLayer.Model;

using global::Model;
using System;
using System.ComponentModel.DataAnnotations.Schema;
[Table("Invoice")]
public class Invoice
{
    [Key]
    public int InvoiceId { get; set; }
    public int OrderId { get; set; }
    public int RateId { get; set; }
    public int Quantity { get; set; }
    public decimal Amount { get; set; }
    public DateTime InvoiceDate { get; set; }

    // Navigation properties
    public virtual Order Order { get; set; }
    public virtual Rates Rates { get; set; }
}