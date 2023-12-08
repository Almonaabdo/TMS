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
    // CarrierViewModel: ViewModel for managing Carrier-related functionality
    public class CarrierViewModel : ViewModelBase
    {
        #region Fields
        private readonly DataService _dataService;
        #endregion

        #region Properties

        // Collection for storing Carrier data
        public ObservableCollection<Carrier> CarrierData { get; private set; } = null!;
        // List for storing Contract data
        public List<Contract> ContractsData { get; private set; } = null!;

        #endregion

        #region Carrier Properties

        // Properties for Carrier details
        private string _companyName = null!;
        private int _ftla;
        private int _ltla;
        private double _ftlaRate;
        private double _ltlaRate;
        private double _reefCharge;
        private string _depotCity = null!;
        private Carrier _selectedCarrier;

        // Carrier properties with OnPropertyChanged
        public string CompanyName
        {
            get => _companyName;
            set
            {
                _companyName = value;
                OnPropertyChanged(nameof(CompanyName));
            }
        }

        public int Ftla
        {
            get => _ftla;
            set
            {
                _ftla = value;
                OnPropertyChanged(nameof(Ftla));
            }
        }

        public int Ltla
        {
            get => _ltla;
            set
            {
                _ltla = value;
                OnPropertyChanged(nameof(Ltla));
            }
        }

        public double FtlaRate
        {
            get => _ftlaRate;
            set
            {
                _ftlaRate = value;
                OnPropertyChanged(nameof(FtlaRate));
            }
        }

        public double LtlaRate
        {
            get => _ltlaRate;
            set
            {
                _ltlaRate = value;
                OnPropertyChanged(nameof(LtlaRate));
            }
        }

        public double ReefCharge
        {
            get => _reefCharge;
            set
            {
                _reefCharge = value;
                OnPropertyChanged(nameof(ReefCharge));
            }
        }

        public string DepotCity
        {
            get => _depotCity;
            set
            {
                _depotCity = value;
                OnPropertyChanged(nameof(DepotCity));
            }
        }

        public Carrier SelectedCarrier
        {
            get => _selectedCarrier;
            set
            {
                _selectedCarrier = value;
                OnPropertyChanged(nameof(SelectedCarrier));
            }
        }

        #endregion

        #region Commands

        // Command for saving Carrier changes
        public ICommand SaveCarrierCommand { get; }
        public ICommand CreateCarrierCommand { get; set; }
        public ICommand DeleteCarrierCommand { get; }

        #endregion

        #region Constructor

        // Constructor for initializing necessary commands, methods, and variables
        public CarrierViewModel()
        {
            _dataService = new DataService();
            SaveCarrierCommand = new RelayCommand(SaveCarrierChanges);
            CreateCarrierCommand = new RelayCommand(CreateCarrier);
            DeleteCarrierCommand = new RelayCommand(DeleteCarrier);
            LoadData();
        }

        private void DeleteCarrier()
        {
            try
            {
                if (SelectedCarrier != null)
                {
                    MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this carrier?",
                        "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        _dataService.DeleteData(SelectedCarrier);
                        CarrierData.Remove(SelectedCarrier);
                        SelectedCarrier = null!;
                        MessageBox.Show("Carrier deleted successfully!");
                    }
                }
                else
                {
                    MessageBox.Show("Please select a carrier to delete.", "Warning", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                MessageBox.Show("Error deleting carrier", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Methods

        // Method for creating a new Carrier
        private void CreateCarrier()
        {
            try
            {
                // Check if any required fields are empty
                if (string.IsNullOrEmpty(CompanyName) || string.IsNullOrEmpty(DepotCity) || Ftla == 0 || Ltla == 0 || FtlaRate == 0 || LtlaRate == 0 || ReefCharge == 0)
                {
                    MessageBox.Show("Please fill in all required fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; // Exit the method if any required field is empty
                }
                _dataService.CreateCarrier(CompanyName, DepotCity, Ftla, Ltla, FtlaRate, LtlaRate, ReefCharge);
                LoadData();
                CompanyName = String.Empty;
                FtlaRate = 0;
                LtlaRate = 0;
                Ftla = 0;
                Ltla = 0;
                DepotCity = string.Empty;
                ReefCharge = 0;
                MessageBox.Show("Successfully Added new carrier.", "Successful Operation", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show("Carrier already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LoggerModel.LogException("Exception adding new carrier.");
            }
        }

        // Method for loading data
        private void LoadData()
        {
            // Load Carrier data
            CarrierData = new ObservableCollection<Carrier>(_dataService.RetrieveTable<Carrier>() ?? throw new InvalidOperationException());
            OnPropertyChanged(nameof(CarrierData));
        }

        // Method for saving Carrier changes
        private void SaveCarrierChanges()
        {
            try
            {
                List<Carrier> updatedCarriers = new List<Carrier>(CarrierData);

                foreach (var carrier in updatedCarriers)
                {
                    _dataService.SaveChanges(carrier);
                }

                MessageBox.Show("Changes saved successfully!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion
    }
}
