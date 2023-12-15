using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using TMS_Project.DataLayer.Model;
using TMS_Project.Helper;
using TMS_Project.Model;

namespace TMS_Project.ViewModel
{
    // Completed
    public class BuyerViewModel : ViewModelBase
    {
        #region Fields
        private DateTime _currentDate;
        private readonly LoggerModel _loggerModel = LoggerModel.Instance;
        public IEnumerable<Contract>? ContractData { get; private set; }
        public ICommand CreateOrderCommand { get; }
        public ICommand ProcessInvoiceCommand { get; }
        private ObservableCollection<JoinedOrder>? _completedOrders;
        private readonly BuyerModel _buyerModel;
        private readonly AdminServices _adminServices;
        public LogInViewModel LogInViewModel { get; private set; } = new();
        private readonly OrderModel _orderModelObject;
        private readonly InvoiceGeneratorModel _invoiceModel;
        private Contract? _selectedContract; 
        #endregion

        #region Properties
        public ObservableCollection<JoinedOrder>? CompletedOrders 
        { 
            get => _completedOrders; 
            set 
            { 
                _completedOrders = value; 
                OnPropertyChanged(nameof(CompletedOrders));
            }
        }
        public DateTime CurrentDate
        {
            get => _currentDate;
            set
            {
                _currentDate = value;
                OnPropertyChanged(nameof(CurrentDate));
            }
        }
        public Contract? SelectedContract
        {
            get => _selectedContract;
            set
            {
                _selectedContract = value;
                OnPropertyChanged(nameof(SelectedContract));
            }
        }

        private JoinedOrder? _selectedOrder;

        public JoinedOrder? SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                _selectedOrder = value;
                OnPropertyChanged(nameof(SelectedOrder));
            }
        }

          #endregion

        #region Constructor

        public BuyerViewModel()
        {
            _buyerModel = new BuyerModel();
            CreateOrderCommand = new RelayCommand(CallCreateOrder);
            ProcessInvoiceCommand = new RelayCommand(CallProcessInvoice);

            CurrentDate = DateTime.Today;
            _orderModelObject = new OrderModel();
            _adminServices = new AdminServices();
            _invoiceModel = new InvoiceGeneratorModel();
            LoadData();
        }

        #endregion

        #region Load Data

        /*
        * METHOD NAME: LoadData
        * DESCRIPTION: Gets the contracts and orders from the DB
        * 
        * RETURN: void
        */
        private void LoadData()
        {
            try
            {
                var loadedContracts = _buyerModel.LoadContracts();
                ContractData = new ObservableCollection<Contract>(loadedContracts);
                CompletedOrders = new ObservableCollection<JoinedOrder>(_adminServices.GetCompletedOrders());
            }
            catch (Exception e)
            {
                _loggerModel.LogException($"Exception while loading data for completed orders and contracts: {e.Message}");
            }
        }

        #endregion

        #region Create Order

        /*
        * METHOD NAME: CallCreateOrder
        * DESCRIPTION: Accepts an order from a customer from the CMP 
        * 
        * RETURN: void
        */
        private void CallCreateOrder()
        {
            try
            {
                // Check if destination is valid, Get the destination city of the contract
                if (SelectedContract?.Destination == null)
                {
                    _loggerModel.LogError("Destination for contract is null");
                    return; // Exit method if the destination is null
                }

                var contractDestCity = _orderModelObject.GetCity(SelectedContract.Destination);
                if (contractDestCity == null)
                {
                    _loggerModel.LogError("Destination city for contract is null");
                    return; // Exit method if the destination city is null
                }

                // Check if origin is null
                if (SelectedContract.Origin == null)
                {
                    _loggerModel.LogError("Origin for contract is null");
                    return; // Exit method if the origin is null
                }

                var contractOriginCity = _orderModelObject.GetCity(SelectedContract.Origin);

                // Check if client name is null
                if (SelectedContract.Client_Name == null)
                {
                    _loggerModel.LogError("Client name for contract is null");
                    return; // Exit method if the client name is null
                }

                // Find customer by name
                var customer = _orderModelObject.FindCustomerByName(SelectedContract.Client_Name);

                // If customer doesn't exist, create customer and order
                if (customer == null)
                {
                    customer = _orderModelObject.CreateCustomer(SelectedContract.Client_Name);
                    if (customer != null)
                        _orderModelObject.CreateOrder(contractDestCity, contractOriginCity, customer.CustomerId,
                            SelectedContract.Job_Type, SelectedContract.Quantity, SelectedContract.Van_Type);
                }
                else // If customer exists, create order only
                {
                    _orderModelObject.CreateOrder(contractDestCity, contractOriginCity, customer.CustomerId,
                        SelectedContract.Job_Type, SelectedContract.Quantity, SelectedContract.Van_Type);
                }

                // Inform user with message box
                MessageBox.Show("Successfully accepted customer. A new order has been created.", "New customer added",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                // Log the exception and display a generic error message to the user
                _loggerModel.LogException($"Error: {ex.Message}");
                MessageBox.Show("An error occurred while processing the request. Please try again later.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Process Invoice

        /*
        * METHOD NAME: CallProcessInvoice
        * DESCRIPTION: Processes an invoice for a completed order
        * 
        * RETURN: void
        */
        private void CallProcessInvoice()
        {
            try
            {
                // Retrieve necessary information from the selected order
                if (SelectedOrder != null)
                {
                    var orderId = SelectedOrder.OrderId;
                    var customerId = SelectedOrder.CustomerId;
                    var tripCost = SelectedOrder.TripCost;
                    var origin = SelectedOrder.Origin;
                    var destination = SelectedOrder.Destination;

                    // Generate invoice
                    var invoice = _invoiceModel.CreateInvoice(orderId, customerId, tripCost, destination, origin);

                    if (invoice != null)
                    {
                        // Show success message
                        MessageBox.Show("Invoice generated successfully.", "Success", MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        // Generate text file for the invoice
                        _invoiceModel.GenerateTxtInvoice(invoice);
                    }
                    else
                    {
                        // Show a message if invoice generation fails
                        MessageBox.Show("Failed to generate invoice.", "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception and display an error message to the user
                _loggerModel.LogException($"Error: {ex.Message}");
                MessageBox.Show("An error occurred while processing the request. Please try again later.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}
