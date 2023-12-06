namespace TMS_Project.Model;
using DataLayer.Context;
using DataLayer.Model;
using Model;
using System;



public class BuyerModel
{
    private readonly TmsDbContext _db;

    public BuyerModel(TmsDbContext context)
    {
        _db = context;
    }



    public Contract GetContracts()
    {

        Contract contract = new Contract();
        return contract;
    }


    public void AddCustomer(string name, string phoneNumber, string email)
    {

        var newCustomer = new Customer();
        newCustomer.Name = name;
        newCustomer.PhoneNumber = phoneNumber;
        newCustomer.Email = email;
        int i = newCustomer.CustomerId;

        if (_db.Customers.Find(i) == null)
        {
            _db.Customers.Add(newCustomer);
            _db.SaveChanges();
        }
        else
        {
            LoggerModel.LogError("Can't add Duplicates. Customer Already exists");
            return;
        }

        if (_db.Customers.Find(i) == null)
        {
            LoggerModel.LogError("Couldn't Add Customer");
        }
    }


    public void DeleteCustomer(int customerId)
    {
        // searching for the entered order
        var customer = _db.Customers?.Find(customerId);

        if (customer != null)
        {
            // remove order and save changes
            _db.Customers?.Remove(customer);
            _db.SaveChanges();
        }
        else
        {
            LoggerModel.LogError("Error Can't find specified Carrier");
        }
    }
}