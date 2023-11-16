using Model;
using System.Runtime.CompilerServices;
using System;
using DataLayer.Context;
using NLog;

namespace TMS_Project.Model;

public class OrderModel
{
    private readonly TmsDbContext _db;

    public OrderModel(TmsDbContext context)
    { 
        _db = context;
    }


    public Order createOrder(int contractID, int buyerID)
    {
        var newOrder = new Order
        {
            ContractId = contractID,
            BuyerId = buyerID,
            OrderStatus = OrderStatus.InProgress,
            DateInitiated = DateTime.Now,
            DateCompleted = null
        };

        _db.Orders.Add(newOrder);
        _db.SaveChanges();
        return newOrder;
    }

    public void deleteOrder (int OrderID)
    {
        // searching for the ented order
        var Order = _db.Orders.Find(OrderID);

        if (Order != null)
        {
            _db.Orders.Remove(Order);
            _db.SaveChanges();
        }
        else
        {
            LoggerModel.LogError("Specified Order Wasn't Found In Database.");
        }
    }

    public void updateCity(int OrderID, string newCity, int newCityID)
    {
        var Order = _db.Orders.Find(OrderID);

        if (Order != null)
        {
            Order.DestinationCity.CityName.Equals(newCity);
            Order.DestinationCity.CityId.Equals(newCityID);
            _db.SaveChanges();
        }
        else
        {
            LoggerModel.LogError("Specified Order city Couldn't be updated");
        }
    }
}