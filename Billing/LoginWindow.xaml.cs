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
        public static string Mac, passwordCheck, username,password;

        public LoginWindow()
        {
            InitializeComponent();
            btnSubmit.Focus();
        }

        private void GetMacAddress()
        {
            string mac = "";
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                if (mac == "")
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    mac = adapter.GetPhysicalAddress().ToString();
                }
            }

            string result = mac.Substring(1, 1) + mac.Substring(10, 1);
            Mac = result;
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            cc.OpenConnection();
                string userPassword = txtPassword.Password;
                string exactPassword = userPassword.Substring(0, 4);
                GetMacAddress();
                if (cc.UserAuthentication("select Count(1) from LoginTable where Username=@username And Password=@password", "@username", txtUsername.Text, "@password", exactPassword, userExist) > 0)
                {
                    cc.DataReader("select Password from LoginTable where Username='" + txtUsername.Text + "'");
                    while (cc.reader.Read())
                    {
                        passwordCheck = cc.reader["Password"].ToString();
                    }
                    cc.CloseReader();
                    if (txtPassword.Password == passwordCheck+Mac)
                    {
                        username = txtUsername.Text;
                        password = txtPassword.Password.Substring(0,4);
                        MainWindow mw = new MainWindow(username,password);
                        mw.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Incorrect Password");
                        txtPassword.Clear();
                        txtPassword.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Incorrect Username");
                    txtUsername.Focus();
                }
                //MessageBox.Show("Something went wrong");
            cc.CloseConnection();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }
    }
}
