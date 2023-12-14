using System;
using System.Linq;
using System.Windows;
using TMS_Project.DataLayer.Context;
using TMS_Project.DataLayer.Model;

namespace TMS_Project.Model;

public class PlannerModel
{
    private readonly TmsDbContext _db;
    public PlannerModel()
    {
        _db = new TmsDbContext();
    }

    public Carrier? GetCarrier(string companyName, string originCityy)
    {
        var carrier = _db.Carriers?.FirstOrDefault(e => e.CompanyName == companyName && e.DepotCity == originCityy);
        if (carrier != null)
        {
            return carrier;
        }
        return null;
    }

    public void AddTripToOrder(int orderId, Trip newTrip)
    {
        // find specified order.
        var order = _db.Orders?.Find(orderId);

        if (order != null)
        {
            try
            {

                order.OrderStatus = OrderStatus.InProgress;
                order.Trips.Add(newTrip);
                
                _db.SaveChanges();
                LoggerModel.LogInfo("Succesfully attached a trip to the order");
            }

            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        else
        {
            MessageBox.Show("order null");
            LoggerModel.LogError("Info: Can't Add Trip to Order! Specified Order Wasn't Found In Database.");
        }
    }

}