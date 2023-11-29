using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataLayer.Model;

namespace TMS_Project.DataLayer.Model;

[Table("Carrier")]
public class Carrier
{
    [Key] public int CarrierId { get; set; }
    public string CompanyName { get; set; } = "";
    //public int Capacity { get; set; }

    // Number of avaiable trucks for each.
    public int FTLA { get; set; }
    public int LTLA { get; set; }

    // charges for Full Truck Load/ Less Truck Load
    public double FtlRate { get; set; }
    public double LtlRate { get; set; }

    // percentage referring to extra cost if its freezer truck
    public double ReefCharge { get; set; }



    // Navigational properties
    public ICollection<Trip> Trips { get; set; } = new List<Trip>();
}