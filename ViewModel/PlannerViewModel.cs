using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TMS_Project.DataLayer.Model;
using TMS_Project.Helper;
using TMS_Project.Model;

namespace TMS_Project.ViewModel;

public class PlannerViewModel : ViewModelBase
{
    #region Properties

    public DateTime CurrentDate
    {
        get => _currentDate;
        set
        {
            _currentDate = value;
            OnPropertyChanged(nameof(CurrentDate));
        }
    }

    public Order? SelectedOrder
    {
        get => _selectedOrder;
        set
        {
            _selectedOrder = value;
            OnPropertyChanged(nameof(SelectedOrder));
        }
    }

    public Order? SelectedInProgressOrder
    {
        get => _selectedInProgressOrder;
        set
        {
            _selectedInProgressOrder = value;
            OnPropertyChanged(nameof(SelectedInProgressOrder));
        }
    }

    public string? SelectedCarrier
    {
        get => _selectedCarrier;
        set
        {
            _selectedCarrier = value;
            OnPropertyChanged(nameof(SelectedCarrier));
        }
    }

    public string? AnotherSelectedCarrier
    {
        get => _anotherselectedCarrier;
        set
        {
            _anotherselectedCarrier = value;
            OnPropertyChanged(nameof(AnotherSelectedCarrier));
        }
    }

    public List<string?>? CarrierNames
    {
        get => _carrierNames;
        set
        {
            _carrierNames = value;
            OnPropertyChanged(nameof(CarrierNames));
        }
    }

    #endregion

    #region Commands

    public ICommand RefreshInvoiceCommand { get; set; }
    public RelayCommand CompleteOrderCommand { get; set; }
    public ICommand? GetOrdersCommand { get; }
    public ICommand AddCarrierCommand { get; }
    public ICommand OrdersTabCommand { get; }
    public ICommand ActiveOrdersTabCommand { get; }
    public ICommand TwoWeeksTimeCommand { get; set; }
    public ICommand AllTimeCommand { get; set; }
    public ICommand IncrementDayCommand { get; set; }

    #endregion

    #region Fields

    private string? _selectedCarrier;
    private Order? _selectedOrder;
    private Order? _selectedInProgressOrder;
    private string? _anotherselectedCarrier;
    private DateTime _currentDate;
    public LogInViewModel LogInViewModel { get; set; } = new();
    private List<string?>? _carrierNames;
    private OrderModel? OrderModel { get; set; }
    private CarrierViewModel? CarrierViewModel { get; set; }
    public ObservableCollection<Order?> OrderData { get; private set; } = null!;
    public ObservableCollection<Invoice> Invoices { get; private set; } = null!;
    public ObservableCollection<Carrier>? CarrierData { get; private set; }
    private PlannerModel? PlannerModel { get; set; }
    public ObservableCollection<Order?> OrderDataInProgress { get; private set; } = null!;
    private readonly ConfigService _configService = new();
    #endregion

    #region Constructor and initiliazer

    /*
     * METHOD NAME: PlannerViewModel
     * DESCRIPTION: Constructor to initialize fields
     *
     * RETURN: void
     */
    public PlannerViewModel()
    {
        // binding commands

        InvoiceFiles = LoadAllFiles();
        RefreshInvoiceCommand = new RelayCommand(RefreshInvoice);
        AllTimeCommand = new RelayCommand(AllTimeSummary);
        TwoWeeksTimeCommand = new RelayCommand(TwoWeekSummary);
        CompleteOrderCommand = new RelayCommand(CallCompleteOrder, CanCallCompleteOrder);
        AddCarrierCommand = new RelayCommand(AddCarrier, CanAddCarrier);
        ActiveOrdersTabCommand = new RelayCommand(RefreshActiveOrdersTab);
        OrdersTabCommand = new RelayCommand(RefreshOrdersTab);
        IncrementDayCommand = new RelayCommand(IncrementOneDay);
        _currentDate = DateTime.Today;
        Initialize();
    }

    private InvoiceGeneratorModel? _generator;

    private void AllTimeSummary()
    {
        _generator?.ReportAllTime();
    }

    private void TwoWeekSummary()
    {
        _generator?.ReportTwoWeeks();
    }

    /*
     * METHOD NAME:  Initialize
     * DESCRIPTION: Initializes models and methods
     *
     * RETURN: void
     */
    private void Initialize()
    {
        GetOrderTable();
        OrderModel = new OrderModel();
        CarrierViewModel = new CarrierViewModel();
        CarrierData = CarrierViewModel.CarrierData;
        PlannerModel = new PlannerModel();
        GetDistinctCarrierNames();
        GetPendingOrders();
        GetInProgressOrders();
        GetAllTimeInvoices();
        _generator = new InvoiceGeneratorModel();
    }

    #endregion

    #region Small methods for updating tables

