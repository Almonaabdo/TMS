using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using TMS_Project.Helper;
using TMS_Project.Model;

namespace TMS_Project.ViewModel
{
    public class AddViewModel : ViewModelBase
    {
        #region Combobox fields
        private string _selectedAddOptions;
        private List<string> _addOptions;
        private ObservableCollection<object> _selectedTable;
        #endregion

        #region Combobox
        public List<string> AddOptions
        {
            get => _addOptions;
            set
            {
                _addOptions = value;
                OnPropertyChanged(nameof(AddOptions));
            }
        }

        public string SelectedAddOptions
        {
            get => _selectedAddOptions;
            set
            {
                _selectedAddOptions = value;
                OnPropertyChanged(nameof(SelectedAddOptions));
            }
        }

        public ObservableCollection<object> SelectedTable
        {
            get => _selectedTable;
            set
            {
                _selectedTable = value;
                OnPropertyChanged(nameof(SelectedTable));
            }
        }
        #endregion




        public AddViewModel()
        {
            _addOptions = new List<string> { "Route", "Rate", "Carrier" };
        }

       
    }
}
