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
    /// Interaction logic for VendorPurchaseBill.xaml
    /// </summary>
    public partial class VendorPurchaseBill : Window
    {
        ConnectionClass cc = new ConnectionClass();
        public static string billNumber;
        public VendorPurchaseBill()
        {
            InitializeComponent();
            billNumber = VendorPurchases.billNo;
            ShowGrid();
        }

        private void ShowGrid()
        {
            ShowDetails();
            cc.DataGridDisplay("select itemid,iname,rate,purchaseQty,total,sprice,gst_percent,gst_amount from Stock where Bno = "+billNumber+" ");
            dataGridVendorBill.ItemsSource = cc.dt.AsDataView();
            dataGridVendorBill.Visibility = System.Windows.Visibility.Visible;
        }

        private void ShowDetails()
        {
            cc.OpenConnection();
            cc.CreateView("CREATE VIEW PurchaseBill AS Select Distinct Stock.Bno,Stock.gtotal,TransactionDetails.PurchaseDate,TransactionDetails.CreditAmount,TransactionDetails.PaymentType from Stock,TransactionDetails where Stock.Bno=TransactionDetails.Bno");
            cc.DataReader("select * from PurchaseBill where Bno = " + billNumber + "");
            while (cc.reader.Read())
            {
                txt_Bill_Number.Text = cc.reader["Bno"].ToString();
                txt_Sale_Date.Text = cc.reader["PurchaseDate"].ToString();
                txt_Grant_Total.Text = "₹ " + cc.reader["gtotal"].ToString();
                textBox_Credit.Text = cc.reader["CreditAmount"].ToString();
                txt_Payment_Type.Text = cc.reader["PaymentType"].ToString();
            }
            cc.CloseReader();
            cc.DropView("DROP VIEW PurchaseBill");
            cc.CloseConnection();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void textBox_Credit_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (textBox_Credit.Text == "")
                    textBox_Credit.Text = "0";
                cc.OpenConnection();
                cc.ExecuteQuery("update TransactionDetails set CreditAmount=" + textBox_Credit.Text + " where Bno=" + billNumber + "");
                cc.CloseConnection();
                MessageBox.Show("Credit Updated");
            }
        }

        private void textBox_Credit_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_Credit.ToolTip = "Changes in here will be saved...";
        }

        private void dataGridVendorBill_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
