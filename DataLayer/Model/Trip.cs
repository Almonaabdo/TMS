using Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Model;

[Table("Trip")]
public class Trip
{
    [Key]
    public int TripId { get; set; }
    public int OrderId { get; set; }
    public int CarrierId { get; set; }
    public TripStatus TripStatus { get; set; }

    // Navigation properties
    public virtual Order? Order { get; set; }
    public virtual Carrier? Carrier { get; set; }
}

public enum TripStatus
{
    Scheduled,
    InProgress,
    Completed
}