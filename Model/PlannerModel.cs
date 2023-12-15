using System;
using System.Linq;
using System.Windows;
using TMS_Project.DataLayer.Context;
using TMS_Project.DataLayer.Model;

namespace TMS_Project.Model;

public class PlannerModel
{


    /*
    * METHOD NAME: GetCarrier
    * DESCRIPTION: Gets carrier information from the DB
    * 
    * RETURN: carrier if found, null otherwise
    */
    public Carrier? GetCarrier(string? companyName, string originCityy)
    {
        try
        {
            var carrier = DbContextSingleton.Instance.Carriers?.FirstOrDefault(e => e.CompanyName == companyName && e.DepotCity == originCityy);
            if (carrier != null)
            {
                LoggerModel.Instance.LogInfo($" Carrier {carrier.CompanyName} found");
                return carrier;

            }
            return null;
        }

        catch
        {
            LoggerModel.Instance.LogError("Finding carrier failed");
            MessageBox.Show("Finding carrier failed");
            return null;
        }
        
    }


    /*
    * METHOD NAME: GetCarrier
    * DESCRIPTION: Gets carrier information from the DB
    * 
    * RETURN: carrier if found, null otherwise
    */
    public Carrier? GetCarrierUsingOriginCity(string originCityy)
    {
        try
        {
            var carrier = DbContextSingleton.Instance.Carriers?.FirstOrDefault(e => e.DepotCity == originCityy);
            if (carrier != null)
            {
                LoggerModel.Instance.LogInfo($" Carrier {carrier.CompanyName} found");
                return carrier;

            }
            return null;
        }

        catch
        {
            LoggerModel.Instance.LogError("Finding carrier failed");
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
    public void AddTripToOrder(int orderId,Order? order, Carrier carrier, double cost)
    {
        // find specified order.
         order = DbContextSingleton.Instance.Orders?.Find(orderId);

        if (order != null)
        {
            try
            {
                 Trip trip = new Trip
                 {
                     OrderId = orderId,
                     Order = order,
                     Carrier = carrier,
                     CarrierId = carrier.CarrierId,
                     TripCost = cost
                 };

                //Change order status and add the new trip to order
                order.OrderStatus = OrderStatus.InProgress;
                order.Trips.Add(trip);
                //Save changes from database
                DbContextSingleton.Instance.SaveChanges();
                LoggerModel.Instance.LogInfo("Successfully attached a trip to the order");
            }

            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        else
        {
            MessageBox.Show("order null");
            LoggerModel.Instance.LogError("Info: Can't Add Trip to Order! Specified Order Wasn't Found In Database.");
        }
    }

}