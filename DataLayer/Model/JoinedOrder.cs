using System;

namespace TMS_Project.DataLayer.Model
{
    public class JoinedOrder
    {
        public int OrderId { get; set; } // Match with Order table
        public int CustomerId { get; set; } // Match with Customer table
        public OrderStatus OrderStatus { get; set; } // Match with Order table
        public DateTime DateCompleted { get; set; } // Match with Order table
        public DateTime DateInitiated { get; set; } // Match with City table
        public string Origin { get; set; } // Match with City table
        public string Destination { get; set; } // Match with City table
        public double TripCost { get; set; } // Match with Trip table
    }
}