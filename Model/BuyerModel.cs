using DataLayer.Context;
using DataLayer.Model;
using Model;
using System;

namespace TMS_Project.Model;

public class BuyerModel
{
     TmsDbContext _db;

    public BuyerModel()
    {

    }


    public void addCustomer(string name, string phoneNumer, string email)
    {
        var newCustomer = new Customer();
        newCustomer.Name = name;
        newCustomer.PhoneNumber = phoneNumer;
        newCustomer.Email = email;  
        _db.Customers.Add(newCustomer);
        _db.SaveChanges();
        
    }

    
}