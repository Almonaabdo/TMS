using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;

namespace TMS_Project.DataLayer.Model
{
    [Table("Trip")]
    public class Trip
    {
        [Key] public int TripId { get; set; }
        public int OrderId { get; set; }
        public int CarrierId { get; set; }
        public TripStatus TripStatus { get; set; }

        public double TripCost { get; set; }

        // Navigation properties
        [ForeignKey("OrderId")] public Order Order { get; set; }
        public Carrier Carrier { get; set; }
    }

    public enum TripStatus
    {
        Scheduled,
        InProgress,
        Completed
    }
}