namespace TMS_Project.DataLayer.Model;

public class JoinedRouteData
{
    public int RouteId { get; set; }
    public string? Origin { get; set; } 
    public string? Destination { get; set; }
    public double Distance { get; set; }
    public double Duration { get; set; }
    
}