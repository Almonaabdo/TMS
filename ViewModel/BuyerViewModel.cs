using System.Collections.ObjectModel;
using System;
using TMS_Project.DataLayer.Model;

using TMS_Project.Model;
using System.Collections.Generic;
using TMS_Project.DataLayer.Context;

namespace TMS.ViewModel;
public class BuyerViewModel
{

   public IEnumerable<Contract>? ContractData { get; private set; }
    
    private readonly TmsDbContext _tmsDbContext;
   private readonly BuyerModel _buyerModel;

   public BuyerViewModel()
    {
        _buyerModel = new BuyerModel(_tmsDbContext);
        LoadData();
    }
    public void LoadData()
    {
        ContractData = new ObservableCollection<Contract>(_buyerModel.LoadContracts() ?? throw new InvalidOperationException());
       
    }
}