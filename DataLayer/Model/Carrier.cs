using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Emit;

namespace TMS_Project.DataLayer.Model;

[Table("Carrier")]
public class Carrier
{
    [Key] public int CarrierId { get; set; }
    public string? CompanyName { get; set; } = "";
    public string? DepotCity { get; set; }
    public int Ftla { get; set; }
    public int Ltla { get; set; }

    // charges for Full Truck Load/ Less Truck Load
    public double FtlRate { get; set; }
    public double LtlRate { get; set; }

    // percentage referring to extra cost if its freezer truck
    public double ReefCharge { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();

    // Navigational properties
    public ICollection<Trip> Trips { get; set; } = new List<Trip>();
}