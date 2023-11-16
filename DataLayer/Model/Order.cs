using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DataLayer.Model;

namespace TMS_Project.DataLayer.Model;

[Table("Orders")]
public class Order
{
    public int OrderId { get; set; }
    public int ContractId { get; set; }
    public int BuyerId { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public DateTime DateInitiated { get; set; }
    public DateTime? DateCompleted { get; set; }

    // Foreign keys for source and destination
    public int SourceCityId { get; set; }
    public int DestinationCityId { get; set; }

    // Navigation properties
    public User? Buyer { get; set; }

    //public ICollection<Trip>? Trip { get; set; }
    public ICollection<Invoice>? Invoices { get; set; }

    public Cities SourceCity { get; set; }
    public Cities DestinationCity { get; set; }
}

public enum OrderStatus
{
    Pending,
    InProgress,
    Completed
}