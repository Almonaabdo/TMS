namespace TMS_Project.Model;
using DataLayer.Context;
using DataLayer.Model;
using Model;
using System;



public class BuyerModel
{
    private  TmsDbContext _db;

    public BuyerModel()
    {
       
    }

    

    public Contract GetContracts()
    {
        Contract contract = new Contract();
        return contract;
    }

    //public void DisplayContracts()
    //{
    //    Contract contract = new Contract();
        
      
    //}


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