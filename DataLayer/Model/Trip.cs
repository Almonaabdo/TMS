using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMS_Project.DataLayer.Model;

[Table("Trip")]
public class Trip
{
    [Key] public int TripId { get; set; }
    public int OrderId { get; set; }
    public int CarrierId { get; set; }
    public TripStatus TripStatus { get; set; }

    // Navigation properties
    // Relationship: Each trip belongs to one order, each order can have multiple trips
    // public virtual Order? Order { get; set; }
    // Relationship: Each trip has one carrier
    public Carrier? Carrier { get; set; }
}

public enum TripStatus
{
    Scheduled,
    InProgress,
    Completed
}