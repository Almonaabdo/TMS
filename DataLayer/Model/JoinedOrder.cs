using System;

namespace TMS_Project.DataLayer.Model;

public class JoinedOrder
{
    public int OrderId; // Match with Order table
    public int CustomerId; // Match with Customer table
    public OrderStatus OrderStatus; // Match with Order table
    public DateTime DateCompleted; // Match with Order table
    public DateTime DateInititated; // Match with City table
    public string Origin; // Match with City table
    public string Destination; // Match with City table
    public double TripCost;   // Match with Trip table




}