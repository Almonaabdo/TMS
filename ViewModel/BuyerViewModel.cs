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
    public ObservableCollection<Order> CompletedOrders { get; set; } = new ObservableCollection<Order>();

    private readonly TmsDbContext _tmsDbContext = DbContextSingleton.Instance;
    private readonly BuyerModel _buyerModel;
    public LogInViewModel LogInViewModel { get; private set; } = new();
    public readonly OrderModel OrderModelObject;  


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
        OrderModelObject = new OrderModel(_tmsDbContext);

        LoadCompleteOrder();

        // Calculate the counts
        int completedOrdersCount = CompletedOrders.Count;
        int contractsCount = ContractData?.Count() ?? 0;
        // Set BuyerNotification based on the counts
        BuyerNotification = $"You have {completedOrdersCount} completed orders and {contractsCount} contracts.";
    }

    #endregion

    #region Methods

    public void LoadData()
    {
        var loadedContracts = _buyerModel.LoadContracts();
        ContractData = new ObservableCollection<Contract>(loadedContracts);
    }
    #endregion

    private void LoadCompleteOrder()
    {
        try
        {
            var complete = _buyerModel.GetCompletedOrders();
            CompletedOrders.Clear();

            foreach (var order in complete)
            {
                CompletedOrders.Add(order);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }


    private void CallCreateOrder()
    {
        try
        {
            if (SelectedContract.Destination == null)
                return;

            var contractDestCity = OrderModelObject.GetCity(SelectedContract.Destination);
            if (contractDestCity == null)
                throw new ArgumentNullException($"GetCity({nameof(SelectedContract)}.Destination)");

            if (SelectedContract.Origin == null)
                return;

            var contractOriginCity = OrderModelObject.GetCity(SelectedContract.Origin);

            if (SelectedContract.Client_Name == null)
                return;

            var customer = OrderModelObject.FindCustomerByName(SelectedContract.Client_Name);
            if (customer == null)
            {
                customer = OrderModelObject.CreateCustomer(SelectedContract.Client_Name);

                OrderModelObject.CreateOrder(contractDestCity, contractOriginCity, customer.CustomerId);
            }
            else
            {
                OrderModelObject.CreateOrder(contractDestCity, contractOriginCity, customer.CustomerId);
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