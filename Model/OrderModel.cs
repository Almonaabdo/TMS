using System;
using TMS_Project.DataLayer.Context;
using TMS_Project.DataLayer.Model;
using System.Linq;
// ReSharper disable UnusedType.Global

namespace TMS_Project.Model;

public class OrderModel
{
    const double Ftlmarkup = 0.08;
    const double Ltlmarkup = 0.05;
    private readonly TmsDbContext _db;

    public OrderModel(TmsDbContext context)
    { 
        _db = context;
    }

    public Customer? FindCustomerByName(string customerName)
    {
        return _db.Customers?.SingleOrDefault(c => c.Name == customerName);
    }


    public Customer CreateCustomer(string name)
    {
        var newCustomer = new Customer
        {
            Name = name
        };

        _db.Customers?.Add(newCustomer);
        _db.SaveChanges();

        return newCustomer;
    }

    public string? GetCityById(int cityId)
    {
        string city = "";
        if (_db.Cities != null)
        {
            var cityFound = _db.Cities.FirstOrDefault(c => c.CityId == cityId);
            city = cityFound.CityName.ToString();
            return city;
        }

        return city;
    }
    public City? GetCity(string? cityName)
    {
        if (_db.Cities != null)
        {
            var city =_db.Cities.FirstOrDefault(c => c.CityName == cityName);
            return city;
        }

        return null;
    }


    /*
    * METHOD NAME: CreateOrder
    * DESCRIPTION: Creates new order by calling the database Add method and saves changes
    * 
    * RETURN: void
    */
    public void CreateOrder(City destCity, City? originCity, int customerId, int jobType, int quantity, int vanType)
    {
        try
        {
            var newOrder = new Order();

            newOrder.OrderStatus = OrderStatus.Pending;
            newOrder.DateInitiated = DateTime.Today;
            newOrder.DestinationCity = destCity;
            newOrder.SourceCity = originCity;
            newOrder.CustomerId = customerId;
            newOrder.Quantity = quantity;

            if(jobType == 0)
            {
                newOrder.JobType = JobType.Ftl;
            }
            else if(jobType == 1)
            {
                newOrder.JobType = JobType.Ltl;
            }
            
            if(vanType == 0)
            {
                newOrder.VanType = VanType.DryVan;
            }
            else if (vanType == 1) 
            {
                newOrder.VanType = VanType.Reefer;
            }
            

            _db.Orders?.Add(newOrder);
            _db.SaveChanges();
        }
        catch (Exception ex)
        {
            LoggerModel.LogError($"Order creating error: {ex.Message}");
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
            order.OrderStatus = OrderStatus.Completed;
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
                ftlRate += (reeferCharge + Ftlmarkup) * ftlRate;
                double amount = ftlRate * totalKm;
                totalAmount[0] = amount * Ftlmarkup;
                                                     // Money gain for TMS
                totalAmount[1] = amount - totalAmount[0];  // Money gain for carrier
                return totalAmount;
            }
            else if (vanType == 0)
            {
                ftlRate += Ftlmarkup * ftlRate;
                double amount = ftlRate * totalKm;
                totalAmount[0] = amount * Ftlmarkup;             // Money gain for TMS
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
                ltlRate += (Ltlmarkup + reeferCharge) * ltlRate;
                double amount = (ltlRate * totalKm) * quantity;

                totalAmount[0] = amount * Ltlmarkup;             // Money gain for TMS
                totalAmount[1] = amount - totalAmount[0];  // Money gain for carrier
                return totalAmount;

            }
            else if (vanType == 0)
            {
                ltlRate += Ltlmarkup * ltlRate;
                double amount = (ltlRate * totalKm) * quantity;
                totalAmount[0] = amount * Ltlmarkup;             // Money gain for TMS
                totalAmount[1] = amount - totalAmount[0];  // Money gain for carrier
                return totalAmount;
            }


        }
        return totalAmount;

    }


}