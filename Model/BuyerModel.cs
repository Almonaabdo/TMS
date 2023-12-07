namespace TMS_Project.Model;
using DataLayer.Context;
using DataLayer.Model;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;

public class BuyerModel
{
    private readonly TmsDbContext _db = new TmsDbContext();
    private readonly ContractMarketPlaceDbContext _cdb = new ContractMarketPlaceDbContext();

    public BuyerModel(TmsDbContext context)
    {

        _db = context;
    }



    public Contract GetContracts()
    {

        Contract contract = new Contract();
        return contract;
    }

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
            LoggerModel.LogException("Error loading contract data.");
        }

        return new List<Contract>();

    }


    public void AddCustomer(string name, string phoneNumber, string email)
    {
        var existingCustomer = _db.Customers?.FirstOrDefault(w => w.Email == email || w.PhoneNumber == phoneNumber);

        if (existingCustomer == null)
        {
            var newCustomer = new Customer();

            newCustomer.Name = name;
            newCustomer.PhoneNumber = phoneNumber;
            newCustomer.Email = email;
            int i = newCustomer.CustomerId;

            _db.Customers?.Add(newCustomer);
            _db.SaveChanges();
            LoggerModel.LogInfo($"Customer added successfully: {name}");
        }
        else
        {
            LoggerModel.LogWarning("Couldn't Add Customer as it already exists");
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
            LoggerModel.LogWarning("Error Can't find specified Carrier");
        }
    }
}