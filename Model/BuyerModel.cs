
namespace TMS_Project.Model;
using DataLayer.Context;
using DataLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;

public class BuyerModel
{

    #region  Fields

    private readonly ContractMarketPlaceDbContext _cdb = new ContractMarketPlaceDbContext();

    #endregion

    #region Methods

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
            // get contract and converts it to a list if not empty
            if (_cdb.Contracts != null)
            {
                List<Contract> contracts = _cdb.Contracts.ToList();
                return contracts.Any() ? contracts : new List<Contract>();
            }
        }
        catch (Exception e)
        {
            LoggerModel.Instance.LogException($"Error loading contract data. {e.Message}");
        }

        return new List<Contract>(); // Return empty list if exception

    }


    #endregion
}