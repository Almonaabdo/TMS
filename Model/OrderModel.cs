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

    public Order CreateOrder(int contractId, int buyerId)
    {
        var newOrder = new Order
        {
            ContractId = contractId,
            BuyerId = buyerId,
            OrderStatus = OrderStatus.InProgress,
            DateInitiated = DateTime.Now,
            DateCompleted = null
        };

        _db.Orders?.Add(newOrder);
        _db.SaveChanges();
        return newOrder;
    }

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
}