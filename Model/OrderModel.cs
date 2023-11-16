using System;
using TMS_Project.DataLayer.Context;
using TMS_Project.DataLayer.Model;
// ReSharper disable UnusedType.Global

namespace TMS_Project.Model;

public class OrderModel
{
    private readonly TmsDbContext _db;
    public OrderModel(TmsDbContext context)
    { 
        _db = context;
    }

    /// <summary>
    /// Method to create an order and update the database
    /// </summary>
    /// <param name="order"></param>
    public void CreateOrder(Order order)
    {
        _db.Orders?.Add(order);
        _db.SaveChanges();
    }

    /// <summary>
    /// Method to delete a give order based on the id
    /// </summary>
    /// <param name="orderId">The order to delete specified by the id</param>
    public void DeleteOrder (int orderId)
    {
        // searching for the entered order
        var order = _db.Orders?.Find(orderId);

        if (order != null)
        {
            _db.Orders?.Remove(order);
            _db.SaveChanges();
        }
        else
        {
            LoggerModel.LogError("Specified Order Wasn't Found In Database.");
        }
    }

    /// <summary>
    /// Method to update the status of a trip
    /// </summary>
    /// <param name="tripId">Id to identify each trip</param>
    /// <param name="newStatus">The new status to be given</param>
    public void UpdateTripStatus(int tripId, TripStatus newStatus)
    {
        var trip = _db.Trips.Find(tripId);
        if (trip != null)
        {
            trip.TripStatus = newStatus;
            _db.SaveChanges();
        }
    }
}