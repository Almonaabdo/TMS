using DataLayer.Model;
using NLog.Fluent;
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
    /// Interaction logic for LogInView.xaml
    /// </summary>
    public partial class LogInView : Window
    {
        /* CLASS COMMENT
         * Name		: public class Login
         * Purpose	: Use for binding of the error message. Has one private attribute and accessor.
         */
        public class Login
        {
            private string error;

            public string Error
            {
                get { return error; }
                set { error = value; }
            }

        }

        public LogInView()
        {
            InitializeComponent();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string pass = PASS.Password;
            string user = USER.Text;

            //shows the buyer window if successful login
            if (pass == "buyer" && user == "buyer")
            {
                BuyerView buyer = new BuyerView();
                buyer.Show();
                this.Close();
            }
            //if password is wrong/blank
            else if (pass != "buyer" && user == "buyer" || pass != null && user == "buyer")
            {
                Login testPass = new Login { Error = "Password is incorrect!" };
                this.DataContext = testPass;

            }
            //if username is wrong/blank
            else if (user != "buyer" && pass == "buyer" || pass == "buyer" && user == null)
            {
                Login testUser = new Login { Error = "Username is incorrect!" };
                this.DataContext = testUser;

            }
            //if both are missing
            else
            {
                Login error = new Login { Error = "Username and Password is incorrect!" };
                this.DataContext = error;
            }

        }


    }//END LOGINVIEW
}//END TMS.PROJECT.VIEW