    /*
     * METHOD NAME: RefreshOrdersTab
     * DESCRIPTION: Refreshes the Orders tab
     *
     * RETURN: void
     */
    private void RefreshOrdersTab()
    {
        //Use the method the populates the orders
        GetPendingOrders();

        //Update the data when it is changed
        GetOrderTable();
    }

    /*
     * METHOD NAME: RefreshActiveOrdersTab
     * DESCRIPTION: Refreshes the Active Orders tab
     *
     * RETURN: void
     */
    private void RefreshActiveOrdersTab()
    {
        //Use the method the populates the orders in progress
        GetInProgressOrders();

        //Update the data when it is changed
        GetOrderInProgressTable();
    }

    /*
     * METHOD NAME: GetPendingOrders
     * DESCRIPTION: Gets the pending orders
     *
     * RETURN: void
     */
    private void GetPendingOrders()
    {
        //Store the observable collection to OrderData
        OrderData = new ObservableCollection<Order?>(
            (DbContextSingleton.Instance.Orders?.Where(order => order.OrderStatus == OrderStatus.Pending).ToList() ??
             throw new InvalidOperationException())!);
    }

    /*
     * METHOD NAME: GetAllTimeInvoices
     * DESCRIPTION: Gets all time invoices
     *
     * RETURN: void
     */
    private void GetAllTimeInvoices()
    {
        //Store the observable collection to OrderDataInProgress
        Invoices = new ObservableCollection<Invoice>(DbContextSingleton.Instance.Invoice?.ToList() ??
                                                     throw new InvalidOperationException());
    }

    private ObservableCollection<string>? LoadAllFiles()
    {
        string? path = _configService.GetInvoicePath();

        if (Directory.Exists(path))
        {
            var files = Directory.GetFiles(path, "*.txt");
            return new ObservableCollection<string>(files);
        }

        return null;
    }

    private void RefreshInvoice()
    {
        InvoiceFiles = LoadAllFiles();
    }

    private ObservableCollection<string>? _invoiceFiles = new ObservableCollection<string>();

    public ObservableCollection<string>? InvoiceFiles
    {
        get => _invoiceFiles;
        set
        {
            _invoiceFiles = value;
            OnPropertyChanged(nameof(InvoiceFiles));
        }
    }
      

    /*
     * METHOD NAME: GetInProgressOrders
     * DESCRIPTION: Gets the in progress orders
     *
     * RETURN: void
     */
    private void GetInProgressOrders()
    {
        //Store the observable collection to OrderDataInProgress
        OrderDataInProgress = new ObservableCollection<Order?>(
            (DbContextSingleton.Instance.Orders?.Where(order => order.OrderStatus == OrderStatus.InProgress).ToList() ??
             throw new InvalidOperationException())!);
    }

    /*
     * METHOD NAME: GetDistinctCarrierNames
     * DESCRIPTION: Gets distinct carrier names for no repeats
     *
     * RETURN: void
     */
    private void GetDistinctCarrierNames()
    {
        // Only select distinct names
        if (DbContextSingleton.Instance.Carriers != null)
            CarrierNames = DbContextSingleton.Instance.Carriers.Select(c => c.CompanyName).Distinct().ToList();
    }

    /*
     * METHOD NAME: GetOrderTable
     * DESCRIPTION: On property change up data OrderData
     *
     * RETURN: void
     */
    private void GetOrderTable()
    {
        OnPropertyChanged(nameof(OrderData));
    }

    /*
     * METHOD NAME: GetOrderInProgressTable
     * DESCRIPTION: On property change up data OrderDataInProgress
     *
     * RETURN: void
     */
    private void GetOrderInProgressTable()
    {
        OnPropertyChanged(nameof(OrderDataInProgress));
    }

    #endregion

    #region Increment day method

    /*
     * METHOD NAME: IncrementOneDay
     * DESCRIPTION: Completes all orders and add one day to date time
     *
     * RETURN: void
     */
    private void IncrementOneDay()
    {
        if (OrderDataInProgress == null)
        {
            MessageBox.Show("There are no orders to complete. Refresh to load new orders.",
                "Increment Date by one day");
            return;
        }

        var incrementDay = MessageBoxButton.OKCancel;
        var choice = MessageBox.Show("This will complete orders. Do you want to proceed?",
            "Increment Date by one day", incrementDay);

        if (choice == MessageBoxResult.Cancel) return;

        try
        {
            CompleteInProgressOrders();
            RefreshActiveOrdersTab();
        }
        catch
        {
           LoggerModel.Instance.LogError("Completing all orders by incrementing date by one day failed");
            MessageBox.Show("Completing all orders by incrementing date by one day failed", "Error");
        }
    }

    #endregion

    #region Complete orders in progress

    private void CompleteInProgressOrders()
    {
        CurrentDate = DateTime.Now.AddDays(1);

        foreach (var order in OrderDataInProgress)
        {
            if (order != null)
            {
                OrderModel?.CompleteOrder(order.OrderId, CurrentDate);
                LoggerModel.Instance.LogInfo($"Automatically completed {order.OrderId}");
            }
        }
    }

