using System.Linq;
using TMS_Project.DataLayer.Context;
using TMS_Project.DataLayer.Model;

namespace TMS_Project.Model;

public class PlannerModel
{
    private readonly TmsDbContext _db;
    public PlannerModel()
    {
        _db = new TmsDbContext();
    }

    public Carrier? GetCarrier(string companyName, string originCityy)
    {
        var carrier = _db.Carriers?.FirstOrDefault(e => e.CompanyName == companyName && e.DepotCity == originCityy);
        if (carrier != null)
        {
            return carrier;
        }
        return null;
    }
}