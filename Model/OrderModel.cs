using System;
using TMS_Project.DataLayer.Context;
using TMS_Project.DataLayer.Model;
using System.Linq;
// ReSharper disable UnusedType.Global

namespace TMS_Project.Model;

public class OrderModel
{
    const double FTLMARKUP = 0.08;
    const double LTLMARKUP = 0.05;
    private readonly TmsDbContext _db;
    private object _dbContext;

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


    /*
  * METHOD NAME: GetKmAndHrs
  * DESCRIPTION: Gets the total Km and hours needed for the carrier to reach the destination from the origin
  * 
  * RETURN: double[], [0] = totalKm, [1] = totalHrs
  */
    public double[] GetKmAndHrs(string destination, string origin)
    {
        double[] totalKmAndHrs = new double[2];

        var route = _db?.Routes?.FirstOrDefault(e =>
            e.SourceCity.CityName == origin && e.DestinationCity.CityName == destination);

        if (route != null)
        {
            totalKmAndHrs[0] = Math.Round(route.Distance, 3);
            totalKmAndHrs[1] = Math.Round(route.Duration, 3);
        }

        return totalKmAndHrs;
    }


    /*
    * METHOD NAME: CalculateRate
    * DESCRIPTION: Calculates the cost of the rates for carriers
    *
    * RETURN: double[] profit for TMS and carrier
    */
    public double[] CalculateRate(Carrier carrier, double totalKm, int vanType, int quantity, int job_type)
    {
       
        double ftlRate = carrier.FtlRate; 
        double ltlRate = carrier.LtlRate;
        double reeferCharge = carrier.ReefCharge;

        double[] totalAmount = new double[2];

        //ftl
        if (job_type == 0)
        {
            //reefer van
            if (vanType == 1)
            {
                ftlRate += (reeferCharge + FTLMARKUP) * ftlRate;
                double amount = ftlRate * totalKm;
                totalAmount[0] = amount * FTLMARKUP;
                                                     // Money gain for TMS
                totalAmount[1] = amount - totalAmount[0];  // Money gain for carrier
                return totalAmount;
            }
            else if (vanType == 0)
            {
                ftlRate += FTLMARKUP * ftlRate;
                double amount = ftlRate * totalKm;
                totalAmount[0] = amount * FTLMARKUP;             // Money gain for TMS
                totalAmount[1] = amount - totalAmount[0];  // Money gain for carrier
                return totalAmount;
            }
        }
        //ltl
        else if (job_type == 1)
        {
            //reefer van
            if (vanType == 1)
            {
                ltlRate += (LTLMARKUP + reeferCharge) * ltlRate;
                double amount = (ltlRate * totalKm) * quantity;

                totalAmount[0] = amount * LTLMARKUP;             // Money gain for TMS
                totalAmount[1] = amount - totalAmount[0];  // Money gain for carrier
                return totalAmount;

            }
            else if (vanType == 0)
            {
                ltlRate += LTLMARKUP * ltlRate;
                double amount = (ltlRate * totalKm) * quantity;
                totalAmount[0] = amount * LTLMARKUP;             // Money gain for TMS
                totalAmount[1] = amount - totalAmount[0];  // Money gain for carrier
                return totalAmount;
            }


        }
        return totalAmount;

    }


}