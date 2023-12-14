using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TMS_Project.DataLayer.Context;
using TMS_Project.DataLayer.Model;
using TMS_Project.Helper;
using TMS_Project.Model;

namespace TMS_Project.ViewModel;

public class BuyerViewModel: ViewModelBase
{
    #region Fields

    public IEnumerable<Contract>? ContractData { get; private set; }
    public ICommand CreateOrderCommand { get; }
    public List<JoinedOrder> CompletedOrders { get; private set; } = new List<JoinedOrder>();

    private readonly TmsDbContext _tmsDbContext = DbContextSingleton.Instance;
    private readonly BuyerModel _buyerModel;
    private readonly DataService _dataService;
    public LogInViewModel LogInViewModel { get; private set; } = new();
    private readonly OrderModel _orderModelObject;  


    private Contract _selectedContract;
    public Contract SelectedContract
    {
        get => _selectedContract;
        set
        {
            _selectedContract = value;
            OnPropertyChanged(nameof(SelectedContract));
        }
    }


    private string _buyerNotification;
    public string BuyerNotification
    {
        get => _buyerNotification;
        set
        {
            _buyerNotification = value;
            OnPropertyChanged(nameof(BuyerNotification));
        }
    }
    #endregion

    #region Constructor

    public BuyerViewModel()
    {
        _buyerModel = new BuyerModel(_tmsDbContext);
        LoadData();
        CreateOrderCommand = new RelayCommand(CallCreateOrder);
        _orderModelObject = new OrderModel(_tmsDbContext);
        _dataService = new DataService();

        // Calculate the counts
      //  int completedOrdersCount = CompletedOrders.Count;
       // int contractsCount = ContractData?.Count() ?? 0;
        // Set BuyerNotification based on the counts
        //BuyerNotification = $"You have {completedOrdersCount} completed orders and {contractsCount} contracts.";
    }

    #endregion

    #region Methods

    private void LoadData()
    {
        var loadedContracts = _buyerModel.LoadContracts();
        ContractData = new ObservableCollection<Contract>(loadedContracts);
       // CompletedOrders = _dataService.GetJoinedOrder().ToList();
    }
    #endregion

    private void CallCreateOrder()
    {
        try
        {
            if (SelectedContract.Destination == null)
                return;

            var contractDestCity = _orderModelObject.GetCity(SelectedContract.Destination);
            if (contractDestCity == null)
                throw new ArgumentNullException($"GetCity({nameof(SelectedContract)}.Destination)");

            if (SelectedContract.Origin == null)
                return;

            var contractOriginCity = _orderModelObject.GetCity(SelectedContract.Origin);

            if (SelectedContract.Client_Name == null)
                return;

            var customer = _orderModelObject.FindCustomerByName(SelectedContract.Client_Name);
            if (customer == null)
            {
                customer = _orderModelObject.CreateCustomer(SelectedContract.Client_Name);

                _orderModelObject.CreateOrder(contractDestCity, contractOriginCity, customer.CustomerId);
            }
            else
            {
                _orderModelObject.CreateOrder(contractDestCity, contractOriginCity, customer.CustomerId);
            }

            MessageBox.Show("Successfully accepted customer. A new order has been created.", "New customer added",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error {ex.Message}.");
        }
    }



   
}