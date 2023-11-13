using DataLayer.Model;
using Model;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DataLayer.Model;

[Table("User")]
public class User
{
    [Key]
    public int UserId { get; set; }
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public UserType UserType { get; set; }

    public virtual ICollection<Order> Orders { get; set; }
    public virtual ICollection<LogFile>? LogFiles { get; set; }

}

public enum UserType
{
    Admin,
    Buyer,
    Planner
}