using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMS_Project.DataLayer.Model;

[Table("City")]
public class City
{
    [Key] public int CityId { get; set; }
    public string CityName { get; set; } = "";

    // Navigation properties
    public ICollection<Order> SourceOrders { get; set; } = new List<Order>();
    public ICollection<Order> DestinationOrders { get; set; } = new List<Order>();

    public virtual ICollection<Route> SourceRoutes { get; set; } = new List<Route>();
    public virtual ICollection<Route> DestinationRoutes { get; set; } = new List<Route>();
}