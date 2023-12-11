using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMS_Project.DataLayer.Model;

[Table("Rate")]
public class Rate
{
    [Key] public int RateId { get; set; }
    public RateType RateType { get; set; }
    public double Amount { get; set; }

    // Navigation properties
    public virtual ICollection<Invoice>? Invoices { get; set; }
}

public enum RateType
{
    FTL = 0,
    LTL = 1
}