using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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

namespace Billing
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        ConnectionClass cc = new ConnectionClass();
        private int userExist;
        public static string Mac;

        public LoginWindow()
        {
            InitializeComponent();
            btnSubmit.Focus();
            //GetMacAddress();
        }

        private void GetMacAddress()
        {
            string mac = "";
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {

                if (nic.OperationalStatus == OperationalStatus.Up && (!nic.Description.Contains("Virtual") && !nic.Description.Contains("Pseudo")))
                {
                    if (nic.GetPhysicalAddress().ToString() != "")
                    {
                        mac = nic.GetPhysicalAddress().ToString();
                    }
                }
            }
            string result = mac.Substring(0, 3);
            Mac = result;
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            cc.OpenConnection();
            try
            {
                GetMacAddress();
                if (cc.UserAuthentication("select Count(1) from LoginTable where Username=@username And Password=@password", "@username", txtUsername.Text, "@password", txtPassword.Password+Mac, userExist) > 0)
                {
                    MainWindow mw = new MainWindow();
                    mw.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Incorrect username/password");
                    txtPassword.Clear();
                    txtUsername.Focus();
                }
            }
            catch(Exception)
            {
                MessageBox.Show("Something went wrong");
            }
            cc.CloseConnection();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }
    }
}
