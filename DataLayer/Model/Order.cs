using DataLayer.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model;

[Table("Order")]
public class Order
{
    [Key] public int OrderId { get; set; }
    public int ContractId { get; set; }
    public int BuyerId { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public DateTime DateInitiated { get; set; }
    public DateTime DateCompleted { get; set; }

    // Foreign keys for source and destination
    public int SourceCityId { get; set; }
    public int DestinationCityId { get; set; }

    // Navigation properties
    public virtual User? Buyer { get; set; }
    public virtual ICollection<Trip>? Trips { get; set; }
    public virtual ICollection<Invoice>? Invoices { get; set; }

    public virtual Cities SourceCity { get; set; }
    public virtual Cities DestinationCity { get; set; }

}

public enum OrderStatus
{
    Pending,
    InProgress,
    Completed
}