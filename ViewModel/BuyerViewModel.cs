using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMS_Project.DataLayer.Context;
using TMS_Project.DataLayer.Model;
using TMS_Project.Model;
using TMS_Project.ViewModel;

namespace TMS.ViewModel;

public class BuyerViewModel
{
    #region Fields

    public IEnumerable<Contract>? ContractData { get; private set; }

    private readonly TmsDbContext _tmsDbContext = new();
    private readonly BuyerModel _buyerModel;
    public LogInViewModel LogInViewModel { get; private set; } = new();

    #endregion

    #region Constructor

    public BuyerViewModel()
    {
        _buyerModel = new BuyerModel(_tmsDbContext);
        LoadData();
    }

    #endregion

    #region Methods

    public void LoadData()
    {
        var loadedContracts = _buyerModel.LoadContracts();
        if (loadedContracts != null)
            ContractData = new ObservableCollection<Contract>(loadedContracts);
        else
            ContractData = new ObservableCollection<Contract>();
    }

    #endregion
}