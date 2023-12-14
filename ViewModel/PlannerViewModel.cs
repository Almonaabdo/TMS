using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TMS_Project.DataLayer.Context;
using TMS_Project.DataLayer.Model;
using TMS_Project.Helper;
using TMS_Project.Model;
using TMS_Project.ViewModel;

namespace TMS.ViewModel
{
    public class PlannerViewModel : ViewModelBase
    {
        #region Properties

        public LogInViewModel LogInViewModel { get; set; } = new();

        public ICommand CompleteOrderCommand { get; }

        private Order _selectedOrder;
        public Order SelectedOrder
        {
            get { return _selectedOrder; }
            set
            {
                _selectedOrder = value;
                OnPropertyChanged(nameof(SelectedOrder));
            }
        }

        private string _selectedCarrier;
        public string SelectedCarrier
        {
            get { return _selectedCarrier; }
            set
            {
                _selectedCarrier = value;
                OnPropertyChanged(nameof(SelectedCarrier));
            }
        }

        private string _anotherselectedCarrier;
        public string AnotherSelectedCarrier
        {
            get { return _anotherselectedCarrier; }
            set
            {
                _anotherselectedCarrier = value;
                OnPropertyChanged(nameof(AnotherSelectedCarrier));
            }
        }


        private List<string> _carrierNames;
        public List<string> CarrierNames
        {
            get { return _carrierNames; }
            set
            {
                _carrierNames = value;
                OnPropertyChanged(nameof(CarrierNames));
            }
        }
        public ICommand GetOrdersCommand { get; }
        public ICommand AddCarrierCommand { get; }


        private DataService _dataService { get; }

        private readonly TmsDbContext _TmsDbContext;
        private OrderModel _orderModel { get; }

        private CarrierViewModel _carrierViewModel { get; }
        public ObservableCollection<Order> OrderData { get; private set; } = null!;

        public ObservableCollection<Carrier> CarrierData { get; private set; } = null!;

        private PlannerModel _plannerModel { get; }

        public ObservableCollection<Order> OrderDataInProgress { get; private set; } = null!;

        #endregion

        #region Constructor

        public PlannerViewModel()
        {
            CompleteOrderCommand = new RelayCommand(CallCompleteOrder);
            AddCarrierCommand = new RelayCommand(AddCarrier);
            _TmsDbContext = new TmsDbContext();
            GetOrderTable();
            _dataService = new DataService();
            _orderModel = new OrderModel(_TmsDbContext);
            _carrierViewModel = new CarrierViewModel();
            CarrierData = _carrierViewModel.CarrierData;
            _plannerModel = new PlannerModel();
            GetDistinctCarrierNames();
            GetPendingOrders();
            GetInProgressOrders();


        }

        #endregion

        #region Methods

        public void GetPendingOrders()
        {
            OrderData = new ObservableCollection<Order>(_TmsDbContext.Orders?.Where(order => order.OrderStatus == OrderStatus.Pending).ToList() ?? throw new InvalidOperationException());
        }

        public void GetInProgressOrders()
        {
            OrderDataInProgress = new ObservableCollection<Order>(_TmsDbContext.Orders?.Where(order => order.OrderStatus == OrderStatus.InProgress).ToList() ?? throw new InvalidOperationException());
        }
        public void GetDistinctCarrierNames()
        {

            CarrierNames = _TmsDbContext.Carriers.Select(c => c.CompanyName).Distinct().ToList();

        }

        public void GetOrderTable()
        {
            OnPropertyChanged(nameof(OrderData));
        }

        public void AddCarrier()
        {
            try
            {
                if (SelectedOrder == null)
                {
                    MessageBox.Show($"Can't procede please pick an order");
                    return;
                }

                if (SelectedOrder.OrderStatus == OrderStatus.InProgress)
                {
                    MessageBoxButton addTrip = MessageBoxButton.OKCancel;
                    var choice = MessageBox.Show($"Order# {SelectedOrder.OrderId} is already in progress are you sure you want to add another trip", "Planner", addTrip);


                    if (choice == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                }

                if (SelectedCarrier != null)
                {
                    MessageBox.Show("firstCarrier not empty");
                    string? sourceCity = _orderModel.GetCityById(SelectedOrder.SourceCityId);
                    MessageBox.Show($" Order source city{sourceCity}");
                    var carrier = _plannerModel.GetCarrier(SelectedCarrier, sourceCity);
                    if (carrier == null)
                    {
                        MessageBox.Show("Can't assign this carrier because Carrier doesn't have the order's origin as a depot city");
                    }

                    else
                    {
                        Trip trip = new Trip();

                        trip.OrderId = SelectedOrder.OrderId;
                        trip.Order = SelectedOrder;
                        trip.CarrierId = carrier.CarrierId;
                        
                        
                        MessageBox.Show("Trips done");
                        MessageBox.Show($" order {SelectedOrder.OrderId} with carrier {SelectedCarrier}");
                        _plannerModel.AddTripToOrder(SelectedOrder.OrderId, trip);
                        MessageBox.Show($"Attached one trip to order {SelectedOrder.OrderId} with carrier {SelectedCarrier}");


                    }

                }


                if (AnotherSelectedCarrier != null)
                {
                    MessageBox.Show("second Carrier not empty");
                    string? sourceCity = _orderModel.GetCityById(SelectedOrder.SourceCityId);
                    MessageBox.Show($" Order source city{sourceCity}");
                    var carrier = _plannerModel.GetCarrier(AnotherSelectedCarrier, sourceCity);
                    if (carrier == null)
                    {
                        MessageBox.Show("Can't assign this carrier because Carrier doesn't have the order's origin as a depot city");
                    }

                    else
                    {
                        Trip trip = new Trip();

                        trip.Carrier = carrier;
                        trip.OrderId = SelectedOrder.OrderId;
                        trip.Order = SelectedOrder;
                        trip.TripStatus = TripStatus.Scheduled;

                        _plannerModel.AddTripToOrder(SelectedOrder.OrderId, trip);
                        MessageBox.Show($"Attached one trip to order {SelectedOrder.OrderId} with carrier {AnotherSelectedCarrier}");



                    }

                }
            }



            catch
            {
                MessageBox.Show("Adding a carrier to a trip to attach to the order failed");
                //LoggerModel.LogError("Adding carrier to a trip for the selected order failed");
                SelectedCarrier = "";
                AnotherSelectedCarrier = "";

            }


        }

        public void CallCompleteOrder()
        {
            try
            {
                if (SelectedOrder.OrderStatus == OrderStatus.Pending)
                {
                    MessageBox.Show($"Order# {SelectedOrder.OrderId} is still pending. Attach a trip to complete it");
                    return;
                }
                _orderModel.CompleteOrder(SelectedOrder.OrderId);
                MessageBox.Show($"Successfully Completed Order# {SelectedOrder.OrderId}");
                GetOrderTable();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        #endregion
    }
}
