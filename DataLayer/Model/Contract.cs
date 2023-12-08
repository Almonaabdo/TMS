using System.ComponentModel.DataAnnotations.Schema;

namespace TMS_Project.DataLayer.Model;

[Table("Contract")] // Specify the table name if it's different from the class name
public class Contract
{
    public string? Client_Name { get; set; }

    public int Job_Type { get; set; }

    public int Quantity { get; set; }

    public string? Origin { get; set; }

    public string? Destination { get; set; }

    public int Van_Type { get; set; }
}