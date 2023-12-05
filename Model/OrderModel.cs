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


    /*
  * METHOD NAME: GetKmAndHrs
  * DESCRIPTION: Gets the total Km and hours needed for the carrier to reach the destination from the origin
  * 
  * RETURN: double[], [0] = totalKm, [1] = totalHrs
  */
    public double[] GetKmAndHrs(string destination, string origin)
    {
        double[] totalKmAndHrs = new double[2];
        int originIndex = 0;
        int destinationIndex = 0;
        string[] kmAndHrsSplit;

        string[] cities = new string[]
        {
            "Windsor",
            "London",
            "Hamilton",
            "Toronto",
            "Oshawa",
            "Belleville",
            "Kingston",
            "Ottawa"
        };

        string[] kmAndHrs = new string[]
        {
            "191|2.5",
            "128|1.75",
            "68|1.25",
            "60|1.3",
            "134|1.65",
            "82|1.2",
            "196|2.5"
        };

        for (int i = 0; i < cities.Length; i++)
        {
            if (cities[i] == origin)
            {
                originIndex = i;
            }
            if (cities[i] == destination)
            {
                destinationIndex = i;
            }

        }

        //If the route is going from west to east. Originindex is less than destination index
        if (originIndex < destinationIndex)
        {
            while (originIndex < destinationIndex)
            {
                kmAndHrsSplit = kmAndHrs[originIndex].Split('|');
                totalKmAndHrs[0] += double.Parse(kmAndHrsSplit[0]);
                totalKmAndHrs[1] += double.Parse(kmAndHrsSplit[1]);
                originIndex++;
            }
        }

        //If the route is going from east to west. Originindex is greater than destination index
        else if (originIndex > destinationIndex)
        {
            while (originIndex > destinationIndex)
            {
                originIndex--;
                kmAndHrsSplit = kmAndHrs[originIndex].Split('|');
                totalKmAndHrs[0] += double.Parse(kmAndHrsSplit[0]);
                totalKmAndHrs[1] += double.Parse(kmAndHrsSplit[1]);

            }
        }


        return totalKmAndHrs;
    }

    //GenerateInvoice(Order order)
    //{
    //    Invoice invoice = new Invoice();
    //    invoice.OrderId = order.OrderId;
    //    LoggerModel.LogInfo("Order Id")
    //}

    /*
    * METHOD NAME: CalculateRate
    * DESCRIPTION: Calculates the cost of the rates for carriers
    *
    * RETURN: double[] profit for TMS and carrier
    */
    // We can change the arguements, we can just take in a Order and all the info needed is in the order
    public double[] CalculateRate(double totalKm, int vanType, int quantity, int job_type)
    {

        double ftlRate = 0.2995; // sample rates
        double ltlRate = 4.986;
        double[] totalAmount = new double[2];

        //ftl
        if (job_type == 0)
        {
            //reefer van
            if (vanType == 1)
            {
                ftlRate += 0.13 * ftlRate;
                double amount = ftlRate * totalKm;
                totalAmount[0] = amount * .08;             // Money gain for TMS
                totalAmount[1] = amount - totalAmount[0];  // Money gain for carrier
                return totalAmount;
            }
            else if (vanType == 0)
            {
                ftlRate += .08 * ftlRate;
                double amount = ftlRate * totalKm;
                totalAmount[0] = amount * .08;             // Money gain for TMS
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
                ltlRate += 0.10 * ltlRate;
                double amount = ltlRate * totalKm * quantity;

                totalAmount[0] = amount * .05;             // Money gain for TMS
                totalAmount[1] = amount - totalAmount[0];  // Money gain for carrier
                return totalAmount;

            }
            else if (vanType == 0)
            {
                ltlRate += .05 * ltlRate;
                double amount = ltlRate * totalKm * quantity;
                totalAmount[0] = amount * .05;             // Money gain for TMS
                totalAmount[1] = amount - totalAmount[0];  // Money gain for carrier
                return totalAmount;
            }


        }
        return totalAmount;

    }


}