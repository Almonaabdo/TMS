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
    private ObservableCollection<JoinedOrder> _completedOrders;
    public ObservableCollection<JoinedOrder> CompletedOrders
    {
        get => _completedOrders;
        set
        {
            _completedOrders = value;
            OnPropertyChanged(nameof(CompletedOrders));
        }
    }
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
 
    #endregion

    #region Constructor

    public BuyerViewModel()
    {
        _buyerModel = new BuyerModel(_tmsDbContext);
      
        CreateOrderCommand = new RelayCommand(CallCreateOrder);
        _orderModelObject = new OrderModel(_tmsDbContext);
        _dataService = new DataService(); 
        LoadData();
    }

    #endregion

    #region Methods

    private void LoadData()
    {
        try
        {
            var loadedContracts = _buyerModel.LoadContracts();
            ContractData = new ObservableCollection<Contract>(loadedContracts);
            CompletedOrders = new ObservableCollection<JoinedOrder>(_dataService.GetCompletedOrders());
        }
        catch (Exception e)
        {
            Console.WriteLine(e + e.Source + e.StackTrace + e.InnerException);
        }
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