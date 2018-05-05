using System;
using System.Collections.Generic;
using System.Data;
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

namespace Billing
{
    /// <summary>
    /// Interaction logic for CustomerDetails.xaml
    /// </summary>
    public partial class CustomerDetails : Window
    {
        ConnectionClass cc = new ConnectionClass();
        public static string customerPhone,billNo;
        public static int balaceAmount, grantTotal, cashPaid;

        public CustomerDetails()
        {
            InitializeComponent();
            customerPhone = CustomerList.custPhone;
            vendorDisplay();
            gridDisplay();
            showBalanceAmount();
        }

        private void showBalanceAmount()
        {
            calculateGrantTotal();
            calculateCashPaid();
            balaceAmount = grantTotal - cashPaid;
            txtBalance.Text = Convert.ToString(balaceAmount);
        }

        private void calculateCashPaid()
        {
            cc.OpenConnection();
            cc.DataReader("select sum(AmountPaid) as CashPaid from CustomerTransactions where CustomerPhone='" + customerPhone + "'");
            while (cc.reader.Read())
            {
                cashPaid = Convert.ToInt32(cc.reader["CashPaid"]);
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void calculateGrantTotal()
        {
            cc.OpenConnection();
            cc.DataReader("select sum(NetAmount) as BillAmount from CustomerTransactions where CustomerPhone='" + customerPhone + "'");
            while (cc.reader.Read())
            {
                grantTotal = Convert.ToInt32(cc.reader["BillAmount"]);
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void vendorDisplay()
        {
            cc.OpenConnection();
            cc.DataReader("select distinct CustomerName,CustomerPhone from BillStock where CustomerPhone = '" + customerPhone + "' ");
            while (cc.reader.Read())
            {
                txt_Customer_Name.Text = cc.reader["CustomerName"].ToString();
                txt_Customer_Phone.Text = cc.reader["CustomerPhone"].ToString();
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void gridDisplay()
        {
            cc.OpenConnection();
            cc.DataGridDisplay("select distinct BillNo,NetAmount,Credit,SaleDate from BillStock where CustomerPhone = '" + customerPhone + "' ");
            dataGridCustomer.ItemsSource = cc.dt.AsDataView();
            dataGridCustomer.Visibility = System.Windows.Visibility.Visible;
            cc.CloseConnection();
        }

        private void dataGridCustomer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                billNo = ((DataRowView)dataGridCustomer.SelectedItem).Row["BillNo"].ToString();
                CustomerBill cb = new CustomerBill();
                cb.ShowDialog();
            }
            catch (Exception)
            {
                MessageBox.Show("Click on a bill");
            }
        }

        private void dataGridCustomer_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }
    }
}
