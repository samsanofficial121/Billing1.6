using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Billing
{
    /// <summary>
    /// Interaction logic for CustomerBill.xaml
    /// </summary>
    public partial class CustomerBill : Window
    {
        ConnectionClass cc = new ConnectionClass();
        public static string billNumber;
        public static int bno;

        public CustomerBill()
        {
            InitializeComponent();
            billNumber = CustomerDetails.billNo;
            bno = Convert.ToInt32(billNumber);
            billNumberDisplay();
            billDisplay();
        }

        private void billDisplay()
        {
            cc.OpenConnection();
            cc.DataGridDisplay("select distinct ItemNo,Product,Price,ProductQuantity,TotalPrice,gstPercent,gstRate from BillStock where BillNo = " + bno + " ");
            dataGridCustomer.ItemsSource = cc.dt.AsDataView();
            dataGridCustomer.Visibility = System.Windows.Visibility.Visible;
            cc.CloseConnection();
        }

        private void billNumberDisplay()
        {
            cc.OpenConnection();
            cc.DataReader("select distinct BillNo,netGst,NetAmount,SaleDate,Credit from BillStock where BillNo = " + bno + " ");
            while (cc.reader.Read())
            {
                txt_Bill_Number.Text = cc.reader["BillNo"].ToString();
                txt_Sale_Date.Text = cc.reader["SaleDate"].ToString();
                txt_Total_Gst.Text = "₹ " + cc.reader["netGst"].ToString();
                txt_Grant_Total.Text = "₹ " + cc.reader["NetAmount"].ToString();
                textBox_Credit.Text = cc.reader["Credit"].ToString();
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
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
                cc.ExecuteQuery("update BillStock set Credit=" + textBox_Credit.Text + " where BillNo=" + bno + "");
                cc.CloseConnection();
                MessageBox.Show("Credit Updated");
            }
        }

        private void textBox_Credit_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_Credit.ToolTip = "Changes in here will be saved...";
        }

        private void dataGridCustomer_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
