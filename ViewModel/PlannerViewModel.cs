using System;
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




namespace TMS.ViewModel;
public class PlannerViewModel : INotifyPropertyChanged
{
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

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


    public ICommand GetOrdersCommand { get; }
    private DataService _dataService { get; }
    
    private readonly TmsDbContext _TmsDbContext;
    private OrderModel _orderModel { get;}
    public ObservableCollection<Order> OrderData { get; private set; } = null!;


    public PlannerViewModel()
    {
        CompleteOrderCommand = new RelayCommand(CallCompleteOrder);
        _TmsDbContext = new TmsDbContext();
        GetOrderTable();
    }



    public void GetOrderTable()
    {
        OrderData = new ObservableCollection<Order>(_dataService.RetrieveTable<Order>() ?? throw new InvalidOperationException());

        OnPropertyChanged(nameof(OrderData));
    }



    public void CallCompleteOrder()
    {
        try
        {
           // _orderModel.CompleteOrder();
            MessageBox.Show("Sucessfully Completed Order");
            GetOrderTable();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
}