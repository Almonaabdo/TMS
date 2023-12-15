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

namespace TMS_Project.ViewModel
{
    public class PlannerViewModel : ViewModelBase
    {
        #region Properties

        private DateTime _currentDate;
        public DateTime CurrentDate
        {
            get => _currentDate;
            set
            {
                _currentDate = value;
                OnPropertyChanged(nameof(CurrentDate));

            }
        }

        private readonly LoggerModel _loggerModel = LoggerModel.Instance;

        public LogInViewModel LogInViewModel { get; set; } = new();

        public ICommand CompleteOrderCommand { get; }

        private Order _selectedOrder;
        public Order SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                _selectedOrder = value;
                OnPropertyChanged(nameof(SelectedOrder));
            }
        }

        private Order _selectedInProgressOrder;
        public Order SelectedInProgressOrder
        {
            get => _selectedInProgressOrder;
            set
            {
                _selectedInProgressOrder = value;
                OnPropertyChanged(nameof(SelectedInProgressOrder));
            }
        }

        private string _selectedCarrier;
        public string SelectedCarrier
        {
            get => _selectedCarrier;
            set
            {
                _selectedCarrier = value;
                OnPropertyChanged(nameof(SelectedCarrier));
            }
        }

        private string _anotherselectedCarrier;
        public string AnotherSelectedCarrier
        {
            get => _anotherselectedCarrier;
            set
            {
                _anotherselectedCarrier = value;
                OnPropertyChanged(nameof(AnotherSelectedCarrier));
            }
        }


        private List<string> _carrierNames;
        public List<string?> CarrierNames
        {
            get => _carrierNames;
            set
            {
                _carrierNames = value;
                OnPropertyChanged(nameof(CarrierNames));
            }
        }
        public ICommand GetOrdersCommand { get; }
        public ICommand AddCarrierCommand { get; }

        public ICommand OrdersTabCommand { get; }

        public ICommand ActiveOrdersTabCommand { get; }

        public ICommand TwoWeeksTimeCommand { get; }

        public ICommand AllTimeCommand { get; }

        private DataService DataService { get; }

        private readonly TmsDbContext _tmsDbContext = DbContextSingleton.Instance;
        private OrderModel OrderModel { get; }

        private CarrierViewModel CarrierViewModel { get; }
        public ObservableCollection<Order> OrderData { get; private set; } = null!;
        public ObservableCollection<Invoice> Invoices { get; private set; } = null!;

        public ObservableCollection<Carrier> CarrierData { get; private set; }

        private PlannerModel PlannerModel { get; }

        public ObservableCollection<Order> OrderDataInProgress { get; private set; } = null!;

        #endregion
        
        #region Constructor

        public PlannerViewModel()
        {
            CompleteOrderCommand = new RelayCommand(CallCompleteOrder);
            AddCarrierCommand = new RelayCommand(AddCarrier);
            ActiveOrdersTabCommand = new RelayCommand(RefreshActiveOrdersTab);
            OrdersTabCommand = new RelayCommand(RefreshOrdersTab);
            _currentDate = DateTime.Today;
            GetOrderTable();
            DataService = new DataService();
            OrderModel = new OrderModel();
            CarrierViewModel = new CarrierViewModel();
            CarrierData = CarrierViewModel.CarrierData;
            PlannerModel = new PlannerModel();
            GetDistinctCarrierNames();
            GetPendingOrders();
            GetInProgressOrders();
            GetAllTimeInvoices();
        }

        #endregion

        #region Methods


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
            OrderData = new ObservableCollection<Order>(_tmsDbContext.Orders?.Where(order => order.OrderStatus == OrderStatus.Pending).ToList() ?? throw new InvalidOperationException());
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
            Invoices = new ObservableCollection<Invoice>(_tmsDbContext.Invoice?.ToList() ?? throw new InvalidOperationException());
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
            OrderDataInProgress = new ObservableCollection<Order>(_tmsDbContext.Orders?.Where(order => order.OrderStatus == OrderStatus.InProgress).ToList() ?? throw new InvalidOperationException());
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
            if (_tmsDbContext.Carriers != null)
                CarrierNames = _tmsDbContext.Carriers.Select(c => c.CompanyName).Distinct().ToList();
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
                //Check if an order is picked
                if (SelectedOrder == null)
                {
                    MessageBox.Show($"Can't proceed please pick an order");
                    return;
                }

                //Can't pick order in progress
                if (SelectedOrder.OrderStatus == OrderStatus.InProgress)
                {
                    MessageBoxButton addTrip = MessageBoxButton.OKCancel;
                    var choice = MessageBox.Show($"Order# {SelectedOrder.OrderId} is already in progress are you sure you want to add another trip", "Planner", addTrip);


                    if (choice == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                }

                //If first carrier not null do the following
                if (SelectedCarrier != null)
                {
                    //Get the source and destination by calling the order model method
                    string? sourceCity = OrderModel.GetCityById(SelectedOrder.SourceCityId);
                    string? destinationCity = OrderModel.GetCityById(SelectedOrder.DestinationCityId);

                    //Gets the carrier and checks weather the carrier picked is allowed
                    var carrier = PlannerModel.GetCarrier(SelectedCarrier, sourceCity);
                    if (carrier == null)
                    {
                        MessageBox.Show($"Can't assign {SelectedCarrier} as a carrier because it doesn't have the order's origin as a depot city", "Error");
                    }

                    else
                    {
                        double totalCost = 0;
                        double[] kmAndHrs = new double[2];
                        //Create new trip for the order
                        Trip trip = new Trip
                        {
                            OrderId = SelectedOrder.OrderId,
                            Order = SelectedOrder,
                            Carrier = carrier,
                            CarrierId = carrier.CarrierId
                        };

                        //Gets the total Km and hours of the trip
                        kmAndHrs = OrderModel.GetKmAndHrs(destinationCity, sourceCity);

                        //Parsing for job type and van type
                        int vanType = 0;
                        int jobType = 0;
                        if (SelectedOrder.JobType == JobType.Ltl)
                        {
                            jobType = 1;
                        }

                        if (SelectedOrder.VanType == VanType.Reefer)
                        {
                            vanType = 1;
                        }

                        //Calculates the total cost for the trip
                        double[] totalCostArray = new double[2];
                        totalCostArray = OrderModel.CalculateRate(carrier, kmAndHrs[0], vanType, SelectedOrder.Quantity, jobType);
                        totalCost = totalCostArray[0] + totalCostArray[1];

                        //Store Total cost
                        trip.TripCost = totalCost;

                        //successful adding of trip to an order
                        PlannerModel.AddTripToOrder(SelectedOrder.OrderId, trip);
                        MessageBox.Show($"Attached one trip to order {SelectedOrder.OrderId} with carrier {SelectedCarrier}");
                        _loggerModel.LogInfo($"Attached one trip to order {SelectedOrder.OrderId} with carrier {AnotherSelectedCarrier}");
                        RefreshOrdersTab();

                    }

                }
                //Check if an order is picked
                if (SelectedOrder == null)
                {
                    MessageBox.Show($"Can't proceed please pick an order");
                    return;
                }

                //Can't pick order in progress
                if (SelectedOrder.OrderStatus == OrderStatus.InProgress)
                {
                    MessageBoxButton addTrip = MessageBoxButton.OKCancel;
                    var choice = MessageBox.Show($"Order# {SelectedOrder.OrderId} is already in progress are you sure you want to add another trip", "Planner", addTrip);


                    if (choice == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                }

                //If second carrier not null do the following
                if (AnotherSelectedCarrier != null)
                {
                    //Get the source and destination by calling the order model method
                    string? sourceCity = OrderModel.GetCityById(SelectedOrder.SourceCityId);
                    string? destinationCity = OrderModel.GetCityById(SelectedOrder.DestinationCityId);

                    //Gets the carrier and checks weather the carrier picked is allowed
                    var carrier = PlannerModel.GetCarrier(AnotherSelectedCarrier, sourceCity);
                    if (carrier == null)
                    {
                        MessageBox.Show($"Can't assign {AnotherSelectedCarrier} as a carrier because it doesn't have the order's origin as a depot city", "Error");
                    }

                    else
                    {
                        double totalCost = 0;
                        double[] kmAndHrs = new double[2];
                        //Create new trip for the order
                        Trip trip = new Trip
                        {
                            OrderId = SelectedOrder.OrderId,
                            Order = SelectedOrder,
                            Carrier = carrier,
                            CarrierId = carrier.CarrierId
                        };

                        //Gets the total Km and hours of the trip
                        kmAndHrs = OrderModel.GetKmAndHrs(destinationCity, sourceCity);

                        //Parsing for job type and van type
                        int vanType = 0;
                        int jobType = 0;
                        if (SelectedOrder.JobType == JobType.Ltl)
                        {
                            jobType = 1;
                        }

                        if (SelectedOrder.VanType == VanType.Reefer)
                        {
                            vanType = 1;
                        }

                        //Calculates the total cost for the trip
                        var totalCostArray = OrderModel.CalculateRate(carrier, kmAndHrs[0], vanType, SelectedOrder.Quantity, jobType);
                        totalCost = totalCostArray[0] + totalCostArray[1];

                        //Store Total cost
                        trip.TripCost = totalCost;

                        //successful adding of trip to an order
                        PlannerModel.AddTripToOrder(SelectedOrder.OrderId, trip);
                        MessageBox.Show($"Attached one trip to order {SelectedOrder.OrderId} with carrier {SelectedCarrier}");
                        _loggerModel.LogInfo($"Attached one trip to order {SelectedOrder.OrderId} with carrier {AnotherSelectedCarrier}");
                        RefreshOrdersTab();

                    }

                }
            }
            catch
            {
                //failed adding of trip to an order
                MessageBox.Show("Adding a carrier to a trip to attach to the order failed", "Failed");
                _loggerModel.LogError("Adding carrier to a trip for the selected order failed");

            }

        }


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
                if (SelectedInProgressOrder.OrderStatus == OrderStatus.Pending)
                {
                    MessageBox.Show($"Order# {SelectedInProgressOrder.OrderId} is still pending. Attach a trip to complete it");
                    return;
                }
                //Calls Order model method that saves the completed order to database
                OrderModel.CompleteOrder(SelectedInProgressOrder.OrderId);
                MessageBox.Show($"Successfully Completed Order# {SelectedInProgressOrder.OrderId}");
                _loggerModel.LogInfo($"Successfully Completed Order# {SelectedInProgressOrder.OrderId}");
                //Refresh the page
                RefreshActiveOrdersTab();
            }
            catch (Exception ex)
            {

                MessageBox.Show("Completing an Order failed");
                _loggerModel.LogError($" Completing Order# {SelectedInProgressOrder.OrderId} failed. {ex.Message}");
            }
        }
        #endregion
    }
}
