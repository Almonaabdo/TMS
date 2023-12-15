using System;
using System.Linq;
using System.Windows;
using TMS_Project.DataLayer.Context;
using TMS_Project.DataLayer.Model;

namespace TMS_Project.Model;

public class PlannerModel
{
    private readonly TmsDbContext _db;
    private readonly LoggerModel _loggerModel = LoggerModel.Instance;

    public PlannerModel()
    {
        _db = new TmsDbContext();
    }


    /*
    * METHOD NAME: GetCarrier
    * DESCRIPTION: Gets carrier information from the DB
    * 
    * RETURN: carrier if found, null otherwise
    */
    public Carrier? GetCarrier(string companyName, string originCityy)
    {
        try
        {
            var carrier = _db.Carriers?.FirstOrDefault(e => e.CompanyName == companyName && e.DepotCity == originCityy);
            if (carrier != null)
            {
                _loggerModel.LogInfo($" Carrier {carrier.CompanyName} found");
                return carrier;

            }
            return null;
        }

        catch
        {
            _loggerModel.LogError("Finding carrier failed");
            MessageBox.Show("Finding carrier failed");
            return null;
        }
        
    }


    /*
    * METHOD NAME: AddTripToOrder
    * DESCRIPTION: Adds trip to an existing order to be completed
    * 
    * RETURN: void
    */
    public void AddTripToOrder(int orderId, Trip newTrip)
    {
        // find specified order.
        var order = _db.Orders?.Find(orderId);

        if (order != null)
        {
            try
            {
                //Change order status and add the new trip to order
                order.OrderStatus = OrderStatus.InProgress;
                order.Trips.Add(newTrip);
                //Save changes from database
                _db.SaveChanges();
                _loggerModel.LogInfo("Succesfully attached a trip to the order");
            }

            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        else
        {
            MessageBox.Show("order null");
            _loggerModel.LogError("Info: Can't Add Trip to Order! Specified Order Wasn't Found In Database.");
        }
    }

}