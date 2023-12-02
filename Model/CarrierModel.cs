using System;
using TMS_Project.DataLayer.Context;
using TMS_Project.DataLayer.Model;

namespace TMS_Project.Model;

public class CarrierModel
{
    private readonly TmsDbContext _db;
    public CarrierModel(TmsDbContext context)
    {
        _db = context;
    }


    public void DeleteCarrier(int carrierId)
    {
        // searching for the entered order
        var carrier = _db.Carriers?.Find(carrierId);

        if (carrier != null)
        {
            // remove order and save changes
            _db.Carriers?.Remove(carrier);
            _db.SaveChanges();
        }
        else
        {
            Console.Write("Error Can't find specified Carrier");
        }
    }



    public bool AddCarrier(Carrier newCarrier)
    {
        // searching for the entered order
        _db.Carriers?.Add(newCarrier);
        _db.SaveChanges();

        int carrierID = newCarrier.CarrierId;

        if (_db.Carriers.Find(carrierID) != null)
        {
            return true;
        }
        else
        {
            Console.Write("Error While Adding Carrier");
            return false;
        }
    }


    public void EditCarrier(int carrierId)
    {
        // searching for the entered order
        var carrier = _db.Carriers?.Find(carrierId);

        if (carrier != null)
        {
            // remove carriers from database and save changes
            _db.Carriers?.Remove(carrier);
            _db.SaveChanges();
        }
        else
        {
            Console.Write("Error Can't find specified Carrier");
        }
    }


    public void EditFtl(Carrier carrierId, double newRate)
    {
        // searching for the entered order
        var carrier = _db.Carriers?.Find(carrierId);

        if (carrier != null)
        {
            carrier.FtlRate = newRate;
        }
        else
        {
            Console.Write("Error Can't find specified Carrier");
        }
    }
}