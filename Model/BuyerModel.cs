using System.Windows.Documents;
using Microsoft.Extensions.Logging;

namespace TMS_Project.Model;
using DataLayer.Context;
using DataLayer.Model;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;

public class BuyerModel
{
    private readonly TmsDbContext _db = DbContextSingleton.Instance;
    private readonly ContractMarketPlaceDbContext _cdb = new ContractMarketPlaceDbContext();
    private readonly LoggerModel _loggerModel = LoggerModel.Instance;
    public BuyerModel()
    {
    }


    public Contract GetContracts()
    {

        Contract contract = new Contract();
        return contract;
    }


    /*
    * METHOD NAME: LoadContracts 
    * DESCRIPTION: Gets list contracts from CMP from DB
    * 
    * RETURN: list of contracts
    */
    public List<Contract> LoadContracts()
    {

        try
        {
            if (_cdb.Contracts != null)
            {

                List<Contract> contracts = _cdb.Contracts.ToList();
                return contracts.Any() ? contracts : new List<Contract>();

            }
        }

        catch (Exception e)
        {
            Console.WriteLine(e);
            _loggerModel.LogException("Error loading contract data.");
        }

        return new List<Contract>();

    }


    /*
    * METHOD NAME: AddCustomer
    * DESCRIPTION: Adds a new customer to the DB if doesn't exist
    * 
    * RETURN: void
    */
    public void AddCustomer(string name, string phoneNumber, string email)
    {
        var existingCustomer = _db.Customers?.FirstOrDefault(w => w.Email == email || w.PhoneNumber == phoneNumber);

        try
        {
            //no existing customer, proceed with adding the customer
            if (existingCustomer == null)
            {
                var newCustomer = new Customer();

                newCustomer.Name = name;
                newCustomer.PhoneNumber = phoneNumber;
                newCustomer.Email = email;
                int i = newCustomer.CustomerId;

                _db.Customers?.Add(newCustomer);
                _db.SaveChanges();
                _loggerModel.LogInfo($"Customer added successfully: {name}");
            }
            else
            {
                _loggerModel.LogWarning("Couldn't Add Customer as it already exists");
            }
        }
        catch(Exception e)
        {

        }
      
    }


    /*
    * METHOD NAME: DeleteCustomer
    * DESCRIPTION: Deletes an existing customer from the DB if found
    * 
    * RETURN: void
    */
    public void DeleteCustomer(int customerId)
    {
        // searching for the entered order
        var customer = _db.Customers?.Find(customerId);

        try
        {
            if (customer != null)
            {
                // remove order and save changes
                _db.Customers?.Remove(customer);
                _db.SaveChanges();
            }
            else
            {
                _loggerModel.LogWarning("Error Can't find specified Carrier");
            }
        }
        catch (Exception e) 
        { 

        }
       
    }


    /*
    * METHOD NAME: GetCity 
    * DESCRIPTION: Converts string city to City
    * 
    * RETURN: city
    */
    public City? GetCity(string cityName)
    {
        var city = _db.Cities?.FirstOrDefault(c => c.CityName == cityName);
        return city;
    }


    /*
    * METHOD NAME: FindCustomerByName
    * DESCRIPTION: Finds string customer from Customer DB
    * 
    * RETURN: customer
    */
    public Customer? FindCustomerByName(string customerName)
    {
        var customer = _db.Customers?.SingleOrDefault(c => c.Name == customerName);
        return customer;
    }


    /*
    * METHOD NAME: CreateCustomer
    * DESCRIPTION: Creates a new customer to be added to the DB
    * 
    * RETURN: void
    */
    public void CreateCustomer(string name)
    {
        try
        {
            var newCustomer = new Customer
            {
                Name = name
            };

            _db.Customers?.Add(newCustomer);
            _db.SaveChanges();
        }
        catch(Exception e)
        {

        }
        
    }


    /*
    * METHOD NAME: GetCompletedOrders 
    * DESCRIPTION: Gets list of completed orders from DB
    * 
    * RETURN: list of completed orders
    */
    public List<Order> GetCompletedOrders()
    {
        return _db.Orders?.Where(order => order.OrderStatus == OrderStatus.Completed).ToList() ?? throw new InvalidOperationException();
    }
}