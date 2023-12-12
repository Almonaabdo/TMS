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
            GetNotCompletedOrders();



        }

        #endregion

        #region Methods

        public List<Order> GetNotCompletedOrders()
        {
            var notCompletedOrders = new List<Order>();
            OrderData = new ObservableCollection<Order>(_TmsDbContext.Orders?.Where(order => order.OrderStatus != OrderStatus.Completed).ToList() ?? throw new InvalidOperationException());
            notCompletedOrders = _TmsDbContext.Orders?.Where(order => order.OrderStatus != OrderStatus.Completed).ToList() ?? throw new InvalidOperationException();
            return notCompletedOrders;

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
                if (AnotherSelectedCarrier != null)
                {
                    var carrier = _plannerModel.GetCarrier(AnotherSelectedCarrier, SelectedOrder.SourceCity.ToString());
                    if (carrier == null)
                    {
                        MessageBox.Show("Can't assign this carrier for the order because it doesn't offer the destination that the order wants");
                    }

                    else
                    {
                        Trip trip = new Trip();

                        trip.Carrier = carrier;
                        trip.OrderId = SelectedOrder.OrderId;
                        trip.Order = SelectedOrder;
                        trip.TripStatus = TripStatus.Scheduled;

                        _plannerModel.AddTripToOrder(SelectedOrder.OrderId, trip);

                        GetOrderTable();
                        AnotherSelectedCarrier = "";
                    }
                }

                if (SelectedCarrier != null)
                {
                    var carrier = _plannerModel.GetCarrier(SelectedCarrier, SelectedOrder.DestinationCity.ToString());
                    if (carrier == null)
                    {
                        MessageBox.Show("Can't assign this carrier for the order because it doesn't offer the destination that the order wants");
                    }

                    else
                    {
                        Trip trip = new Trip();

                        trip.OrderId = SelectedOrder.OrderId;
                        trip.Order = SelectedOrder;
                        trip.Carrier = carrier;

                        _plannerModel.AddTripToOrder(SelectedOrder.OrderId, trip);

                        GetOrderTable();
                        SelectedCarrier = "";
                    }

                }
            }

            catch
            {
                MessageBox.Show("Adding a carrier to a trip to attach to the order failed");
                LoggerModel.LogError("Adding carrier to a trip for the selected order failed");
                SelectedCarrier = "";
                AnotherSelectedCarrier = "";

            }


        }


        public void CallCompleteOrder()
        {
            try
            {
                // _orderModel.CompleteOrder();
                MessageBox.Show("Successfully Completed Order");
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
