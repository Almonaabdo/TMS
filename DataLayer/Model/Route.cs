using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Model;

[Table("Route")]
public class Route
{
    [Key]
    public int RouteId { get; set; }
    public int SourceCityId { get; set; }
    public int DestinationCityId { get; set; }
    public decimal Distance { get; set; }
    public TimeSpan Duration { get; set; }

    // Navigation properties
    public virtual Cities? SourceCity { get; set; }
    public virtual Cities? DestinationCity { get; set; }
}