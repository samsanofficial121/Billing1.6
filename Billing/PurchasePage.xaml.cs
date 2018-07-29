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
using System.Configuration;
using System.IO;
using System.Resources;
using System.Drawing;
using Billing.Properties;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Billing
{
    /// <summary>
    /// Interaction logic for PurchasePage.xaml
    /// </summary>
    public partial class PurchasePage : Page
    {
        public static double Netgst;
        public static string selctedPaymentType, vendorName, Sno, billNumber, BillType;
        public static int bno, vendorId, genItemId, readItemId, billExistValue;
        public static List<string> vendorList = new List<string>();
        DataTable dt = new DataTable();
        ConnectionClass cc = new ConnectionClass();

        public PurchasePage()
        {
            InitializeComponent();
            datePicker.Text = DateTime.Now.Date.ToString("dd-MMM-yy");
            if (MainWindow.userName == "admin")
            {
                NonGst();
            }
            else
            {
                checkBoxGST.Visibility = Visibility.Collapsed;
                Gst();
            }
            comboBoxPaymentVendor.SelectedItem = "By Cash";
            comboBoxScale.SelectedItem = "Nos";
            FilterViewModel vm = new FilterViewModel();
            this.DataContext = vm;
            clearLists();
            clearTable();
            vendorNameComboBox.Focus();
            comboBoxScale.Items.Add("Kg");
            comboBoxScale.Items.Add("Mtr");
            comboBoxScale.Items.Add("Nos");
            comboBoxPaymentVendor.Items.Add("By Cash");
            comboBoxPaymentVendor.Items.Add("By Credit");
            comboBoxPaymentVendor.Items.Add("Multi Payment");
            fieldsClear();
            HideBalance();
            VendorAction();
            ReadItemId();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            dt.Columns.Add("Item_ID", typeof(int));
            dt.Columns.Add("Item_Name", typeof(string));
            dt.Columns.Add("Rate", typeof(double));
            dt.Columns.Add("Gst_Percent", typeof(double));
            dt.Columns.Add("Gst_Amount", typeof(double));
            dt.Columns.Add("Quantity", typeof(int));
            dt.Columns.Add("Scale", typeof(string));
            dt.Columns.Add("SQuantity", typeof(int));
            dt.Columns.Add("Total", typeof(double));
            dt.Columns.Add("Profit_Percent", typeof(double));
            dt.Columns.Add("Selling_Price", typeof(double));
            dt.Columns.Add("Gst_SPercent", typeof(double));
            dt.Columns.Add("Gst_SAmount", typeof(double));
        }

        private void clearLists()
        {
            vendorList.Clear();
        }

        private void clearTable()
        {
            dt.Clear();
            txtItemName.Focus();
            cc.CloseConnection();
            txtVendorId.Text = "";
            txtVendorPhone.Text = "";
            txtVendorPlace.Text = "";
            txtBox_CashPaid.Text = "";
            Label_BalanceAmnt.Content = "";
            txtBlkNetAmount.Text = "";
            vendorNameComboBox.SelectedItem = null;
            comboBoxPaymentVendor.SelectedItem = "By Cash";
        }

        private void VendorAction()
        {
            cc.OpenConnection();
            cc.DataReader("select Vname from VendorDetails");
            while (cc.reader.Read())
            {
                vendorList.Add(cc.reader["Vname"].ToString());
                vendorList.Sort();
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void fieldsClear()
        {
            txtBarcode.Text = "";
            txtItemName.Text = "";
            txtItemRate.Text = "";
            txtItemQty.Text = "";
            txtTotalPrice.Text = "";
            txtSellingPrice.Text = "";
            txtGstPercent.Text = "0";
            txtGstAmount.Text = "0";
            txtItemProfit.Text = "";
            txtItemSQty.Text = "";
            comboBoxScale.SelectedItem = "Nos";
            txtSaleGstPercent.Text = "0";
            txtSaleGstAmount.Text = "0";
        }

        private void CommonBarcode()
        {
            dt.Rows.Add(new string[] { txtBarcode.Text, txtItemName.Text, txtItemRate.Text, txtGstPercent.Text, txtGstAmount.Text, txtItemQty.Text, comboBoxScale.Text, txtItemSQty.Text, txtTotalPrice.Text, txtItemProfit.Text, txtSellingPrice.Text, txtSaleGstPercent.Text, txtSaleGstAmount.Text });
            dataGrid.ItemsSource = dt.DefaultView;
            txtBlkNetAmount.Text = (dt.Compute("Sum(Total)", "")).ToString();
            fieldsClear();
            cc.CloseConnection();
            txtItemName.Focus();
            txtGstPercent.Text = "0";
            txtGstAmount.Text = "0";
        }

        private void txtSellingPrice_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if(txtItemProfit.Text == "" && txtSellingPrice.Text == "")
                {
                    MessageBox.Show("Enter either 'Profit %' or 'Selling Price'");
                    txtItemProfit.Focus();
                }
                else
                {
                    txtSaleGstPercent.Focus();
                }
            }
        }

        private void txtGstPercent_GotFocus(object sender, RoutedEventArgs e)
        {
            txtGstPercent.Text = "";
            txtGstAmount.Text = "";
        }

        private void txtItemProfit_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if(txtItemProfit.Text=="")
                {
                    txtSellingPrice.Focus();
                }
                else
                {
                    txtSaleGstPercent.Focus();
                }
            }
        }

        private void txtSaleGstPercent_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (txtItemName.Text == "" || txtItemRate.Text == "" || txtItemQty.Text == "" || txtItemSQty.Text == "" || txtItemProfit.Text == "" || txtSellingPrice.Text == "")
                {
                    MessageBox.Show("Enter The Details");
                    txtItemName.Focus();
                }
                else
                {
                    if (txtBarcode.Text == "")
                    {
                        if (txtGstPercent.Text == "")
                        {
                            txtGstPercent.Text = "0";
                            txtGstAmount.Text = "0";
                        }
                        if (txtSaleGstPercent.Text == "")
                        {
                            txtSaleGstPercent.Text = "0";
                            txtSaleGstAmount.Text = "0";
                        }
                        dt.Rows.Add(new string[] { Convert.ToString(genItemId), txtItemName.Text, txtItemRate.Text, txtGstPercent.Text, txtGstAmount.Text, txtItemQty.Text, comboBoxScale.Text, txtItemSQty.Text, txtTotalPrice.Text, txtItemProfit.Text, txtSellingPrice.Text, txtSaleGstPercent.Text, txtSaleGstAmount.Text });
                        dataGrid.ItemsSource = dt.DefaultView;
                        txtBlkNetAmount.Text = (dt.Compute("Sum(Total)", "")).ToString();
                        fieldsClear();
                        genItemId += 1;
                        cc.CloseConnection();
                        txtItemName.Focus();
                        txtGstPercent.Text = "0";
                        txtGstAmount.Text = "0";
                    }
                    else
                    {
                        CommonBarcode();
                    }
                }
            }
        }

        private void newSNo()
        {
            cc.OpenConnection();
            cc.DataReader("select top 1 Sno from Stock order by Sno desc");
            if (cc.reader.HasRows)
            {
                while (cc.reader.Read())
                {
                    int sno = Convert.ToInt32(cc.reader["Sno"]);
                    Sno = Convert.ToString(sno + 1);
                }
            }
            else
            {
                Sno = "1";
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void newBillNo()
        {
            cc.OpenConnection();
            if (checkBoxGST.IsChecked == true)
            {
                cc.DataReader("select top 1 BNo from Stock where BillType='GST' order by BNo desc");
            }
            else
            {
                cc.DataReader("select top 1 BNo from Stock where BillType='NON_GST' order by BNo desc");
            }
            if (cc.reader.HasRows)
            {
                while (cc.reader.Read())
                {
                    int bnum = Convert.ToInt32(cc.reader["BNo"]);
                    billNumber = Convert.ToString(bnum + 1);
                }
            }
            else
            {
                billNumber = "1";
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void GridToStock()
        {
            foreach (DataRow dg in dt.Rows)
            {
                string gridId = dg[0].ToString();
                string gridName = dg[1].ToString();
                string gridRate = dg[2].ToString();
                string gridGstPercent = dg[3].ToString();
                string gridGstAmount = dg[4].ToString();
                string gridQuantity = dg[5].ToString();
                string gridScale = dg[6].ToString();
                string gridSQuantity = dg[7].ToString();
                string gridTotal = dg[8].ToString();
                string gridProfitPercent = dg[9].ToString();
                string gridSPrice = dg[10].ToString();
                string gridSGstPercent = dg[11].ToString();
                string gridSGstAmount = dg[12].ToString();
                newSNo();
                cc.OpenConnection();
                cc.ExecuteQuery("insert into Stock(Sno,Bno,itemid,iname,rate,quantity,squantity,purchaseQty,total,profit_percent,sprice,gtotal,gst_percent,gst_amount,scale,salegstpercent,salegstamount,BillType) values('" + Sno + "', '" + billNumber + "', '" + gridId + "', '" + gridName + "','" + gridRate + "','" + gridQuantity + "','" + gridSQuantity + "','" + gridQuantity + "','" + gridTotal + "','" + gridProfitPercent + "','" + gridSPrice + "','" + txtBlkNetAmount.Text + "','" + gridGstPercent + "','" + gridGstAmount + "','" + gridScale + "','" + gridSGstPercent + "','" + gridSGstAmount + "','" + BillType + "')");
                cc.CloseReader();
                cc.OpenConnection();
            }
        }

        private void btn_Update_Click(object sender, RoutedEventArgs e)
        {
            if (txtVendorId.Text == "" || txtVendorPhone.Text == "" || txtVendorPhone.Text == "")
            {
                MessageBox.Show("Add/Choose The Vendor..!");
                vendorNameComboBox.Focus();
            }

            else if (selctedPaymentType == "Payment Mode")
            {
                MessageBox.Show("Select A Payment Method");
                comboBoxPaymentVendor.Focus();
            }
            else
            {
                Netgst = Convert.ToDouble(dt.Compute("Sum(Gst_Amount)", ""));
                cc.OpenConnection();
                string formattedDate = datePicker.Text;
                vendorName = vendorNameComboBox.SelectedItem.ToString();
                int vndrId = Convert.ToInt32(txtVendorId.Text);
                if(checkBoxGST.IsChecked==true)
                {
                    BillType = "GST";
                }
                else
                {
                    BillType = "NON_GST";
                }
                newBillNo();
                if (selctedPaymentType == "By Cash")
                {
                    GridToStock();
                    cc.TransactionDetails("insert into TransactionDetails(Vid,Vname,Bno,Gtotal,Payment,Vphone,Vplace,PurchaseDate,CreditAmount,DebitAmount,CashPaid,PaymentType,GstAmount,BillType) values(@Vid,@Vname,@Bno,@Gtotal,@Payment,@Vphone,@Vplace,@PurchaseDate,@CreditAmount,@DebitAmount,@CashPaid,@PaymentType,@GstAmount,@BillType)", "Vid", vndrId, "Vname", vendorName, "Bno", Convert.ToInt32(billNumber), "Gtotal", txtBlkNetAmount.Text, "Payment", txtBlkNetAmount.Text, "Vphone", txtVendorPhone.Text, "Vplace", txtVendorPlace.Text, "PurchaseDate", formattedDate, "CreditAmount", Convert.ToString(0), "DebitAmount", Convert.ToString(0), "CashPaid", txtBlkNetAmount.Text, "PaymentType", selctedPaymentType, "GstAmount", Netgst.ToString(), "BillType", BillType);
                    DoAfterPaymentSelection();
                }
                else if (selctedPaymentType == "By Credit")
                {
                    GridToStock();
                    cc.TransactionDetails("insert into TransactionDetails(Vid,Vname,Bno,Gtotal,Payment,Vphone,Vplace,PurchaseDate,CreditAmount,DebitAmount,CashPaid,PaymentType,GstAmount,BillType) values(@Vid,@Vname,@Bno,@Gtotal,@Payment,@Vphone,@Vplace,@PurchaseDate,@CreditAmount,@DebitAmount,@CashPaid,@PaymentType,@GstAmount,@BillType)", "Vid", vndrId, "Vname", vendorName, "Bno", Convert.ToInt32(billNumber), "Gtotal", txtBlkNetAmount.Text, "Payment", Convert.ToString(0), "Vphone", txtVendorPhone.Text, "Vplace", txtVendorPlace.Text, "PurchaseDate", formattedDate, "CreditAmount", txtBlkNetAmount.Text, "DebitAmount", Convert.ToString(0), "CashPaid", Convert.ToString(0), "PaymentType", selctedPaymentType, "GstAmount", Netgst.ToString(), "BillType", BillType);
                    DoAfterPaymentSelection();
                }
                else if (selctedPaymentType == "Multi Payment")
                {
                    string lastContent = balance.Content.ToString();
                    if (lastContent == "Credit")
                    {
                        GridToStock();
                        cc.TransactionDetails("insert into TransactionDetails(Vid,Vname,Bno,Gtotal,Payment,Vphone,Vplace,PurchaseDate,CreditAmount,DebitAmount,CashPaid,PaymentType,GstAmount,BillType) values(@Vid,@Vname,@Bno,@Gtotal,@Payment,@Vphone,@Vplace,@PurchaseDate,@CreditAmount,@DebitAmount,@CashPaid,@PaymentType,@GstAmount,@BillType)", "Vid", vndrId, "Vname", vendorName, "Bno", Convert.ToInt32(billNumber), "Gtotal", txtBlkNetAmount.Text, "Payment", txtBox_CashPaid.Text, "Vphone", txtVendorPhone.Text, "Vplace", txtVendorPlace.Text, "PurchaseDate", formattedDate, "CreditAmount", Convert.ToString(Label_BalanceAmnt.Content), "DebitAmount", Convert.ToString(0), "CashPaid", txtBox_CashPaid.Text, "PaymentType", selctedPaymentType, "GstAmount", Netgst.ToString(), "BillType", BillType);
                        DoAfterPaymentSelection();
                    }
                    else
                    {
                        GridToStock();
                        cc.TransactionDetails("insert into TransactionDetails(Vid,Vname,Bno,Gtotal,Payment,Vphone,Vplace,PurchaseDate,CreditAmount,DebitAmount,CashPaid,PaymentType,GstAmount,BillType) values(@Vid,@Vname,@Bno,@Gtotal,@Payment,@Vphone,@Vplace,@PurchaseDate,@CreditAmount,@DebitAmount,@CashPaid,@PaymentType,@GstAmount,@BillType)", "Vid", vndrId, "Vname", vendorName, "Bno", Convert.ToInt32(billNumber), "Gtotal", txtBlkNetAmount.Text, "Payment", txtBox_CashPaid.Text, "Vphone", txtVendorPhone.Text, "Vplace", txtVendorPlace.Text, "PurchaseDate", formattedDate, "CreditAmount", Convert.ToString(0), "DebitAmount", Convert.ToString(Label_BalanceAmnt.Content), "CashPaid", txtBox_CashPaid.Text, "PaymentType", selctedPaymentType, "GstAmount", Netgst.ToString(), "BillType", BillType);
                        DoAfterPaymentSelection();
                    }
                }
                else
                {
                    MessageBox.Show("Select Any Payment Mode..!");
                    comboBoxPaymentVendor.Focus();
                }
            }
        }

        private void ReadItemId()
        {
            cc.OpenConnection();
            cc.DataReader("select ConfigValue from ConfigTable where ConfigId = 5 ");
            while (cc.reader.Read())
            {
                genItemId = Convert.ToInt32(cc.reader["ConfigValue"]);
            }
            cc.CloseReader();
            cc.CloseConnection();
            readItemId = genItemId;
        }

        private void writeItemId()
        {
            cc.OpenConnection();
            cc.ExecuteQuery("update ConfigTable set ConfigValue=" + genItemId + " where ConfigId = 5 ");
            cc.CloseConnection();
        }

        private void DoAfterPaymentSelection()
        {
            writeItemId();
            string txt = "Stock Updated...\n\n";
            string msgtxt = "Do you want to generate barcode?";
            string title = "Stock";
            MessageBoxButton mbb = MessageBoxButton.OKCancel;
            MessageBoxResult result = MessageBox.Show(txt+msgtxt, title, mbb);
            switch (result)
            {
                case MessageBoxResult.OK:
                    bno = Convert.ToInt32(billNumber);
                    BarCodePage bcp = new BarCodePage();
                    this.NavigationService.Navigate(bcp);
                    break;

                case MessageBoxResult.Cancel:
                    break;
            }
            clearTable();
        }

        private void txtItemRate_TextChanged(object sender, TextChangedEventArgs e)
        {
            calculateTotalPrice();
        }
        private void txtGstPercent_TextChanged(object sender, TextChangedEventArgs e)
        {
            calculateGstAmount();
            if (txtGstPercent.Text == "")
            {
                txtGstAmount.Text = "";
            }
        }

        private void calculateGstAmount()
        {
            double r;
            double g;
            if (double.TryParse(txtItemRate.Text, out r) && double.TryParse(txtGstPercent.Text, out g))
            {
                txtGstAmount.Text = ((r * g) / 100).ToString();
            }
        }
        private void calculateTotalPrice()
        {
            double r;
            double q;
            double g;
            if (txtGstAmount.Text == "")
            {
                txtGstPercent.Text = "0";
                txtGstAmount.Text = "0";
            }
                if (double.TryParse(txtItemRate.Text, out r) && double.TryParse(txtItemQty.Text, out q) && double.TryParse(txtGstAmount.Text, out g))
            {
                txtTotalPrice.Text = ((r+g) * q).ToString();
            }
        }

        private void txtItemQty_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtItemSQty.Text = txtItemQty.Text;
            calculateTotalPrice();
            if (txtItemQty.Text == "")
            {
                txtTotalPrice.Text = "";
            }
        }

        private void CalculateSellPrice_And_Profit()
        {
            double rate;
            double gstamount;
            double profit;
            double sellp;
            double sellpResult;
            double profitResult;
            if (txtItemProfit.IsFocused && double.TryParse(txtItemRate.Text, out rate) && double.TryParse(txtGstAmount.Text, out gstamount) && double.TryParse(txtItemProfit.Text, out profit))
            {
                sellpResult = (rate + gstamount) + ((((rate+ gstamount) * profit) / 100));
                txtSellingPrice.Text = Convert.ToString(sellpResult);
            }
            else if (txtSellingPrice.IsFocused && double.TryParse(txtItemRate.Text, out rate) && double.TryParse(txtGstAmount.Text, out gstamount) && double.TryParse(txtSellingPrice.Text, out sellp))
            {
                profitResult = (((sellp - (rate + gstamount)) / (rate + gstamount)) * 100);
                txtItemProfit.Text = Convert.ToString(profitResult);
            }
        }

        private void txtItemProfit_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateSellPrice_And_Profit();
            if (txtItemProfit.Text == "")
            {
                txtSellingPrice.Text = "";
            }

        }

        private void txtSellingPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateSellPrice_And_Profit();
            if (txtSellingPrice.Text == "")
            {
                txtItemProfit.Text = "";
            }
        }

        private void calculateBalanceAmount()
        {
            double netAmnt = Convert.ToDouble(txtBlkNetAmount.Text);
            double cashPaid;
            if (double.TryParse(txtBox_CashPaid.Text, out cashPaid))
            {
                if (netAmnt >= cashPaid)
                {
                    balance.Content = "Credit";
                    Label_BalanceAmnt.Content = (netAmnt - cashPaid).ToString();
                }
                else
                {
                    balance.Content = "Debit";
                    Label_BalanceAmnt.Content = ((netAmnt - cashPaid) * -1);
                }
            }
        }

        private void comboBoxPaymentVendor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selctedPaymentType = comboBoxPaymentVendor.SelectedItem.ToString();
            if (selctedPaymentType == "Multi Payment")
            {
                ShowBalance();
            }
            else if (selctedPaymentType == "By Credit")
            {
                HideBalance();
            }
            else if (selctedPaymentType == "By Cash")
            {
                HideBalance();
            }
        }

        private void ShowBalance()
        {
            cashPay.Visibility = Visibility.Visible;
            txtBox_CashPaid.Visibility = Visibility.Visible;
            balance.Visibility = Visibility.Visible;
            Label_BalanceAmnt.Visibility = Visibility.Visible;
        }

        private void txtItemRate_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var txtItemRate = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void txtItemQty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var txtItemQty = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void txtItemSQty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var txtItemSQty = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void txtItemProfit_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var txtItemProfit = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void txtSellingPrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var txtSellingPrice = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void txtGstPercent_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var txtGstPercent = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void HideBalance()
        {
            cashPay.Visibility = Visibility.Hidden;
            txtBox_CashPaid.Visibility = Visibility.Hidden;
            balance.Visibility = Visibility.Hidden;
            Label_BalanceAmnt.Visibility = Visibility.Hidden;
        }

        private class FilterViewModel
        {
            public IEnumerable<string> DataSource { get; set; }
            public FilterViewModel()
            {
                DataSource = vendorList;
            }
        }

        private void vendorNameComboBox_GotFocus(object sender, RoutedEventArgs e)
        {
            vendorNameComboBox.IsDropDownOpen = true;
        }

        private void dataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void calculateSaleGstAmount()
        {
            double r;
            double g;
            if (double.TryParse(txtSellingPrice.Text, out r) && double.TryParse(txtSaleGstPercent.Text, out g))
            {
                txtSaleGstAmount.Text = ((r * g) / 100).ToString();
            }
        }

        private void txtSaleGstPercent_TextChanged(object sender, TextChangedEventArgs e)
        {
            calculateSaleGstAmount();
            if (txtSaleGstPercent.Text == "")
            {
                txtSaleGstAmount.Text = "";
            }
        }

        private void txtSaleGstPercent_GotFocus(object sender, RoutedEventArgs e)
        {
            txtSaleGstPercent.Text = "";
            txtSaleGstAmount.Text = "";
        }

        private void txtSaleGstPercent_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var txtGstPercent = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void checkBoxGST_Checked(object sender, RoutedEventArgs e)
        {
            Gst();
            dt.Clear();
            txtBlkNetAmount.Text = "";
        }

        private void Gst()
        {
            txtGstPercent.Visibility = Visibility.Visible;
            txtGstAmount.Visibility = Visibility.Visible;
            label_GP.Visibility = Visibility.Visible;
            label_GA.Visibility = Visibility.Visible;
            txtItemQty.Margin = new Thickness(366, 221, 0, 0);
            label_Quantity.Margin = new Thickness(363, 198, 0, 0);
            comboBoxScale.Margin = new Thickness(427, 221, 0, 0);
            txtItemSQty.Margin = new Thickness(487, 221, 0, 0);
            label_Sqty.Margin = new Thickness(482, 198, 0, 0);
            txtTotalPrice.Margin = new Thickness(548, 221, 0, 0);
            label_Total.Margin = new Thickness(542, 198, 0, 0);
            txtItemProfit.Margin = new Thickness(609, 221, 0, 0);
            label_Profit.Margin = new Thickness(604, 198, 0, 0);
            txtSellingPrice.Margin = new Thickness(670, 221, 0, 0);
            label_Sellp.Margin = new Thickness(664, 198, 0, 0);
            txtSaleGstPercent.Margin = new Thickness(731, 221, 0, 0);
            label_SGP.Margin = new Thickness(726, 198, 0, 0);
            txtSaleGstAmount.Margin = new Thickness(792, 221, 0, 0);
            label_SGA.Margin = new Thickness(786, 198, 0, 0);
        }

        private void NonGst()
        {
            txtGstPercent.Visibility = Visibility.Collapsed;
            txtGstAmount.Visibility = Visibility.Collapsed;
            label_GP.Visibility = Visibility.Collapsed;
            label_GA.Visibility = Visibility.Collapsed;
            txtItemQty.Margin = new Thickness(244, 221, 0, 0);
            label_Quantity.Margin = new Thickness(240, 198, 0, 0);
            comboBoxScale.Margin = new Thickness(305, 221, 0, 0);
            txtItemSQty.Margin = new Thickness(365, 221, 0, 0);
            label_Sqty.Margin = new Thickness(361, 198, 0, 0);
            txtTotalPrice.Margin = new Thickness(426, 221, 0, 0);
            label_Total.Margin = new Thickness(421, 198, 0, 0);
            txtItemProfit.Margin = new Thickness(487, 221, 0, 0);
            label_Profit.Margin = new Thickness(484, 198, 0, 0);
            txtSellingPrice.Margin = new Thickness(548, 221, 0, 0);
            label_Sellp.Margin = new Thickness(543, 198, 0, 0);
            txtSaleGstPercent.Margin = new Thickness(609, 221, 0, 0);
            label_SGP.Margin = new Thickness(605, 198, 0, 0);
            txtSaleGstAmount.Margin = new Thickness(670, 221, 0, 0);
            label_SGA.Margin = new Thickness(667, 198, 0, 0);
        }

        private void checkBoxGST_Unchecked(object sender, RoutedEventArgs e)
        {
            NonGst();
            dt.Clear();
            txtBlkNetAmount.Text = "";
        }

        private void txtBarcode_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                txtItemName.Focus();
            }
        }

        private void txtItemName_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (txtItemName.Text == "")
                {
                    MessageBox.Show("Enter an 'Item Name'");
                    txtItemName.Focus();
                }
                else
                {
                    txtItemRate.Focus();
                }
            }
        }

        private void txtItemRate_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if(checkBoxGST.IsChecked==true)
                {
                    if (txtItemRate.Text == "")
                    {
                        MessageBox.Show("Enter the 'Item Rate'");
                        txtItemRate.Focus();
                    }
                    else
                    {
                        txtGstPercent.Focus();
                    }
                }
                else
                {
                    if (txtItemRate.Text == "")
                    {
                        MessageBox.Show("Enter the 'Item Rate'");
                        txtItemRate.Focus();
                    }
                    else
                    {
                        txtItemQty.Focus();
                    }
                }
            }
        }

        private void txtGstPercent_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (txtGstPercent.Text == "")
                {
                    MessageBox.Show("Enter the 'GST %'");
                    txtGstPercent.Focus();
                }
                else
                {
                    txtItemQty.Focus();
                }
            }
        }

        private void txtGstAmount_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                txtItemQty.Focus();
            }
        }

        private void txtItemQty_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (txtItemQty.Text == "")
                {
                    MessageBox.Show("Enter the 'Quantity'");
                    txtItemQty.Focus();
                }
                else
                {
                    txtItemSQty.Focus();
                }
            }
        }

        private void txtItemSQty_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                txtItemProfit.Focus();
            }
        }

        private void txtTotalPrice_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                txtItemProfit.Focus();
            }
        }

        private void txtItemProfit_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                txtSaleGstPercent.Focus();
            }
        }

        private void txtBox_CashPaid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                btn_Update.Focus();
            }
        }

        private void txtBox_CashPaid_GotFocus(object sender, RoutedEventArgs e)
        {
            if(txtBlkNetAmount.Text=="")
            {
                MessageBox.Show("Do some purchase");
                txtItemName.Focus();
            }
        }

        private void vendorNameComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            CollectionView itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(vendorNameComboBox.ItemsSource);

            itemsViewOriginal.Filter = ((a) =>
            {
                if (String.IsNullOrEmpty(vendorNameComboBox.Text)) return true;
                else
                {
                    if (((string)a.ToString().ToUpper()).Contains(vendorNameComboBox.Text.ToUpper())) return true;
                    else return false;
                }
            });

            itemsViewOriginal.Refresh();
        }

        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                try
                {
                    int selectedIndex = dataGrid.SelectedIndex;
                    if (dt.Rows.Count == 1)
                    {
                        dt.Clear();
                        txtBlkNetAmount.Text = "";
                        ReadItemId();
                    }
                    else if (dt.Rows.Count > 1)
                    {
                        dt.Rows.RemoveAt(selectedIndex);
                        dataGrid.ItemsSource = null;
                        dataGrid.ItemsSource = dt.DefaultView;
                        txtBlkNetAmount.Text = (dt.Compute("Sum(Total)", "")).ToString();
                    }
                    else
                    {
                        MessageBox.Show("No items to delete");
                    }
                }
                catch
                {
                    MessageBox.Show("Empty Row");
                }
            }
        }

        private void btn_Clear_Click(object sender, RoutedEventArgs e)
        {
            dt.Clear();
            txtBlkNetAmount.Text = "";
            ReadItemId();
        }

        private void txtBox_CashPaid_TextChanged(object sender, TextChangedEventArgs e)
        {
            calculateBalanceAmount();
        }

        private void btn_Purchase_History_Click(object sender, RoutedEventArgs e)
        {
            cc.OpenConnection();
            if (cc.BillPreview1("select COUNT(*) from Stock", billExistValue) > 0)
            {
                cc.CloseConnection();
                PurchaseHistory ph = new PurchaseHistory();
                this.NavigationService.Navigate(ph);
            }
            else
            {
                MessageBox.Show("No Bills to Show");
            }
        }

        private void backbutton_Click(object sender, RoutedEventArgs e)
        {
            dt.Clear();
            txtBlkNetAmount.Text = "";
            txtItemName.Focus();
            cc.CloseConnection();
            txtVendorId.Text = "";
            txtVendorPhone.Text = "";
            txtVendorPlace.Text = "";
            SPpage sp = new SPpage();
            this.NavigationService.Navigate(sp);
        }

        private void btn_New_Click(object sender, RoutedEventArgs e)
        {
            vendorNameComboBox.SelectedItem = null;
            txtVendorId.Text = "";
            txtVendorPhone.Text = "";
            txtVendorPlace.Text = "";
            SaveVendor sv = new SaveVendor();
            sv.InputChanged += OnDialogInputChanged;
            sv.Show();
        }

        private void OnDialogInputChanged(object sender, SaveVendor.DialogInputEventArgs e)
        {
            var sv = sender as SaveVendor;
            string vndrName = sv.txt_Vendor_Name.Text;
            vendorList.Add(vndrName);
            vendorList.Sort();
            vendorNameComboBox.SelectedItem = vndrName;
        }

        private void vendorNameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cc.OpenConnection();
            cc.DataReader("select Vid,Vphone,Vplace from VendorDetails where Vname = '" + vendorNameComboBox.SelectedItem + "' ");
            while (cc.reader.Read())
            {
                txtVendorId.Text = cc.reader["Vid"].ToString();
                txtVendorPhone.Text = cc.reader["Vphone"].ToString();
                txtVendorPlace.Text = cc.reader["Vplace"].ToString();
            }
            cc.CloseReader();
            cc.CloseConnection();
            txtItemName.Focus();
        }

    }
}
