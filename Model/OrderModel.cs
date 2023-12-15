using System;
using TMS_Project.DataLayer.Model;
using System.Linq;
// ReSharper disable UnusedType.Global

namespace TMS_Project.Model;

public class OrderModel
{
    #region Fields

    private const double Ftlmarkup = 0.08;
    private const double Ltlmarkup = 0.05;

    #endregion
   

    #region Methods

    /*
    * METHOD NAME: FindCustomerByName
    * DESCRIPTION: Finds a customer from the DB through name
    *
    * RETURN: found customer
    */
    public Customer? FindCustomerByName(string customerName)
    {
        return DbContextSingleton.Instance.Customers?.SingleOrDefault(c => c != null && c.Name == customerName);
    }


    /*
    * METHOD NAME: CreateCustomer
    * DESCRIPTION: Creates a new customer and is added to the DB
    *
    * RETURN: newCustomer
    */
    public Customer? CreateCustomer(string name)
    {
        try
        {
            var newCustomer = new Customer
            {
                Name = name
            };

            DbContextSingleton.Instance.Customers?.Add(newCustomer);
            DbContextSingleton.Instance.SaveChanges();

            return newCustomer;
        }
        catch(Exception e)
        {
            LoggerModel.Instance.LogException($"{e.Message}");
        }

        return null;
    }


    /*
    * METHOD NAME: GetCityById
    * DESCRIPTION: Gets city from DB through ID and if found, convert to string
    *
    * RETURN: city
    */
    public string? GetCityById(int cityId)
    {
        try
        {
            string? city = "";
            if (DbContextSingleton.Instance.Cities != null)
            {
                var cityFound = DbContextSingleton.Instance.Cities.FirstOrDefault(c => c.CityId == cityId);
                city = cityFound?.CityName;
                LoggerModel.Instance.LogInfo($"{city} Found");
                return city;
            }

            return city;
        }
        catch
        {
            LoggerModel.Instance.LogError("Finding the city by id failed");
            return null;
           
        }
    }


