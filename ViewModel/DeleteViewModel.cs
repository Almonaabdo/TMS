using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using TMS_Project.DataLayer.Model;
using TMS_Project.Helper;
using TMS_Project.Model;

namespace TMS_Project.ViewModel
{
    public class DeleteViewModel : ViewModelBase
    {
        private string _selectedDeleteOption;
        private List<string> _deleteOptions;
        private ObservableCollection<object> _selectedTableData;
        private readonly DataService _dataService;
        private object _selectedRows;

        public object SelectedRows
        {
            get => _selectedRows;
            set
            {
                _selectedRows = value;
                OnPropertyChanged(nameof(SelectedRows));
            }
        }
        public List<string> DeleteOptions
        {
            get => _deleteOptions;
            set
            {
                _deleteOptions = value;
                OnPropertyChanged(nameof(DeleteOptions));
            }
        }

        public string SelectedDeleteOption
        {
            get => _selectedDeleteOption;
            set
            {
                _selectedDeleteOption = value;
                LoadTableData();
                OnPropertyChanged(nameof(SelectedDeleteOption));
            }
        }

        public ObservableCollection<object> SelectedTableData
        {
            get => _selectedTableData;
            set
            {
                _selectedTableData = value;
                OnPropertyChanged(nameof(SelectedTableData));
            }
        }

        public RelayCommand DeleteCommand { get; }

        public DeleteViewModel()
        {
            _dataService = new DataService();
            DeleteOptions = new List<string> { "Route", "Rate", "Carrier" }; // Add other options as needed
            DeleteCommand = new RelayCommand(DeleteData);
        }

        private void LoadTableData()
        {
            switch (SelectedDeleteOption)
            {
                case "Route":
                    SelectedTableData = new ObservableCollection<object>(_dataService.RetrieveTable<Route>()?.Cast<object>() ?? throw new InvalidOperationException());
                    break;
                case "Rate":
                    SelectedTableData = new ObservableCollection<object>(_dataService.RetrieveTable<Rate>()?.Cast<object>() ?? throw new InvalidOperationException());
                    break;
                case "Carrier":
                    SelectedTableData = new ObservableCollection<object>(_dataService.RetrieveTable<Carrier>()?.Cast<object>() ?? throw new InvalidOperationException());
                    break;
                default:
                    MessageBox.Show("Please select a valid option.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
        }

        private void DeleteData()
        {
            try
            {
                switch (SelectedDeleteOption)
                {
                    case "Route":
                        DeleteRows<Route>();
                        break;
                    case "Rate":
                        DeleteRows<Rate>();
                        break;
                    case "Carrier":
                        DeleteRows<Carrier>();
                        break;
                    default:
                        MessageBox.Show("Please select a valid option.", "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                MessageBox.Show("Error deleting data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteRows<T>() where T : class
        {
            try
            {
                if (SelectedRows != null)
                {
                    MessageBoxResult result = MessageBox.Show("Are you sure you want to delete the selected row?",
                        "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        _dataService.DeleteData(SelectedRows as T);

                        // Refresh the data after deletion
                        LoadTableData();

                        MessageBox.Show("Selected row deleted successfully!");
                    }
                }
                else
                {
                    MessageBox.Show("Please select a row to delete.", "Warning", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                MessageBox.Show("Error deleting row", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
