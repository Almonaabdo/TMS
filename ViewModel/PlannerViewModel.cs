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

        private Carrier _selectedCarrier;
        public Carrier SelectedCarrier
        {
            get { return _selectedCarrier; }
            set
            {
                _selectedCarrier = value;
                OnPropertyChanged(nameof(SelectedCarrier));
            }
        }

        private Carrier _anotherselectedCarrier;
        public Carrier AnotherSelectedCarrier
        {
            get { return _anotherselectedCarrier; }
            set
            {
                _anotherselectedCarrier = value;
                OnPropertyChanged(nameof(AnotherSelectedCarrier));
            }
        }

        public ICommand GetOrdersCommand { get; }
        private DataService _dataService { get; }

        private readonly TmsDbContext _TmsDbContext;
        private OrderModel _orderModel { get; }
        public ObservableCollection<Order> OrderData { get; private set; } = null!;

        #endregion

        #region Constructor

        public PlannerViewModel()
        {
            CompleteOrderCommand = new RelayCommand(CallCompleteOrder);
            _TmsDbContext = new TmsDbContext();
            GetOrderTable();
            _dataService = new DataService();
            _orderModel = new OrderModel(_TmsDbContext);

        }

        #endregion

        #region Methods

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
                    Trip trip = new Trip();

                    trip.Carrier = AnotherSelectedCarrier;
                    trip.OrderId = SelectedOrder.OrderId;
                    trip.Order = SelectedOrder;
                    trip.TripStatus = TripStatus.Scheduled;
                    SelectedOrder.Trips.Add(trip);

                    GetOrderTable();
                    LoggerModel.LogInfo("Succesfully added a carrier to a trip for the selected order");

                }

                if (SelectedCarrier != null)
                {
                    Trip trip = new Trip();

                    trip.OrderId = SelectedOrder.OrderId;
                    trip.Order = SelectedOrder;
                    trip.Carrier = SelectedCarrier;
                    trip.TripStatus = TripStatus.Scheduled;
                    SelectedOrder.Trips.Add(trip);

                    GetOrderTable();
                    LoggerModel.LogInfo("Succesfully added a carrier to a trip for the selected order");
                }



            }

            catch
            {
                LoggerModel.LogInfo("Adding carrier to a trip for the selected order failed");
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
