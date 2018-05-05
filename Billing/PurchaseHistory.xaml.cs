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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Configuration;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Billing
{
    /// <summary>
    /// Interaction logic for PurchaseHistory.xaml
    /// </summary>
    public partial class PurchaseHistory : Page
    {
        ConnectionClass cc = new ConnectionClass();
        public static int billExistValue, bno;
        public static string billNo;
        public static List<string> billNoList = new List<string>();

        public PurchaseHistory()
        {
            InitializeComponent();
            billNoList.Clear();
            txt_Bill_No.Focus();
            readBillNumber();
            ViewBill();
            blankDataGrid.Visibility = System.Windows.Visibility.Visible;
        }

        private void readBillNumber()
        {
            string billNoPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Config\Config.txt");
            string billNoValue = File.ReadAllText(billNoPath);
            if (billNoValue.Contains("PurchaseBillNumber"))
            {
                string[] val = billNoValue.Substring(billNoValue.IndexOf("PurchaseBillNumber") + 20).Split(Convert.ToChar("'"));
                billNoList.Add(val[0].ToString());
                foreach (var number in billNoList)
                {
                    billNo = number.ToString();
                    txt_Bill_No.Text = (Convert.ToInt32(billNo) - 1).ToString();
                }
            }
        }

        private void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            PurchasePage pp = new PurchasePage();
            this.NavigationService.Navigate(pp);
        }

        private void btn_Next_Click(object sender, RoutedEventArgs e)
        {
            if (txt_Bill_No.Text == "")
            {
                MessageBox.Show("Enter bill number");
                clearDetails();
                dataGridPurchase.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                dataGridPurchase.Visibility = System.Windows.Visibility.Hidden;
                clearDetails();
                int bnoi;
                bnoi = Convert.ToInt32(txt_Bill_No.Text) + 1;
                txt_Bill_No.Text = Convert.ToString(bnoi);
                txt_Bill_No.Focus();
                ViewBill();
            }
        }

        private void btn_Prev_Click(object sender, RoutedEventArgs e)
        {
            if (txt_Bill_No.Text == "")
            {
                MessageBox.Show("Enter bill number");
                clearDetails();
                dataGridPurchase.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                dataGridPurchase.Visibility = System.Windows.Visibility.Hidden;
                clearDetails();
                int bnod;
                bnod = Convert.ToInt32(txt_Bill_No.Text) - 1;
                if (bnod > 0)
                {
                    txt_Bill_No.Text = Convert.ToString(bnod);
                    txt_Bill_No.Focus();
                }
                ViewBill();
            }
        }

        private void txt_Bill_No_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            clearDetails();
            dataGridPurchase.Visibility = System.Windows.Visibility.Hidden;
                if (e.Key == Key.Return)
                {
                    if (txt_Bill_No.Text == "")
                    {
                        MessageBox.Show("Enter bill number");
                        clearDetails();
                        dataGridPurchase.Visibility = System.Windows.Visibility.Hidden;
                    }
                    else
                    {
                        cc.OpenConnection();
                        string txtbn = Convert.ToString(txt_Bill_No.Text);
                        if (cc.BillPreview("select COUNT(*) from Stock where Bno = @bno ", "@bno", txtbn, billExistValue) > 0)
                        {
                            displayVendorDetails();
                            int bn = Convert.ToInt32(txt_Bill_No.Text);
                            cc.DataGridDisplay("select itemid,iname,rate,purchaseQty,total,sprice,gst_percent,gst_amount from Stock where Bno = " + bn + " ");
                            dataGridPurchase.ItemsSource = cc.dt.AsDataView();
                            dataGridPurchase.Visibility = System.Windows.Visibility.Visible;
                            txt_Bill_No.Focus();
                        }
                        else
                        {
                            MessageBox.Show("Bill does not exist");
                            clearDetails();
                            txt_Bill_No.Focus();
                            dataGridPurchase.Visibility = System.Windows.Visibility.Hidden;
                        }
                        cc.CloseConnection();
                    }
                }
        }

        private void ViewBill()
        {
            if(txt_Bill_No.Text != null)
            {
                cc.OpenConnection();
                string txtbn = Convert.ToString(txt_Bill_No.Text);
                if (cc.BillPreview("select COUNT(*) from Stock where Bno = @bno ", "@bno", txtbn, billExistValue) > 0)
                {
                    displayVendorDetails();
                    int bn = Convert.ToInt32(txt_Bill_No.Text);
                    cc.DataGridDisplay("select itemid,iname,rate,purchaseQty,total,sprice,gst_percent,gst_amount from Stock where Bno = " + bn + " ");
                    dataGridPurchase.ItemsSource = cc.dt.AsDataView();
                    dataGridPurchase.Visibility = System.Windows.Visibility.Visible;
                    txt_Bill_No.Focus();
                }
                else
                {
                    MessageBox.Show("Bill does not exist");
                    clearDetails();
                    txt_Bill_No.Focus();
                }
                cc.CloseConnection();
            }
        }

        private void displayVendorDetails()
        {
            cc.OpenConnection();
            int tbn = Convert.ToInt32(txt_Bill_No.Text);
            cc.CreateView("CREATE VIEW DisplayVendor AS Select Stock.Bno,Stock.gtotal,TransactionDetails.Vid,TransactionDetails.Vname,TransactionDetails.Vphone,TransactionDetails.Vplace,TransactionDetails.PurchaseDate from Stock,TransactionDetails where Stock.Bno=TransactionDetails.Bno");
            cc.DataReader("Select * from DisplayVendor where Bno=" + tbn + "");
            while (cc.reader.Read())
            {
                txt_Net_Amount.Text = cc.reader["gtotal"].ToString();
                txt_Vendor_Id.Text = cc.reader["Vid"].ToString();
                txt_Vendor_Name.Text = cc.reader["Vname"].ToString();
                txt_Vendor_Place.Text = cc.reader["Vplace"].ToString();
                txt_Vendor_Phone.Text = cc.reader["Vphone"].ToString();
                txt_Purchase_Date.Text = cc.reader["PurchaseDate"].ToString();
            }
            cc.CloseReader();
            cc.DropView("DROP VIEW DisplayVendor");
            cc.CloseConnection();
        }

        private void btn_BarCode_Click(object sender, RoutedEventArgs e)
        {
            bno = Convert.ToInt32(txt_Bill_No.Text);
            BarCodePageHistory bph = new BarCodePageHistory();
            this.NavigationService.Navigate(bph);
        }

        private void txt_Bill_No_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var txt_Bill_No = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void dataGridPurchase_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void clearDetails()
        {
            txt_Net_Amount.Text = "";
            txt_Vendor_Id.Text = "";
            txt_Vendor_Name.Text = "";
            txt_Vendor_Place.Text = "";
            txt_Vendor_Phone.Text = "";
            txt_Purchase_Date.Text = "";
        }
    }
}
