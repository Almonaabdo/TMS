using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TMS_Project.View
{
    /// <summary>
    /// Interaction logic for AdminView.xaml
    /// </summary>
    public partial class AdminView : Window
    {
        public AdminView()
        {
            InitializeComponent();
        }

        ////logs out the user
        //private void Button_Logout(object sender, RoutedEventArgs e)
        //{
        //    LogInView login = new LogInView();
        //    login.Show();
        //    this.Close();

        //}

        //data grid for carrier data database
        private void CarrierData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CarrierData.AutoGenerateColumns = true;
        }

        //data grid for route table database

        private void RouteTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RouteTable.AutoGenerateColumns = true;
        }

        //data grid for rate/fee table database

        private void RateFeeTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RateFeeTable.AutoGenerateColumns = true;

        }
    }
}