    /*
    * METHOD NAME: CreateOrder
    * DESCRIPTION: Creates new order by calling the database Add method and saves changes
    * 
    * RETURN: void
    */
    public City? GetCity(string? cityName)
    {
        try
        {
            if (DbContextSingleton.Instance.Cities != null)
            {
                var city = DbContextSingleton.Instance.Cities.FirstOrDefault(c => c.CityName == cityName);
                return city;
            }
        }
        catch (Exception e)
        {
            LoggerModel.Instance.LogException($"{e.Message}");
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
            //Creates a new order with new properties
            var newOrder = new Order
            {
                OrderStatus = OrderStatus.Pending,
                DateInitiated = DateTime.Now,
                DestinationCity = destCity,
                SourceCity = originCity,
                CustomerId = customerId,
                Quantity = quantity
            };

            //Job type and van type parsing
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


            DbContextSingleton.Instance.Orders?.Add(newOrder);
            DbContextSingleton.Instance.SaveChanges();
            LoggerModel.Instance.LogInfo($"{newOrder.OrderId} Successfully created");
        }
        catch (Exception ex)
        {
            LoggerModel.Instance.LogError($"Order creating error: {ex.Message}");
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
        try
        {
            // searching for the entered order
            var order = DbContextSingleton.Instance.Orders?.Find(orderId);

            if (order != null)
            {
                // remove order and save changes
                DbContextSingleton.Instance.Orders?.Remove(order);
                DbContextSingleton.Instance.SaveChanges();
            }
            else
            {
                LoggerModel.Instance.LogError("Info: Specified Order Wasn't Found In Database.");
            }
        }
        catch(Exception e)
        {
            LoggerModel.Instance.LogException($"{e.Message}");
        }
        
    }


    /*
    * METHOD NAME: CompleteOrder
    * DESCRIPTION: Changes status of specified order, and completed order data as today's date.
    *
    * RETURN: void
    */
    public void CompleteOrder(int orderId, DateTime date)
    {
        try
        {
            // searching for the entered order
            var order = DbContextSingleton.Instance.Orders?.Find(orderId);

            if (order != null)
            {
                // updating the status of the found order by changing status and dateCompleted.
                UpdateTripStatus(orderId, TripStatus.Completed);
                order.OrderStatus = OrderStatus.Completed;
                // changing dataCompleted to the current date of today.
                order.DateCompleted = date;

                DbContextSingleton.Instance.SaveChanges();
            }
            else
            {
                LoggerModel.Instance.LogError("Info: Can't Complete order! Specified Order Wasn't Found In Database.");
            }
        }
        catch (Exception e)
        {
            LoggerModel.Instance.LogException($"{e.Message}");
        }
    }


    /*
    * METHOD NAME: UpdateTripStatus
    * DESCRIPTION: Deletes an order from DB if it's found
    * 
    * RETURN: void
    */
    private void UpdateTripStatus(int tripId, TripStatus newStatus)
    {
        try
        {
            // find specified trip.
            var trip = DbContextSingleton.Instance.Trips?.Find(tripId);


            if (trip != null)
            {
                // check if new status matches old status
                if (trip.TripStatus != newStatus)
                {
                    // update trip if it's found and doesn't match
                    trip.TripStatus = newStatus;
                    DbContextSingleton.Instance.SaveChanges();
                }
                else
                {
                    LoggerModel.Instance.LogError("Info: Trip Status Wasn't change as new status remains the same");
                }
            }
            else
            {
                LoggerModel.Instance.LogError("Info: Can't Change Trip Status! Specified trip Wasn't Found In Database.");
            }
        }
        catch(Exception e)
        {
            LoggerModel.Instance.LogException($"{e.Message}");
        }
    }


    /*
    * METHOD NAME: GetKmAndHrs
    * DESCRIPTION: Gets the total Km and hours needed for the carrier to reach the destination from the origin
    * 
    * RETURN: double[], [0] = totalKm, [1] = totalHrs
    */
    public double[]? GetKmAndHrs(string destination, string origin)
    {
        try
        {
            var totalKmAndHrs = new double[2];

            var route = DbContextSingleton.Instance.Routes?.FirstOrDefault(e =>
                e.SourceCity.CityName == origin && e.DestinationCity.CityName == destination);

            if (route != null)
            {
                totalKmAndHrs[0] = Math.Round(route.Distance, 3);
                totalKmAndHrs[1] = Math.Round(route.Duration, 3);
                LoggerModel.Instance.LogInfo($"Successfully found total km and hrs. Total Km: {totalKmAndHrs[0]} Total hours: {totalKmAndHrs[1]}");
            }


            return totalKmAndHrs;
        }
        catch
        {
            LoggerModel.Instance.LogError("Getting the km and hours of the specific route failed");
        }
        return null;
    }


    /*
    * METHOD NAME: CalculateRate
    * DESCRIPTION: Calculates the cost of the rates for carriers
    *
    * RETURN: double[] profit for TMS and carrier
    */
    public double[] CalculateRate(Carrier carrier, double totalKm, int vanType, double quantity, int jobType)
    {
        double[] totalAmount = new double[2];
        try
        {
            //Assigns the carrier rates
            double ftlRate = carrier.FtlRate;
            double ltlRate = carrier.LtlRate;
            double reeferCharge = carrier.ReefCharge;

            

            //ftl
            if (jobType == 0)
            {
                //reefer van
                if (vanType == 1)
                {
                    ftlRate += (reeferCharge + Ftlmarkup) * ftlRate; //add a markup for the charge 
                    double amount = ftlRate * totalKm;
                    totalAmount[0] = amount * Ftlmarkup;
                    // Money gain for TMS
                    totalAmount[1] = amount - totalAmount[0];  // Money gain for carrier
                    LoggerModel.Instance.LogInfo($"Successfully calculated the amount Tms money: {totalAmount[0]} Truck money: {totalAmount[1]}");
                    return totalAmount;
                    
                }
                else if (vanType == 0)
                {
                    ftlRate += Ftlmarkup * ftlRate;             //add a markup for the charge
                    double amount = ftlRate * totalKm;
                    totalAmount[0] = amount * Ftlmarkup;             // Money gain for TMS
                    totalAmount[1] = amount - totalAmount[0];  // Money gain for carrier
                    LoggerModel.Instance.LogInfo($"Successfully calculated the amount Tms money: {totalAmount[0]} Truck money: {totalAmount[1]}");
                    return totalAmount;
                }
                
            }
            //ltl
            else if (jobType == 1)
            {
                //reefer van
                if (vanType == 1)
                {
                    ltlRate += (Ltlmarkup + reeferCharge) * ltlRate;    //add a markup for the charge
                    double amount = (ltlRate * totalKm) * quantity;

                    totalAmount[0] = amount * Ltlmarkup;             // Money gain for TMS
                    totalAmount[1] = amount - totalAmount[0];  // Money gain for carrier
                    LoggerModel.Instance.LogInfo($"Successfully calculated the amount Tms money: {totalAmount[0]} Truck money: {totalAmount[1]}");
                    return totalAmount;

                }
                else if (vanType == 0)
                {
                    ltlRate += Ltlmarkup * ltlRate;                     //add a markup for the charge
                    double amount = (ltlRate * totalKm) * quantity;
                    totalAmount[0] = amount * Ltlmarkup;             // Money gain for TMS
                    totalAmount[1] = amount - totalAmount[0];  // Money gain for carrier
                    LoggerModel.Instance.LogInfo($"Successfully calculated the amount Tms money: {totalAmount[0]} Truck money: {totalAmount[1]}");
                    return totalAmount;
                }
               
            }
            return totalAmount;
        }
        catch(Exception e)
        {
            LoggerModel.Instance.LogException($"Calculating the total cost failed. The arguments values might be wrong. {e.Message}");
            return totalAmount;
        }

    }

#endregion
}