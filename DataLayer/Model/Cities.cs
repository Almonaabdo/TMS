using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMS_Project.DataLayer.Model;

[Table("Cities")]
public class Cities
{
    [Key] public int CityId { get; set; }
    public string CityName { get; set; } = "";

    // Navigation properties
    public virtual ICollection<Order> SourceOrders { get; set; } = new List<Order>();
    public virtual ICollection<Order> DestinationOrders { get; set; } = new List<Order>();
}
