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
        private readonly LoggerModel _loggerModel = LoggerModel.Instance;

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
            LoadData();
        }



        #endregion

        #region Methods

        /*
        * METHOD NAME: CreateCarrier
        * DESCRIPTION: Creates a new Carrier
        * 
        * RETURN: void
        */
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
                _loggerModel.LogException("Exception adding new carrier.");
            }
        }


        /*
        * METHOD NAME: LoadData
        * DESCRIPTION: Loads data
        * 
        * RETURN: void
        */
        private void LoadData()
        {
            // Load Carrier data
            CarrierData = new ObservableCollection<Carrier>(_dataService.RetrieveTable<Carrier>() ?? throw new InvalidOperationException());
            OnPropertyChanged(nameof(CarrierData));
        }


        /*
        * METHOD NAME: SaveCarrierChanges
        * DESCRIPTION: Saves the changes done in the Carrier
        * 
        * RETURN: void
        */
        private void SaveCarrierChanges()
        {
            try
            {
                var updatedCarriers = new List<Carrier>(CarrierData);

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