    #endregion

    #region Add carrier method

    /*
     * METHOD NAME: AddCarrier
     * DESCRIPTION: Adds one or multiple carriers in the order for its trip(s)
     *
     * RETURN: void
     */
    private void AddCarrier()
    {
        try
        {
            if (OrderData == null)
            {
                MessageBox.Show("Can't add a trip. There are no orders. Refresh to get new orders.", "Error");
                return;
            }

            if (SelectedOrder == null)
            {
                MessageBox.Show("Can't proceed, please pick an order.", "Error");
                return;
            }

            if (SelectedOrder.OrderStatus == OrderStatus.InProgress)
            {
                var addTrip = MessageBoxButton.OKCancel;
                var choice =
                    MessageBox.Show(
                        $"Order# {SelectedOrder.OrderId} is already in progress. Are you sure you want to add another trip?",
                        "Planner", addTrip);

                if (choice == MessageBoxResult.Cancel) return;
            }

            AttachCarrierToOrder(SelectedCarrier);
            AttachCarrierToOrder(AnotherSelectedCarrier);

            RefreshOrdersTab();
        }
        catch
        {
            MessageBox.Show("Adding a carrier to a trip to attach to the order failed", "Failed");
           LoggerModel.Instance.LogError("Adding carrier to a trip for the selected order failed");
        }
    }

    private bool CanAddCarrier()
    {
        return SelectedOrder != null;
    }

    #endregion

    #region Attach carrier to trip method

    private void AttachCarrierToOrder(string? selectedCarrier)
    {
        if (selectedCarrier != null)
        {
            if (SelectedOrder != null)
            {
                var sourceCity = OrderModel?.GetCityById(SelectedOrder.SourceCityId);
                var destinationCity = OrderModel?.GetCityById(SelectedOrder.DestinationCityId);

                if (sourceCity != null)
                {
                    var carrier = PlannerModel?.GetCarrier(selectedCarrier, sourceCity);
                    if (carrier == null)
                    {
                        MessageBox.Show(
                            $"Can't assign {selectedCarrier} as a carrier because it doesn't have the order's origin as a depot city",
                            "Error");
                        return;
                    }

                    double totalCost = 0;
                    if (destinationCity != null)
                    {
                        var kmAndHrs = OrderModel?.GetKmAndHrs(destinationCity, sourceCity);

                        var vanType = SelectedOrder.JobType == JobType.Ltl ? 1 : 0;
                        var jobType = SelectedOrder.VanType == VanType.Reefer ? 1 : 0;

                        if (kmAndHrs != null)
                        {
                            var totalCostArray =
                                OrderModel?.CalculateRate(carrier, kmAndHrs[0], vanType, SelectedOrder.Quantity, jobType);
                            if (totalCostArray != null) totalCost = totalCostArray[0] + totalCostArray[1];
                        }
                    }

                    PlannerModel?.AddTripToOrder(SelectedOrder.OrderId, SelectedOrder, carrier, totalCost);
                }
            }

            if (SelectedOrder != null)
            {
                MessageBox.Show($"Attached one trip to order {SelectedOrder.OrderId} with carrier {selectedCarrier}",
                    "Trip Added");
                LoggerModel.Instance.LogInfo(
                    $"Attached one trip to order {SelectedOrder.OrderId} with carrier {selectedCarrier}");
            }
        }
    }

    #endregion

    #region Call complete order

    /*
     * METHOD NAME: CallCompleteOrder
     * DESCRIPTION: Completes an Order
     *
     * RETURN: void
     */
    private void CallCompleteOrder()
    {
        try
        {
            //Check weather the order is pending if pending dont proceed
            if (SelectedInProgressOrder != null && SelectedInProgressOrder.OrderStatus == OrderStatus.Pending)
            {
                MessageBox.Show(
                    $"Order# {SelectedInProgressOrder.OrderId} is still pending. Attach a trip to complete it");
                return;
            }

            //Calls Order model method that saves the completed order to database
            if (SelectedInProgressOrder != null)
            {
                OrderModel?.CompleteOrder(SelectedInProgressOrder.OrderId, DateTime.Now);
                MessageBox.Show($"Successfully Completed Order# {SelectedInProgressOrder.OrderId}");
                LoggerModel.Instance.LogInfo($"Successfully Completed Order# {SelectedInProgressOrder.OrderId}");
            }

            //Refresh the page
            RefreshActiveOrdersTab();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Completing an Order failed");
            if (SelectedInProgressOrder != null)
                LoggerModel.Instance.LogError(
                    $" Completing Order# {SelectedInProgressOrder.OrderId} failed. {ex.Message}");
        }
    }

    private bool CanCallCompleteOrder()
    {
        return SelectedInProgressOrder != null;
    }

    #endregion
}