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



    /*
    * METHOD NAME: CreateOrder
    * DESCRIPTION: Creates new order by calling the database Add method and saves changes
    * 
    * RETURN: void
    */
    public void CreateOrder(Order order)
    {
        try 
        { 
            _db.Orders?.Add(order);
            _db.SaveChanges();
        }
        catch (Exception ex)
        {
            LoggerModel.LogError($"Order creating erro: {ex.Message}");
        }
    }



    /*
    * METHOD NAME: DeleteOrder
    * DESCRIPTION: Deletes an order from DB if it's found
    *
    * RETURN: void
    */
    public void DeleteOrder (int orderId)
    {
        // searching for the entered order
        var order = _db.Orders?.Find(orderId);

        if (order != null)
        {
            // remove order and save changes
            _db.Orders?.Remove(order);
            _db.SaveChanges();
        }
        else
        {
            LoggerModel.LogError("Info: Specified Order Wasn't Found In Database.");
        }
    }



    /*
    * METHOD NAME: CompleteOrder
    * DESCRIPTION: Changes status of specified order, and completed order data as todays date.
    *
    * RETURN: void
    */
    public void CompleteOrder(int orderId)
    {
        // searching for the entered order
        var order = _db.Orders?.Find(orderId);

        if (order != null)
        {
            // updating the status of the found order by changing status and dateCompleted.
            UpdateTripStatus(orderId, TripStatus.Completed);

            // changing dataCopleted to the current date of today.
            order.DateCompleted = DateTime.Now;

            _db.SaveChanges();
        }
        else
        {
            LoggerModel.LogError("Info: Can't Complete order! Specified Order Wasn't Found In Database.");
        }
    }



    /*
    * METHOD NAME: UpdateTripStatus
    * DESCRIPTION: Deletes an order from DB if it's found
    * 
    * RETURN: void
    */
    public void UpdateTripStatus(int tripId, TripStatus newStatus)
    {
        // find specified trip.
        var trip = _db.Trips?.Find(tripId);

        if (trip != null)
        {
            // check if new status matches old status
            if (trip.TripStatus != newStatus)
            { 
                // update trip if it's found and doesn't match
                trip.TripStatus = newStatus;
                _db.SaveChanges();
            }
            else
            {
                LoggerModel.LogError("Info: Trip Status Wasn't change as new status remains the same");
            }
        }
        else
        {
            LoggerModel.LogError("Info: Can't Change Trip Status! Specified trip Wasn't Found In Database.");
        }
    }
}