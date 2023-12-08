using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMS_Project.DataLayer.Model;

[Table("Route")]
public class Route
{
    [Key] public int RouteId { get; set; }
    public int SourceCityId { get; set; }
    public int DestinationCityId { get; set; }
    public decimal Distance { get; set; }
    public decimal Duration { get; set; }

    // Navigation properties
    public virtual City SourceCity { get; set; } = new();
    public virtual City DestinationCity { get; set; } = new();
}