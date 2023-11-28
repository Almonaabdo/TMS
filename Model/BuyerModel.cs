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
        if(job_type == 0)
        {
            //reefer van
            if(vanType == 1) 
            {
                ftlRate += 0.13 * ftlRate;
                double amount = ftlRate * totalKm;
                totalAmount[0] = amount * .08;             // Money gain for TMS
                totalAmount[1] = amount - totalAmount[0];  // Money gain for carrier
                return totalAmount;
            }
            else if(vanType == 0) 
            {
                ftlRate += .08 * ftlRate;
                double amount = ftlRate * totalKm;
                totalAmount[0] = amount * .08;             // Money gain for TMS
                totalAmount[1] = amount - totalAmount[0];  // Money gain for carrier
                return totalAmount;
            }
        }
        //ltl
        else if(job_type == 1) 
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