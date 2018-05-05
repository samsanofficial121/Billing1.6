using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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

namespace Billing
{
    /// <summary>
    /// Interaction logic for SalesHistory.xaml
    /// </summary>
    public partial class SalesHistory : Page
    {
        ConnectionClass cc = new ConnectionClass();
        public static string billNo;
        public static int billExistValue, bno;
        public static List<string> billNoList = new List<string>();

        public static List<int> billnoList = new List<int>();
        public static List<string> custNameList = new List<string>();
        public static List<string> custPhoneList = new List<string>();
        public static List<int> barcodeList = new List<int>();
        public static List<string> itemNameList = new List<string>();
        public static List<double> priceList = new List<double>();
        public static List<int> qtyList = new List<int>();
        public static List<string> gstPercentList = new List<string>();
        public static List<double> gstRateList = new List<double>();
        public static List<double> totalGstList = new List<double>();
        public static List<double> totalPriceList = new List<double>();
        public static List<double> netAmountList = new List<double>();
        public static List<double> salesReturnList = new List<double>();
        public static List<string> saleDateList = new List<string>();
        public static int bNO;
        public static BillingReport Report = new BillingReport();

        public SalesHistory()
        {
            InitializeComponent();
            billNoList.Clear();
            txtBillNo.Focus();
            readBillNumber();
            viewBill();
            blankDataGrid.Visibility = System.Windows.Visibility.Visible;
        }

        private void viewBill()
        {
            if (txtBillNo.Text != null)
            {
                cc.OpenConnection();
                string txtbn = Convert.ToString(txtBillNo.Text);
                if (cc.BillPreview("select COUNT(*) from BillStock where BillNo = @bno ", "@bno", txtbn, billExistValue) > 0)
                {
                    displayDetails();
                    int bn = Convert.ToInt32(txtBillNo.Text);
                    cc.DataGridDisplay("select ItemNo,Product,Price,ProductQuantity,gstPercent,gstRate,TotalPrice from BillStock where BillNo = " + bn + " ");
                    dataGrid1.ItemsSource = cc.dt.AsDataView();
                    dataGrid1.Visibility = System.Windows.Visibility.Visible;
                    txtBillNo.Focus();
                }
                else
                {
                    MessageBox.Show("Bill does not exist");
                    clearDetails();
                    txtBillNo.Focus();
                }
                cc.CloseConnection();
            }
        }

        private void clearDetails()
        {
            labelNetAmount.Content = "";
            textBoxCustomer.Text = "";
            textBoxCustomerPh.Text = "";
        }

        private void displayDetails()
        {
            cc.OpenConnection();
            int tbn = Convert.ToInt32(txtBillNo.Text);
            cc.DataReader("select NetAmount,SaleDate,CustomerName,CustomerPhone,Credit from BillStock where BillNo = " + tbn + " ");
            while (cc.reader.Read())
            {
                labelNetAmount.Content = cc.reader["NetAmount"].ToString();
                txtSaleDate.Text = cc.reader["SaleDate"].ToString();
                textBoxCustomer.Text = cc.reader["CustomerName"].ToString();
                textBoxCustomerPh.Text = cc.reader["CustomerPhone"].ToString();
                textBox_Credit.Text = cc.reader["Credit"].ToString();
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void readBillNumber()
        {
            string saleBillNoPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Config\Config.txt");
            string saleBillNoValue = File.ReadAllText(saleBillNoPath);
            if (saleBillNoValue.Contains("SalesBillNumber"))
            {
                string[] val = saleBillNoValue.Substring(saleBillNoValue.IndexOf("SalesBillNumber") + 17).Split(Convert.ToChar("'"));
                billNoList.Add(val[0].ToString());
                foreach (var number in billNoList)
                {
                    billNo = Convert.ToString(number);
                    txtBillNo.Text = (Convert.ToInt32(billNo) - 1).ToString();
                }
            }
        }

        private void backbutton_Click(object sender, RoutedEventArgs e)
        {
            SellPage sp = new SellPage();
            this.NavigationService.Navigate(sp);
        }

        private void txtBillNo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var txtBillNo = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void btn_Next_Click(object sender, RoutedEventArgs e)
        {
            if (txtBillNo.Text == "")
            {
                MessageBox.Show("Enter bill number");
                clearDetails();
                dataGrid1.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                dataGrid1.Visibility = System.Windows.Visibility.Hidden;
                clearDetails();
                int bnoi;
                bnoi = Convert.ToInt32(txtBillNo.Text) + 1;
                txtBillNo.Text = Convert.ToString(bnoi);
                txtBillNo.Focus();
                viewBill();
            }
        }

        private void btn_Prev_Click(object sender, RoutedEventArgs e)
        {
            if (txtBillNo.Text == "")
            {
                MessageBox.Show("Enter bill number");
                clearDetails();
                dataGrid1.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                dataGrid1.Visibility = System.Windows.Visibility.Hidden;
                clearDetails();
                int bnod;
                bnod = Convert.ToInt32(txtBillNo.Text) - 1;
                if (bnod > 0)
                {
                    txtBillNo.Text = Convert.ToString(bnod);
                    txtBillNo.Focus();
                }
                viewBill();
            }
        }

        private void txtBillNo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            clearDetails();
            dataGrid1.Visibility = System.Windows.Visibility.Hidden;
            if (e.Key == Key.Return)
            {
                if (txtBillNo.Text == "")
                {
                    MessageBox.Show("Enter bill number");
                    clearDetails();
                    dataGrid1.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    cc.OpenConnection();
                    string txtbn = Convert.ToString(txtBillNo.Text);
                    if (cc.BillPreview("select COUNT(*) from BillStock where BillNo = @bno ", "@bno", txtbn, billExistValue) > 0)
                    {
                        displayDetails();
                        int bn = Convert.ToInt32(txtBillNo.Text);
                        cc.DataGridDisplay("select ItemNo,Product,Price,ProductQuantity,gstPercent,gstRate,TotalPrice from BillStock where BillNo = " + bn + " ");
                        dataGrid1.ItemsSource = cc.dt.AsDataView();
                        dataGrid1.Visibility = System.Windows.Visibility.Visible;
                        txtBillNo.Focus();
                    }
                    else
                    {
                        MessageBox.Show("Bill does not exist");
                        clearDetails();
                        txtBillNo.Focus();
                        dataGrid1.Visibility = System.Windows.Visibility.Hidden;
                    }
                    cc.CloseConnection();
                }
            }
        }

        private void textBox_Credit_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (textBox_Credit.Text == "")
                    textBox_Credit.Text = "0";
                cc.OpenConnection();
                int tbn = Convert.ToInt32(txtBillNo.Text);
                cc.ExecuteQuery("update BillStock set Credit=" + textBox_Credit.Text + " where BillNo=" + tbn + "");
                cc.CloseConnection();
                MessageBox.Show("Credit Updated");
            }
        }

