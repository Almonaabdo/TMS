using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TMS_Project.DataLayer.Model;

namespace TMS_Project.DataLayer.Model;

[Table("LogFile")]
public class LogFile
{
    [Key] public int LogId { get; set; }
    public int UserId { get; set; }
    public string LogDetails { get; set; } = "";
    public DateTime LogTimeStamp { get; set; }

    // Navigation properties
    public virtual User? Users { get; set; }
}