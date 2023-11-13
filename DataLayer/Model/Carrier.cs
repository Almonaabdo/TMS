using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Model;

[Table("Carrier")]
public class Carrier
{
    [Key]
    public int CarrierId { get; set; }
    public string CompanyName { get; set; } = "";
    public int Capacity { get; set; }

    // Navigational properties
    public virtual ICollection<Trip> Trips { get; set; }

}