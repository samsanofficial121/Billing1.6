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
        public static string customerPhone, billNo, customerName, totalSDiscount;
        public static double balaceAmount, grantTotal, cashPaid, specialDiscount;

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
            calculateSpecialDiscount();
            balaceAmount = (grantTotal - cashPaid) - specialDiscount;
            txtBalance.Text = Convert.ToString(balaceAmount);
        }

        private void calculateCashPaid()
        {
            cc.OpenConnection();
            if (MainWindow.userName == "admin")
            {
                cc.DataReader("select sum(AmountPaid) as CashPaid from CustomerTransactions where CustomerPhone='" + customerPhone + "'");
            }
            else
            {
                cc.DataReader("select sum(AmountPaid) as CashPaid from CustomerTransactions where CustomerPhone='" + customerPhone + "' and BillType='GST'");
            }
            while (cc.reader.Read())
            {
                cashPaid = Convert.ToDouble(cc.reader["CashPaid"]);
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void calculateGrantTotal()
        {
            cc.OpenConnection();
            if (MainWindow.userName == "admin")
            {
                cc.DataReader("select sum(NetAmount) as BillAmount from CustomerTransactions where CustomerPhone='" + customerPhone + "'");
            }
            else
            {
                cc.DataReader("select sum(NetAmount) as BillAmount from CustomerTransactions where CustomerPhone='" + customerPhone + "' and BillType='GST'");
            }
            while (cc.reader.Read())
            {
                grantTotal = Convert.ToDouble(cc.reader["BillAmount"]);
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void calculateSpecialDiscount()
        {
            cc.OpenConnection();
            if (MainWindow.userName == "admin")
            {
                cc.DataReader("select sum(Discount) as SpecialDisc from CustomerTransactions where CustomerPhone='" + customerPhone + "'");
            }
            else
            {
                cc.DataReader("select sum(Discount) as SpecialDisc from CustomerTransactions where CustomerPhone='" + customerPhone + "' and BillType='GST'");
            }
            while (cc.reader.Read())
            {
                totalSDiscount = cc.reader["SpecialDisc"].ToString();
            }
            cc.CloseReader();
            cc.CloseConnection();
            specialDiscount = Convert.ToDouble(totalSDiscount);
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
            if (MainWindow.userName == "admin")
            {
                if (MainWindow.isgst == 1)
                {
                    cc.DataGridDisplay("select distinct BillNo,NetAmount,AmountPaid,Credit,Debit,Date from CustomerTransactions where CustomerPhone = '" + customerPhone + "' and BillType='GST' ");
                }
                else
                {
                    cc.DataGridDisplay("select distinct BillNo,NetAmount,AmountPaid,Credit,Debit,Date from CustomerTransactions where CustomerPhone = '" + customerPhone + "' and BillType='NON_GST' ");
                }
            }
            else
            {
                cc.DataGridDisplay("select distinct BillNo,NetAmount,AmountPaid,Credit,Debit,Date from CustomerTransactions where CustomerPhone = '" + customerPhone + "' and BillType='GST' ");
            }
            dataGridCustomer.ItemsSource = cc.dt.AsDataView();
            dataGridCustomer.Visibility = System.Windows.Visibility.Visible;
            cc.CloseConnection();
        }

        private void paymentButton_Click(object sender, RoutedEventArgs e)
        {
            customerName = txt_Customer_Name.Text;
            CustomerPayment cp = new CustomerPayment();
            cp.InputChanged += OnDialogInputChanged;
            cp.lblHeading.Visibility = Visibility.Collapsed;
            cp.comboBoxCustomerName.Visibility = Visibility.Collapsed;
            cp.lblName.Visibility = Visibility.Visible;
            cp.labelCustomerName.Visibility = Visibility.Visible;
            cp.Show();
        }

        private void OnDialogInputChanged(object sender, CustomerPayment.DialogInputEventArgs e)
        {
            var vp = sender as CustomerPayment;
            string balancePayment = vp.txtBalance.Text;
            txtBalance.Text = balancePayment;
            gridDisplay();
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
