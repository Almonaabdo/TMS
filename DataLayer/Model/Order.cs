using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using TMS_Project.DataLayer.Context;

namespace TMS_Project.DataLayer.Model
{
    [Table("Orders")]
    public class Order
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateTime DateInitiated { get; set; }
        public DateTime? DateCompleted { get; set; }
        //public string? ClientName { get; set; }
       public JobType? JobType { get; set; }
       public VanType VanType { get; set; }
        public double Quantity { get; set; }

        // Foreign keys for source and destination
        public int SourceCityId { get; set; }
        public int DestinationCityId { get; set; }

        // Navigation properties
        public Customer? Customer { get; set; }
        public City? SourceCity { get; set; }
        public City? DestinationCity { get; set; }

        public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
        public Invoice? Invoices { get; set; }
    }

    public enum OrderStatus
    {
        Pending,
        InProgress,
        Completed
    }

    public enum VanType
    {
        DryVan = 0,
        Reefer = 1
    }

    public enum JobType
    {
        Ftl = 0,
        Ltl = 1
    }
}