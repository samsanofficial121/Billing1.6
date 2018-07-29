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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Billing
{
    /// <summary>
    /// Interaction logic for VendorPurchases.xaml
    /// </summary>
    public partial class VendorPurchases : Window
    {
        ConnectionClass cc = new ConnectionClass();
        public static string vendorID,billNo, vendorName;
        public static int grantTotal, cashPaid, balaceAmount;

        public VendorPurchases()
        {
            InitializeComponent();
            vendorID = ListTheVendor.vendorId;
            ShowGrid();
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
            if (MainWindow.userName=="admin")
            {
                if(MainWindow.isgst==1)
                {
                    cc.DataReader("select sum(Payment) as CashPaid from TransactionDetails where Vid=" + vendorID + " and BillType='GST'");
                }
                else
                {
                    cc.DataReader("select sum(Payment) as CashPaid from TransactionDetails where Vid=" + vendorID + " and BillType='NON_GST'");
                }

            }
            else
            {
                cc.DataReader("select sum(Payment) as CashPaid from TransactionDetails where Vid=" + vendorID + " and BillType='GST'");
            }
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
            if (MainWindow.userName == "admin")
            {
                if (MainWindow.isgst == 1)
                {
                    cc.DataReader("select sum(gtotal) as BillAmount from TransactionDetails where Vid=" + vendorID + " and BillType='GST'");
                }
                else
                {
                    cc.DataReader("select sum(gtotal) as BillAmount from TransactionDetails where Vid=" + vendorID + " and BillType='NON_GST'");
                }

            }
            else
            {
                cc.DataReader("select sum(gtotal) as BillAmount from TransactionDetails where Vid=" + vendorID + " and BillType='GST'");
            }
            while (cc.reader.Read())
            {
                grantTotal = Convert.ToInt32(cc.reader["BillAmount"]);
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void ShowGrid()
        {
            LoadVendor();
            if(MainWindow.userName=="admin")
            {
                if(MainWindow.isgst==1)
                {
                    cc.OpenConnection();
                    cc.DataGridDisplay("Select Bno,gtotal,Vid,FORMAT(PurchaseDate,'dd-MMM-yyyy') as PDate,Payment from TransactionDetails where Vid=" + vendorID + " and BillType='GST'");
                    dataGridPurchasedVendor.ItemsSource = cc.dt.AsDataView();
                    dataGridPurchasedVendor.Visibility = System.Windows.Visibility.Visible;
                    cc.CloseConnection();
                }
                else
                {
                    cc.OpenConnection();
                    cc.DataGridDisplay("Select Bno,gtotal,Vid,FORMAT(PurchaseDate,'dd-MMM-yyyy') as PDate,Payment from TransactionDetails where Vid=" + vendorID + " and BillType='NON_GST'");
                    dataGridPurchasedVendor.ItemsSource = cc.dt.AsDataView();
                    dataGridPurchasedVendor.Visibility = System.Windows.Visibility.Visible;
                    cc.CloseConnection();
                }
            }
            else
            {
                cc.OpenConnection();
                cc.DataGridDisplay("Select Bno,gtotal,Vid,FORMAT(PurchaseDate,'dd-MMM-yyyy') as PDate,Payment from TransactionDetails where Vid=" + vendorID + " and BillType='GST'");
                dataGridPurchasedVendor.ItemsSource = cc.dt.AsDataView();
                dataGridPurchasedVendor.Visibility = System.Windows.Visibility.Visible;
                cc.CloseConnection();
            }
        }

        private void dataGridPurchasedVendor_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void paymentButton_Click(object sender, RoutedEventArgs e)
        {
            vendorName = textBlockVName.Text;
            VendorPayment vp = new VendorPayment();
            vp.InputChanged += OnDialogInputChanged;
            vp.lblHeading.Visibility = Visibility.Collapsed;
            vp.comboBoxVendorName.Visibility = Visibility.Collapsed;
            vp.lblName.Visibility = Visibility.Visible;
            vp.labelVendorName.Visibility = Visibility.Visible;
            vp.Show();
        }

        private void OnDialogInputChanged(object sender, VendorPayment.DialogInputEventArgs e)
        {
            var vp = sender as VendorPayment;
            string balancePayment = vp.txtBalance.Text;
            txtBalance.Text = balancePayment;
            ShowGrid();
        }

        private void dataGridPurchasedVendor_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                billNo = ((DataRowView)dataGridPurchasedVendor.SelectedItem).Row["Bno"].ToString();
                VendorPurchaseBill vpb = new VendorPurchaseBill();
                vpb.ShowDialog();
            }
            catch (Exception)
            {
                MessageBox.Show("Click on a bill");
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void LoadVendor()
        {
            cc.OpenConnection();
            cc.DataReader("select Vid,Vname,Vphone,Vplace from VendorDetails where Vid=" + vendorID +"");
            while (cc.reader.Read())
            {
                textBlockVId.Text = cc.reader["Vid"].ToString();
                textBlockVName.Text = cc.reader["Vname"].ToString();
                textBlockVPhone.Text = cc.reader["Vphone"].ToString();
                textBlockVPlace.Text = cc.reader["Vplace"].ToString();
            }
            cc.CloseReader();
            cc.CloseConnection();
        }
    }
}
