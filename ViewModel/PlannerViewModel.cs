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
    private CarrierModel CarrierModelObject { get; }
    private readonly OrderModel OrderModelObject;

    private readonly TmsDbContext _TmsDbContext;

    public ObservableCollection<Order> OrderData { get; private set; } = null!;


    public PlannerViewModel()
    {
        CompleteOrderCommand = new RelayCommand(CallCompleteOrder);
        _TmsDbContext = new TmsDbContext();
        CarrierModelObject = new CarrierModel(_TmsDbContext);
        OrderModelObject = new OrderModel();
        GetOrderTable();
    }



    public void GetOrderTable()
    {
        OrderData = new ObservableCollection<Order>(OrderModelObject.LoadTable<Order>() ?? throw new InvalidOperationException());

        OnPropertyChanged(nameof(OrderData));
    }



    public void CallCompleteOrder()
    {
        try
        {
            OrderModelObject.CompleteOrder(SelectedOrder);
            MessageBox.Show("Sucessfully Completed Order");
            GetOrderTable();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
}