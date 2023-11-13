using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Model;
[Table("Rates")]
public class Rates
{
    [Key]
    public int RateId { get; set; }
    public RateType RateType { get; set; }
    public decimal Amount { get; set; }

    // Navigation properties
    public virtual ICollection<Invoice>? Invoices { get; set; }
}

public enum RateType
{
    Standard,
    Premium
}