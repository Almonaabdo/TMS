using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Input;
using TMS_Project.DataLayer.Model;
using TMS_Project.Helper;
using TMS_Project.Model;

namespace TMS_Project.ViewModel;

public class RateRouteViewModel: ViewModelBase
{
    private readonly DataService _dataService;


    public RateRouteViewModel()
    {
        _dataService = new DataService();
        SaveRateCommand = new RelayCommand(SaveRateChanges);
        SaveRouteCommand = new RelayCommand(SaveRouteChanges);
        LoadData();
    }
    public ObservableCollection<Rate> RateData { get; private set; } = null!;
    public ObservableCollection<Route> JoinedRouteData { get; private set; } = null!;
    public ObservableCollection<JoinedRouteTable> RouteData { get; private set; } = null!;

    public ICommand SaveRateCommand { get; }
    public ICommand SaveRouteCommand { get; }

    private void LoadData()
    {
        // Load Carrier data
        RateData = new ObservableCollection<Rate>(_dataService.RetrieveTable<Rate>() ?? throw new InvalidOperationException());
        RouteData = new ObservableCollection<JoinedRouteTable>(_dataService.GetJoinedRouteData() ?? throw new InvalidOperationException());
        OnPropertyChanged(nameof(RateData));
    }

    private void SaveRateChanges()
    {
        try
        {
            List<Rate> updatedRate = new List<Rate>(RateData);

            foreach (var rate in updatedRate)
            {
                _dataService.SaveChanges(rate);
            }

            MessageBox.Show("Changes saved successfully!","Database operation", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }


    private void SaveRouteChanges()
    {
        try
        {
            List<JoinedRouteTable> updatedRoute = new List<JoinedRouteTable>(RouteData);

            foreach (var routeData in updatedRoute)
            {
                _dataService.SaveChanges(routeData);
            }

            MessageBox.Show("Changes saved successfully!", "Database operation", MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}