using Model;
using System.Runtime.CompilerServices;
using System;
using DataLayer.Context;

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

    public Order deleteOrder (int contractID, int buyerID)
    {
        var newOrder = new Order
        {
            ContractId = contractID,
            BuyerId = buyerID
        };

        _db.Orders.Remove(newOrder);

        return newOrder;
    }
}