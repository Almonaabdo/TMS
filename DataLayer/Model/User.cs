using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMS_Project.DataLayer.Model;

[Table("User")]
public class User
{
    [Key] public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? Password { get; set; } = string.Empty;
    public UserType UserType { get; set; }
}

public enum UserType
{
    Admin = 0,
    Buyer = 1,
    Planner = 2
}