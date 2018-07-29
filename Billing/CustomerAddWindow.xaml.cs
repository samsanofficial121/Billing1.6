using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for CustomerAddWindow.xaml
    /// </summary>
    public partial class CustomerAddWindow : Window
    {
        public static string genCustomerId;
        public static int customerId;
        ConnectionClass cc = new ConnectionClass();

        public CustomerAddWindow()
        {
            InitializeComponent();
            readCustomerId();
            textBoxCustomerName.Focus();
        }

        private void readCustomerId()
        {
            cc.OpenConnection();
            cc.DataReader("select ConfigValue from ConfigTable where ConfigId = 8 ");
            while (cc.reader.Read())
            {
                genCustomerId = cc.reader["ConfigValue"].ToString();
            }
            cc.CloseReader();
            cc.CloseConnection();
            txt_Customer_Id.Text = genCustomerId;
        }

        private void writeCustomerId()
        {
            cc.OpenConnection();
            cc.ExecuteQuery("update ConfigTable set ConfigValue=" + customerId + " where ConfigId = 8 ");
            cc.CloseConnection();
        }

        public event EventHandler<DialogInputEventArgs> InputChanged = delegate { };
        private void savebutton_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxCustomerName.Text == "" || textBoxCustomerPhone.Text == "")
            {
                MessageBox.Show("Enter Customer Details..!");
            }
            else
            {
                cc.OpenConnection();
                customerId = Convert.ToInt32(genCustomerId) + 1;
                cc.ExecuteQuery("insert into CustomerDetails(CustomerId,CustomerName,CustomerPhone,CardId,CardPoints) values( ' " + genCustomerId + " ' , '" + textBoxCustomerName.Text + "' , '" + textBoxCustomerPhone.Text + "', '" + textBoxCardId.Text + "',0)");
                MessageBox.Show("Customer Details Added");
                writeCustomerId();
                readCustomerId();
                InputChanged(this, new DialogInputEventArgs() { Input = this.textBoxCustomerName.Text });
                clearCustomerDetails();
                textBoxCustomerName.Focus();
                cc.CloseConnection();
                SystemCommands.CloseWindow(this);

            }
        }
        public class DialogInputEventArgs : EventArgs
        {
            public string Input { get; set; }
        }
        private void clearCustomerDetails()
        {
            textBoxCustomerName.Text = "";
            textBoxCustomerPhone.Text = "";
        }

        private void btn_CloseButton_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void textBoxCustomerName_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                textBoxCustomerPhone.Focus();
            }
        }

        private void textBoxCustomerPhone_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                textBoxCardId.Focus();
            }
        }

        private void textBoxCardId_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                savebutton.Focus();
            }
        }
    }
}