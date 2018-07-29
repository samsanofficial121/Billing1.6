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
        public static int billExistValue, bno, nextBillno;
        public static string billNo, currentBillNo;
        public static List<string> billNosList = new List<string>();

        public PurchaseHistory()
        {
            InitializeComponent();
            if(MainWindow.userName!="admin")
            {
                checkBoxGST.Visibility = Visibility.Collapsed;
            }
            billNosList.Clear();
            billNoTextBoxChange();
            ReadBillNumber();
        }

        private void billNoTextBoxChange()
        {
            cc.OpenConnection();
            if (MainWindow.userName == "admin")
            {
                if(checkBoxGST.IsChecked==true)
                {
                    cc.DataReader("select distinct Bno from Stock where BillType='GST'");
                }
                else
                {
                    cc.DataReader("select distinct Bno from Stock where BillType='NON_GST'");
                }
            }
            else
            {
                cc.DataReader("select distinct Bno from Stock where BillType='GST'");
            }
            while (cc.reader.Read())
            {
                billNosList.Add(Convert.ToString(cc.reader["Bno"]));
            }
            cc.CloseReader();
            cc.CloseConnection();

        }

        private void ReadBillNumber()
        {
            if (billNosList.Count <= 0)
            {
                MessageBox.Show("No Bills to Show");
            }
            else
            {
                string bnl = billNosList[billNosList.Count - 1];
                txt_Bill_No.Text = bnl;
                ViewBill();
            }
        }

        private void txt_Bill_No_GotFocus(object sender, RoutedEventArgs e)
        {
            currentBillNo = txt_Bill_No.Text;
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
            }
            else
            {
                nextBillno = billNosList.IndexOf(txt_Bill_No.Text);
                int bnod;
                bnod = nextBillno + 1;
                if (bnod < billNosList.Count)
                {
                    txt_Bill_No.Text = billNosList[bnod];
                    ViewBill();
                }
                else
                {
                    MessageBox.Show("End of List...!");
                }
            }
        }

        private void btn_Prev_Click(object sender, RoutedEventArgs e)
        {
            if (txt_Bill_No.Text == "")
            {
                MessageBox.Show("Enter bill number");
            }
            else
            {
                nextBillno = billNosList.IndexOf(txt_Bill_No.Text);
                int bnod;
                bnod = nextBillno - 1;
                if (bnod >= 0)
                {
                    txt_Bill_No.Text = billNosList[bnod];
                    ViewBill();
                }
                else
                {
                    MessageBox.Show("End of List...!");
                }
            }
        }

        private void txt_Bill_No_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (txt_Bill_No.Text == "")
                {
                    MessageBox.Show("Enter bill number");
                    txt_Bill_No.Text = currentBillNo;
                    txt_Bill_No.Focus();
                }
                else
                {
                    ViewBill();
                }
            }
        }

        private void ViewBill()
        {
            if(txt_Bill_No.Text != null)
            {
                if(MainWindow.userName=="admin")
                {
                    if (checkBoxGST.IsChecked == true)
                    {
                        cc.OpenConnection();
                        string txtbn = Convert.ToString(txt_Bill_No.Text);
                        if (cc.BillPreview("select COUNT(*) from Stock where Bno = @bno and BillType='GST'", "@bno", txtbn, billExistValue) > 0)
                        {
                            displayVendorDetails();
                            int bn = Convert.ToInt32(txt_Bill_No.Text);
                            cc.DataGridDisplay("select itemid,iname,rate,purchaseQty,total,sprice,gst_percent,gst_amount from Stock where Bno = " + bn + " and BillType='GST'");
                            dataGridPurchase.ItemsSource = cc.dt.AsDataView();
                        }
                        else
                        {
                            MessageBox.Show("Bill does not exist");
                            txt_Bill_No.Text = currentBillNo;
                            txt_Bill_No.Focus();
                        }
                        cc.CloseConnection();
                    }
                    else
                    {
                        cc.OpenConnection();
                        string txtbn = Convert.ToString(txt_Bill_No.Text);
                        if (cc.BillPreview("select COUNT(*) from Stock where Bno = @bno and BillType='NON_GST'", "@bno", txtbn, billExistValue) > 0)
                        {
                            displayVendorDetails();
                            int bn = Convert.ToInt32(txt_Bill_No.Text);
                            cc.DataGridDisplay("select itemid,iname,rate,purchaseQty,total,sprice,gst_percent,gst_amount from Stock where Bno = " + bn + " and BillType='NON_GST'");
                            dataGridPurchase.ItemsSource = cc.dt.AsDataView();
                        }
                        else
                        {
                            MessageBox.Show("Bill does not exist");
                            txt_Bill_No.Text = currentBillNo;
                            txt_Bill_No.Focus();
                        }
                        cc.CloseConnection();
                    }
                }
                else
                {
                    cc.OpenConnection();
                    string txtbn = Convert.ToString(txt_Bill_No.Text);
                    if (cc.BillPreview("select COUNT(*) from Stock where Bno = @bno and BillType='GST'", "@bno", txtbn, billExistValue) > 0)
                    {
                        displayVendorDetails();
                        int bn = Convert.ToInt32(txt_Bill_No.Text);
                        cc.DataGridDisplay("select itemid,iname,rate,purchaseQty,total,sprice,gst_percent,gst_amount from Stock where Bno = " + bn + " and BillType='GST'");
                        dataGridPurchase.ItemsSource = cc.dt.AsDataView();
                    }
                    else
                    {
                        MessageBox.Show("Bill does not exist");
                        txt_Bill_No.Text = currentBillNo;
                        txt_Bill_No.Focus();
                    }
                    cc.CloseConnection();
                }
            }
        }

        private void displayVendorDetails()
        {
            cc.OpenConnection();
            int tbn = Convert.ToInt32(txt_Bill_No.Text);
            if(MainWindow.userName=="admin")
            {
                if (checkBoxGST.IsChecked == true)
                {
                    cc.CreateView("CREATE VIEW DisplayVendor AS Select Stock.Bno,Stock.gtotal,TransactionDetails.Vid,TransactionDetails.Vname,TransactionDetails.Vphone,TransactionDetails.Vplace,FORMAT(TransactionDetails.PurchaseDate,'dd-MMM-yyyy') as PDate from Stock,TransactionDetails where Stock.Bno=TransactionDetails.Bno and Stock.BillType='GST' and TransactionDetails.BillType='GST'");
                }
                else
                {
                    cc.CreateView("CREATE VIEW DisplayVendor AS Select Stock.Bno,Stock.gtotal,TransactionDetails.Vid,TransactionDetails.Vname,TransactionDetails.Vphone,TransactionDetails.Vplace,FORMAT(TransactionDetails.PurchaseDate,'dd-MMM-yyyy') as PDate from Stock,TransactionDetails where Stock.Bno=TransactionDetails.Bno and Stock.BillType='NON_GST' and TransactionDetails.BillType='NON_GST'");
                }
            }
            else
            {
                cc.CreateView("CREATE VIEW DisplayVendor AS Select Stock.Bno,Stock.gtotal,TransactionDetails.Vid,TransactionDetails.Vname,TransactionDetails.Vphone,TransactionDetails.Vplace,FORMAT(TransactionDetails.PurchaseDate,'dd-MMM-yyyy') as PDate from Stock,TransactionDetails where Stock.Bno=TransactionDetails.Bno and Stock.BillType='GST' and TransactionDetails.BillType='GST'");
            }
            cc.DataReader("Select * from DisplayVendor where Bno=" + tbn + "");
            while (cc.reader.Read())
            {
                txt_Net_Amount.Text = cc.reader["gtotal"].ToString();
                txt_Vendor_Id.Text = cc.reader["Vid"].ToString();
                txt_Vendor_Name.Text = cc.reader["Vname"].ToString();
                txt_Vendor_Place.Text = cc.reader["Vplace"].ToString();
                txt_Vendor_Phone.Text = cc.reader["Vphone"].ToString();
                txt_Purchase_Date.Text = cc.reader["PDate"].ToString();
            }
            cc.CloseReader();
            cc.DropView("DROP VIEW DisplayVendor");
            cc.CloseConnection();
        }

        private void checkBoxGST_Checked(object sender, RoutedEventArgs e)
        {
            billNosList.Clear();
            billNoTextBoxChange();
            ReadBillNumber();
        }

        private void checkBoxGST_Unchecked(object sender, RoutedEventArgs e)
        {
            billNosList.Clear();
            billNoTextBoxChange();
            ReadBillNumber();
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
    }
}
