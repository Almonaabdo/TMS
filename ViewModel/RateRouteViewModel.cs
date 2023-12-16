using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Input;
using Devart.Common;
using TMS_Project.DataLayer.Model;
using TMS_Project.Helper;
using TMS_Project.Model;

namespace TMS_Project.ViewModel;

public class RateRouteViewModel: ViewModelBase
{
    private readonly AdminServices _adminServices;


    public RateRouteViewModel()
    {
        _adminServices = new AdminServices();
        SaveRateCommand = new RelayCommand(SaveRateChanges);
        SaveRouteCommand = new RelayCommand(SaveRouteChanges);
        LoadData();
    }

    public ObservableCollection<Rate> RateData { get; private set; } = null!;
    public ObservableCollection<JoinedRouteTable> RouteData { get; private set; } = null!;

    #region Commands

    public ICommand SaveRateCommand { get; }
    public ICommand SaveRouteCommand { get; }

    #endregion

    #region Methods

    /*
    * METHOD NAME: LoadData
    * DESCRIPTION: Loads carrier data 
    * 
    * RETURN: void
    */
    private void LoadData()
    {
        // Load Carrier data
        RateData = new ObservableCollection<Rate>(_adminServices.RetrieveTable<Rate>() ?? throw new InvalidOperationException());
        RouteData = new ObservableCollection<JoinedRouteTable>(_adminServices.GetJoinedRouteData() ?? throw new InvalidOperationException());
        OnPropertyChanged(nameof(RateData));
    }


    /*
    * METHOD NAME: SaveRateChanges
    * DESCRIPTION: Saves the changes made in Rate table
    * 
    * RETURN: void
    */
    private void SaveRateChanges()
    {
        try
        {
            List<Rate> updatedRate = new List<Rate>(RateData);

            foreach (var rate in updatedRate)
            {
                _adminServices.SaveChanges(rate);
            }

            MessageBox.Show("Changes saved successfully!","Database operation", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception e)
        {
            LoggerModel.Instance.LogException($"{e.Message}");
        }
    }


    /*
    * METHOD NAME: SaveRouteChanges
    * DESCRIPTION: Saves the changes made in Route table
    * 
    * RETURN: void
    */
    private void SaveRouteChanges()
    {
        try
        {
            List<JoinedRouteTable> updatedRoute = new List<JoinedRouteTable>(RouteData);
            if (_adminServices != null)
            {
                foreach (var routeData in updatedRoute)
                {
                    _adminServices.SaveChanges(routeData);
                }
            }
           

            MessageBox.Show("Changes saved successfully!", "Database operation", MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception e)
        {
           LoggerModel.Instance.LogException($"{e.Message}");
        }
    }

    #endregion
}