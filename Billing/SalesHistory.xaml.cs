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
        public static string billNo, currentBillNo;
        public static int billExistValue, bno, nextBillno;
        public static List<string> billNosList = new List<string>();
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
        public static List<double> discountList = new List<double>();
        public static int bNO;
        public static BillingReport Report = new BillingReport();

        public SalesHistory()
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

        private void viewBill()
        {
            if (txtBillNo.Text != null)
            {
                if (MainWindow.userName == "admin")
                {
                    cc.OpenConnection();
                    string txtbn = Convert.ToString(txtBillNo.Text);
                    if(checkBoxGST.IsChecked==true)
                    {
                        if (cc.BillPreview("select COUNT(*) from BillStock where BillNo = @bno and BillType='GST'", "@bno", txtbn, billExistValue) > 0)
                        {
                            displayDetails();
                            int bn = Convert.ToInt32(txtBillNo.Text);
                            cc.DataGridDisplay("select ItemNo,Product,Price,ProductQuantity,gstPercent,gstRate,TotalPrice from BillStock where BillNo = " + bn + " and BillType='GST' ");
                            dataGrid1.ItemsSource = cc.dt.AsDataView();
                        }
                        else
                        {
                            MessageBox.Show("Bill does not exist");
                            txtBillNo.Text = currentBillNo;
                            txtBillNo.Focus();
                        }
                    }
                    else
                    {
                        if (cc.BillPreview("select COUNT(*) from BillStock where BillNo = @bno and BillType='NON_GST'", "@bno", txtbn, billExistValue) > 0)
                        {
                            displayDetails();
                            int bn = Convert.ToInt32(txtBillNo.Text);
                            cc.DataGridDisplay("select ItemNo,Product,Price,ProductQuantity,gstPercent,gstRate,TotalPrice from BillStock where BillNo = " + bn + " and BillType='NON_GST'");
                            dataGrid1.ItemsSource = cc.dt.AsDataView();
                        }
                        else
                        {
                            MessageBox.Show("Bill does not exist");
                            txtBillNo.Text = currentBillNo;
                            txtBillNo.Focus();
                        }
                    }
                    cc.CloseConnection();
                }
                else
                {
                    cc.OpenConnection();
                    string txtbn = Convert.ToString(txtBillNo.Text);
                    if (cc.BillPreview("select COUNT(*) from BillStock where BillNo = @bno and BillType='GST'", "@bno", txtbn, billExistValue) > 0)
                    {
                        displayDetails();
                        int bn = Convert.ToInt32(txtBillNo.Text);
                        cc.DataGridDisplay("select ItemNo,Product,Price,ProductQuantity,gstPercent,gstRate,TotalPrice from BillStock where BillNo = " + bn + " and BillType='GST' ");
                        dataGrid1.ItemsSource = cc.dt.AsDataView();
                    }
                    else
                    {
                        MessageBox.Show("Bill does not exist");
                        txtBillNo.Text = currentBillNo;
                        txtBillNo.Focus();
                    }
                    cc.CloseConnection();
                }
            }
        }

        private void displayDetails()
        {
            cc.OpenConnection();
            int tbn = Convert.ToInt32(txtBillNo.Text);
            if(MainWindow.userName=="admin")
            {
                if (checkBoxGST.IsChecked == true)
                {
                    cc.DataReader("select NetAmount,SaleDate,CustomerName,CustomerPhone,Credit from BillStock where BillNo = " + tbn + " and BillType='GST' ");
                }
                else
                {
                    cc.DataReader("select NetAmount,SaleDate,CustomerName,CustomerPhone,Credit from BillStock where BillNo = " + tbn + " and BillType='NON_GST' ");
                }
            }
            else
            {
                cc.DataReader("select NetAmount,SaleDate,CustomerName,CustomerPhone,Credit from BillStock where BillNo = " + tbn + " and BillType='GST' ");
            }
            while (cc.reader.Read())
            {
                txtBlkNetAmount.Text = cc.reader["NetAmount"].ToString();
                txtSaleDate.Text = cc.reader["SaleDate"].ToString();
                textBoxCustomer.Text = cc.reader["CustomerName"].ToString();
                textBoxCustomerPh.Text = cc.reader["CustomerPhone"].ToString();
                textBlock_Credit.Text = cc.reader["Credit"].ToString();
            }
            cc.CloseReader();
            cc.CloseConnection();
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
            }
            else
            {
                nextBillno = billNosList.IndexOf(txtBillNo.Text);
                int bnod;
                bnod = nextBillno + 1;
                if (bnod < billNosList.Count)
                {
                    txtBillNo.Text = billNosList[bnod];
                    viewBill();
                }
                else
                {
                    MessageBox.Show("End of List...!");
                }
            }
        }

        private void btn_Prev_Click(object sender, RoutedEventArgs e)
        {
            if (txtBillNo.Text == "")
            {
                MessageBox.Show("Enter bill number");
            }
            else
            {
                nextBillno = billNosList.IndexOf(txtBillNo.Text);
                int bnod;
                bnod = nextBillno - 1;
                if (bnod >= 0)
                {
                    txtBillNo.Text = billNosList[bnod];
                    viewBill();
                }
                else
                {
                    MessageBox.Show("End of List...!");
                }
            }
        }

        private void txtBillNo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (txtBillNo.Text == "")
                {
                    MessageBox.Show("Enter bill number");
                    txtBillNo.Text = currentBillNo;
                    txtBillNo.Focus();
                }
                else
                {
                    viewBill();
                }
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
            discountList.Clear();
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
                string[] custName = Regex.Split(custNameList[i], "  ");
                drow["CustName"] = custName[0];
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
            DataTable dTable = billDetails.BillingData;
            DataRow dr = dTable.NewRow();
            dr["NetAmount"] = totalPriceList.Sum();
            dr["Discount"] = discountList[discountList.Count - 1];
            dTable.Rows.Add(dr);
            Report.Database.Tables["BillingData"].SetDataSource((DataTable)dTable);
            try
            {
                Report.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
                Report.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                Report.PrintToPrinter(1, true, 0, 0);
            }
            catch (Exception)
            {
                MessageBox.Show("No printer connected");
            }
            
        }

        private void addBill()
        {
            cc.OpenConnection();
            if(MainWindow.userName=="admin")
            {
                if (checkBoxGST.IsChecked == true)
                {
                    cc.DataReader("select * from BillStock where BillNo = " + bNO + " BillType='GST' ");
                }
                else
                {
                    cc.DataReader("select * from BillStock where BillNo = " + bNO + " BillType='NON_GST' ");
                }
            }
            else
            {
                cc.DataReader("select * from BillStock where BillNo = " + bNO + " BillType='GST' ");
            }
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
                discountList.Add(Convert.ToDouble(cc.reader["Discount"]));
            }
            cc.CloseReader();
        }

        private void dataGrid1_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void billNoTextBoxChange()
        {
            cc.OpenConnection();
            if(MainWindow.userName=="admin")
            {
                if (checkBoxGST.IsChecked == true)
                {
                    cc.DataReader("select distinct BillNo from BillStock where BillType='GST'");
                }
                else
                {
                    cc.DataReader("select distinct BillNo from BillStock where BillType='NON_GST'");
                }
            }
            else
            {
                cc.DataReader("select distinct BillNo from BillStock where BillType='GST'");
            }
            while (cc.reader.Read())
            {
                billNosList.Add(Convert.ToString(cc.reader["BillNo"]));
            }
            cc.CloseReader();
            cc.CloseConnection();

        }

        private void ReadBillNumber()
        {
            if(billNosList.Count <= 0)
            {
                MessageBox.Show("No Bills to Show");
            }
            else
            {
                string bnl = billNosList[billNosList.Count - 1];
                txtBillNo.Text = bnl;
                viewBill();
            }
        }

        private void datatostock()
        {
            foreach (DataRow dg in cc.dt.Rows)
            {
                string itemid = dg[0].ToString();
                string quantity = dg[3].ToString();
                cc.OpenConnection();
                cc.ExecuteQuery("update stock set quantity = quantity+ " + Convert.ToInt32(quantity) + " where itemid = " + Convert.ToInt32(itemid) + "");
                cc.CloseConnection();
            }
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

        private void txtBillNo_GotFocus(object sender, RoutedEventArgs e)
        {
            currentBillNo = txtBillNo.Text;
        }

        private void deletebutton_Click(object sender, RoutedEventArgs e)
        {
            cc.OpenConnection();
            cc.ExecuteQuery("Delete from BillStock where BillNo =" + Convert.ToInt32(txtBillNo.Text) + "");
            cc.CloseConnection();
            MessageBox.Show("Bill Deleted!");
            datatostock();
            int bnod;
            bnod = Convert.ToInt32(txtBillNo.Text) - 1;
            if (bnod > 0)
            {
                txtBillNo.Text = Convert.ToString(bnod);
                txtBillNo.Focus();
            }
            viewBill();
            billNosList.Clear();
            billNoTextBoxChange();
        }

        private void textBoxCustomerPh_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBoxCustomerPh = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }
    }
}
