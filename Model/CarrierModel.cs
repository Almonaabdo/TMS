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

    public void CreateCarrier(string? name, int newFtla, int newLtla, double newFtlaRate, double newLtlaRate, double newReefCharge)
    {
        var newCarrier = new Carrier
        {
            CompanyName = name,
            FTLA = newFtla,
            LTLA = newLtla,
            FtlRate = newFtlaRate,
            LtlRate = newLtlaRate,
            ReefCharge = newReefCharge
        };

        _db?.Carriers?.Add(newCarrier);
        _db?.SaveChanges();
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