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
using System.Windows.Threading;
using TMS_Project.ViewModel;

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
         //   DispatcherTimer LiveTime = new DispatcherTimer();

         //   LiveTime.Interval = TimeSpan.FromSeconds(1);
//LiveTime.Tick += timer_tick;
          //  LiveTime.Start();
        }

       // void timer_tick(object sender, EventArgs e)
       // {
         //   LiveTimeLabel.Content = DateTime.Now.ToString("MM-dd-yyyy hh:mm:ss tt");
      //  }
    }
}