        private void btn_Print_Click(object sender, RoutedEventArgs e)
        {
            bno = Convert.ToInt32(txtBillNo.Text);
            clearList();
            bNO = Convert.ToInt32(bno);
            generateBill();
        }

        private void clearList()
        {
            billnoList.Clear();
            barcodeList.Clear();
            itemNameList.Clear();
            priceList.Clear();
            qtyList.Clear();
            totalPriceList.Clear();
            netAmountList.Clear();
            custNameList.Clear();
            custPhoneList.Clear();
            gstPercentList.Clear();
            gstRateList.Clear();
            totalGstList.Clear();
            saleDateList.Clear();
            salesReturnList.Clear();
        }

        private void generateBill()
        {
            BillingReportData billDetails = new BillingReportData();
            DataTable dataTable = billDetails._BillingReportData;
            addBill();
            for (int i = 0; i < billnoList.Count; i++)
            {
                DataRow drow = dataTable.NewRow();
                drow["Bno"] = billnoList[i];
                drow["CustName"] = custNameList[i];
                drow["CustPhone"] = custPhoneList[i];
                drow["Barcode"] = barcodeList[i];
                drow["Item"] = itemNameList[i];
                drow["Rate"] = priceList[i];
                drow["Unit"] = qtyList[i];
                drow["SalesReturn"] = salesReturnList[i];
                drow["Gst%"] = gstPercentList[i];
                drow["Gst"] = gstRateList[i];
                drow["TotalGst"] = totalGstList[i];
                drow["Total"] = totalPriceList[i];
                drow["GTotal"] = netAmountList[i];
                drow["Date"] = saleDateList[i];
                dataTable.Rows.Add(drow);
            }
            Report.Database.Tables["BillingReportData"].SetDataSource((DataTable)dataTable);
            Report.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
            Report.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
            Report.PrintToPrinter(1, true, 0, 0);
        }

        private void addBill()
        {
            cc.OpenConnection();
            cc.DataReader("select * from BillStock where BillNo = " + bNO + " ");
            while (cc.reader.Read())
            {
                billnoList.Add(Convert.ToInt32(cc.reader["BillNo"]));
                barcodeList.Add(Convert.ToInt32(cc.reader["ItemNo"]));
                itemNameList.Add(Convert.ToString(cc.reader["Product"]));
                priceList.Add(Convert.ToDouble(cc.reader["Price"]));
                qtyList.Add(Convert.ToInt32(cc.reader["ProductQuantity"]));
                totalPriceList.Add(Convert.ToDouble(cc.reader["TotalPrice"]));
                netAmountList.Add(Convert.ToDouble(cc.reader["NetAmount"]));
                custNameList.Add(Convert.ToString(cc.reader["CustomerName"]));
                custPhoneList.Add(Convert.ToString(cc.reader["CustomerPhone"]));
                salesReturnList.Add(Convert.ToDouble(cc.reader["SalesReturn"]));
                gstPercentList.Add(Convert.ToString(cc.reader["gstPercent"]));
                gstRateList.Add(Convert.ToDouble(cc.reader["gstRate"]));
                totalGstList.Add(Convert.ToDouble(cc.reader["netGst"]));
                saleDateList.Add(Convert.ToString(cc.reader["SaleDate"]));
            }
            cc.CloseReader();
        }

        private void dataGrid1_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void textBoxCustomerPh_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBoxCustomerPh = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }
    }
}
