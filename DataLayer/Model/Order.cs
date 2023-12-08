using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        // Foreign keys for source and destination
        public int SourceCityId { get; set; }
        public int DestinationCityId { get; set; }

        // Navigation properties
        public Customer Customer { get; set; }
        public City SourceCity { get; set; }
        public City DestinationCity { get; set; }

        public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
        public Invoice Invoices { get; set; }
    }

    public enum OrderStatus
    {
        Pending,
        InProgress,
        Completed
    }
}