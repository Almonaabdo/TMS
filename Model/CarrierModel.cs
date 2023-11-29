using TMS_Project.DataLayer.Context;

namespace TMS_Project.Model;

public class CarrierModel
{
    private readonly TmsDbContext _db;
    public CarrierModel(TmsDbContext context)
    {
        _db = context;
    }



    //List<string> AvaiableCities(string CompanyName)
    //{
    //    var carrier = _db.Carriers?.Find(CompanyName);

    //    return myList;
    //}
}