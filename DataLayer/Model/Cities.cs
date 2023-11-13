using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Model;

[Table("Cities")]
public class Cities
{
    [Key]
    public int CityId { get; set; }
    public string CityName { get; set; } = "";

    // Navigation properties
   // public virtual ICollection<Order>? SourceOrders { get; set; }
    //public virtual ICollection<Order>? DestinationOrders { get; set; }
}