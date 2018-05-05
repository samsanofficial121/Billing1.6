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
//using WPF_AutoCompleteComboBox;

namespace Billing
{
    /// <summary>
    /// Interaction logic for PurchasePage.xaml
    /// </summary>
    public partial class PurchasePage : Page
    {
        int count = 0;
        public static string readSno, selctedPaymentType, incrementPbno;
        public static int genSno, readPurchaseBno, purchaseBno;
        public static string genVendorId;
        public static int vendorId, bill;
        public static string vendorName;
        int genItemId, grantTotal, readItemId;
        public static List<string> vendorList = new List<string>();
        public static List<string> serialNoList = new List<string>();
        public static List<string> billNoList = new List<string>();
        public static List<string> itemIdList = new List<string>();
        ConnectionClass cc = new ConnectionClass();

        public PurchasePage()
        {
            InitializeComponent();
            FilterViewModel vm = new FilterViewModel();
            this.DataContext = vm;
            clearLists();
            clearTable();
            vendorNameComboBox.Focus();
            txtGstPercent.Text = "0";
            lblGstAmount.Content = "0";
            comboBoxPaymentVendor.Items.Add("By Cash");
            comboBoxPaymentVendor.Items.Add("By Credit");
            comboBoxPaymentVendor.Items.Add("Multi Payment");
            HideBalance();
            VendorAction();
            readSerialNo();
            generateItemId();
            tableDisplay();
        }

        private void clearLists()
        {
            vendorList.Clear();
            serialNoList.Clear();
            billNoList.Clear();
            itemIdList.Clear();
        }

        private void readSerialNo()
        {
            string serialNoPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Config\Config.txt");
            string serialNoValue = File.ReadAllText(serialNoPath);
            if (serialNoValue.Contains("SerialNumber"))
            {
                string[] val = serialNoValue.Substring(serialNoValue.IndexOf("SerialNumber") + 14).Split(Convert.ToChar("'"));
                serialNoList.Add(val[0].ToString());
                foreach (var number in serialNoList)
                {
                    readSno = number.ToString();
                }
            }
        }

        private void clearTable()
        {
            cc.OpenConnection();
            cc.DataReader("delete * from Purchase");
            cc.CloseReader();
            lblGrantTotal.Content = "";
            count = 0;
            tableDisplay();
            txtItemName.Focus();
            cc.CloseConnection();
            txtVendorId.Text = "";
            txtVendorPhone.Text = "";
            txtVendorPlace.Text = "";
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
            txtItemName.Text = "";
            txtItemRate.Text = "";
            txtItemQty.Text = "";
            lblTotalPrice.Content = "";
            txtSellingPrice.Text = "";
            txtGstPercent.Text = "";
            lblGstAmount.Content = "";
            txtItemProfit.Text = "";
            txtItemSQty.Text = "";
            txtBox_CashPaid.Text = "";
            balance.Content = "";
        }

        private void tableDisplay()
        {
            cc.OpenConnection();
            cc.DataGridDisplay("select * from Purchase");
            dataGrid.ItemsSource = cc.dt.AsDataView();
            dataGrid.Visibility = System.Windows.Visibility.Visible;
            cc.CloseConnection();
        }

        //Add Button
        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            if (txtItemName.Text == "" || txtItemRate.Text == "" || txtItemQty.Text == "" || txtItemSQty.Text == "" || txtItemProfit.Text == "" || txtSellingPrice.Text == "" || txtGstPercent.Text == "")
            {
                MessageBox.Show("Enter The Details");
                txtItemName.Focus();
            }
            else
            {
                cc.OpenConnection();
                cc.DataReader("insert into Purchase(Sno,Item_ID,Item_Name,Rate,Quantity,SQuantity,Total,ProfitPercent,Selling_Price,Gst_Percent,Gst_Amount) values(' " + readSno + " ' , '" + genItemId + "','" + txtItemName.Text + "','" + txtItemRate.Text + "','" + txtItemQty.Text + "','" + txtItemSQty.Text + "','" + lblTotalPrice.Content + "','" + txtItemProfit.Text + "','" + txtSellingPrice.Text + "','" + txtGstPercent.Text + "','" + lblGstAmount.Content + "')");
                cc.CloseReader();
                tableDisplay();
                count = count + Convert.ToInt32(lblTotalPrice.Content);
                lblGrantTotal.Content = count;
                grantTotal = Convert.ToInt32(lblGrantTotal.Content);
                fieldsClear();
                genSno = Convert.ToInt32(readSno);
                genSno += 1;
                genItemId += 1;
                readSno = Convert.ToString(genSno);
                txtItemName.Focus();
                cc.CloseConnection();
            }
        }

        //Update Button
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
                cc.OpenConnection();
                var time = DateTime.Now;
                string formattedDate = time.ToString("dd-MMM-yy");
                readPurchaseBillNo();
                vendorName = vendorNameComboBox.SelectedItem.ToString();
                int vndrId = Convert.ToInt32(txtVendorId.Text);
                if (selctedPaymentType == "By Cash")
                {
                    cc.DoPurchase("insert into Stock(Bno,itemid,iname,rate,quantity,squantity,purchaseQty,total,profit_percent,sprice,gtotal,gst_percent,gst_amount) select ?,Item_Id,Item_Name,Rate,Quantity,SQuantity,Quantity,Rate*Quantity,ProfitPercent,Selling_Price,?,Gst_Percent,Gst_Amount from Purchase", "Bno", readPurchaseBno, "gtotal", Convert.ToInt32(lblGrantTotal.Content));
                    cc.TransactionDetails("insert into TransactionDetails(Vid,Vname,Bno,Gtotal,Payment,Vphone,Vplace,PurchaseDate,CreditAmount,DebitAmount,CashPaid,PaymentType) values(?,?,?,?,?,?,?,?,?,?,?,?)", "Vid", vndrId, "Vname", vendorName, "Bno", readPurchaseBno, "Gtotal", Convert.ToInt32(lblGrantTotal.Content), "Payment", Convert.ToInt32(lblGrantTotal.Content), "Vphone", txtVendorPhone.Text, "Vplace", txtVendorPlace.Text, "PurchaseDate", formattedDate, "CreditAmount", Convert.ToString(0), "DebitAmount", Convert.ToString(0), "CashPaid", Convert.ToInt32(lblGrantTotal.Content), "PaymentType", selctedPaymentType);
                    DoAfterPaymentSelection();
                }
                else if (selctedPaymentType == "By Credit")
                {
                    cc.DoPurchase("insert into Stock(Bno,itemid,iname,rate,quantity,squantity,purchaseQty,total,profit_percent,sprice,gtotal,gst_percent,gst_amount) select ?,Item_Id,Item_Name,Rate,Quantity,SQuantity,Quantity,Rate*Quantity,ProfitPercent,Selling_Price,?,Gst_Percent,Gst_Amount from Purchase", "Bno", readPurchaseBno, "gtotal", Convert.ToInt32(lblGrantTotal.Content));
                    cc.TransactionDetails("insert into TransactionDetails(Vid,Vname,Bno,Gtotal,Payment,Vphone,Vplace,PurchaseDate,CreditAmount,DebitAmount,CashPaid,PaymentType) values(?,?,?,?,?,?,?,?,?,?,?,?)", "Vid", vndrId, "Vname", vendorName, "Bno", readPurchaseBno, "Gtotal", Convert.ToInt32(lblGrantTotal.Content), "Payment", Convert.ToInt32(0), "Vphone", txtVendorPhone.Text, "Vplace", txtVendorPlace.Text, "PurchaseDate", formattedDate, "CreditAmount", Convert.ToString(lblGrantTotal.Content), "DebitAmount", Convert.ToString(0), "CashPaid", Convert.ToInt32(0), "PaymentType", selctedPaymentType);
                    DoAfterPaymentSelection();
                }
                else if (selctedPaymentType == "Multi Payment")
                {
                    string lastContent = balance.Content.ToString();
                    if (lastContent == "Credit")
                    {
                        cc.DoPurchase("insert into Stock(Bno,itemid,iname,rate,quantity,squantity,purchaseQty,total,profit_percent,sprice,gtotal,gst_percent,gst_amount) select ?,Item_Id,Item_Name,Rate,Quantity,SQuantity,Quantity,Rate*Quantity,ProfitPercent,Selling_Price,?,Gst_Percent,Gst_Amount from Purchase", "Bno", readPurchaseBno, "gtotal", Convert.ToInt32(lblGrantTotal.Content));
                        cc.TransactionDetails("insert into TransactionDetails(Vid,Vname,Bno,Gtotal,Payment,Vphone,Vplace,PurchaseDate,CreditAmount,DebitAmount,CashPaid,PaymentType) values(?,?,?,?,?,?,?,?,?,?,?,?)", "Vid", vndrId, "Vname", vendorName, "Bno", readPurchaseBno, "Gtotal", Convert.ToInt32(lblGrantTotal.Content), "Payment", Convert.ToInt32(txtBox_CashPaid.Text), "Vphone", txtVendorPhone.Text, "Vplace", txtVendorPlace.Text, "PurchaseDate", formattedDate, "CreditAmount", Convert.ToString(Label_BalanceAmnt.Content), "DebitAmount", Convert.ToString(0), "CashPaid", Convert.ToInt32(txtBox_CashPaid.Text), "PaymentType", selctedPaymentType);
                        DoAfterPaymentSelection();
                    }
                    else
                    {
                        cc.DoPurchase("insert into Stock(Bno,itemid,iname,rate,quantity,squantity,purchaseQty,total,profit_percent,sprice,gtotal,gst_percent,gst_amount) select ?,Item_Id,Item_Name,Rate,Quantity,SQuantity,Quantity,Rate*Quantity,ProfitPercent,Selling_Price,?,Gst_Percent,Gst_Amount from Purchase", "Bno", readPurchaseBno, "gtotal", Convert.ToInt32(lblGrantTotal.Content));
                        cc.TransactionDetails("insert into TransactionDetails(Vid,Vname,Bno,Gtotal,Payment,Vphone,Vplace,PurchaseDate,CreditAmount,DebitAmount,CashPaid,PaymentType) values(?,?,?,?,?,?,?,?,?,?,?,?)", "Vid", vndrId, "Vname", vendorName, "Bno", readPurchaseBno, "Gtotal", Convert.ToInt32(lblGrantTotal.Content), "Payment", Convert.ToInt32(txtBox_CashPaid.Text), "Vphone", txtVendorPhone.Text, "Vplace", txtVendorPlace.Text, "PurchaseDate", formattedDate, "CreditAmount", Convert.ToString(0), "DebitAmount", Convert.ToString(Label_BalanceAmnt.Content), "CashPaid", Convert.ToInt32(txtBox_CashPaid.Text), "PaymentType", selctedPaymentType);
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

        private void readPurchaseBillNo()
        {
            string purchaseBillNoPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Config\Config.txt");
            string purchaseBillNoValue = File.ReadAllText(purchaseBillNoPath);
            if (purchaseBillNoValue.Contains("PurchaseBillNumber"))
            {
                string[] val = purchaseBillNoValue.Substring(purchaseBillNoValue.IndexOf("PurchaseBillNumber") + 20).Split(Convert.ToChar("'"));
                billNoList.Add(val[0].ToString());
                foreach (var number in billNoList)
                {
                    readPurchaseBno = Convert.ToInt32(number);
                    purchaseBno = readPurchaseBno;
                }
            }
        }

        private void DoAfterPaymentSelection()
        {
            MessageBox.Show("Stock Updated");
            incrementPbno = Convert.ToString(readPurchaseBno + 1);
            writePurchaseBillNo();
            writeItemId();
            string msgtxt = "Do you want to generate barcode?";
            string title = "Barcode";
            MessageBoxButton mbb = MessageBoxButton.OKCancel;
            MessageBoxResult result = MessageBox.Show(msgtxt, title, mbb);
            switch (result)
            {
                case MessageBoxResult.OK:
                    BarCodePage bcp = new BarCodePage();
                    this.NavigationService.Navigate(bcp);
                    break;

                case MessageBoxResult.Cancel:
                    break;
            }
            cc.OpenConnection();
            cc.DataReader("delete * from Purchase");
            cc.CloseReader();
            lblGrantTotal.Content = "";
            count = 0;
            tableDisplay();
            txtItemName.Focus();
            cc.CloseConnection();
            txtVendorId.Text = "";
            txtVendorPhone.Text = "";
            txtVendorPlace.Text = "";
            vendorNameComboBox.SelectedItem = null;
            readSno = Convert.ToString(1);
        }

        private void writeItemId()
        {
            string configFilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Config\Config.txt");
            string[] configValues = File.ReadAllLines(configFilePath);
            for (int i = 0; i < configValues.Length; i++)
            {
                if (configValues[i] == "BarcodeId '" + readItemId + "'")
                {
                    configValues[i] = "BarcodeId '" + genItemId + "'";
                }
            }
            File.WriteAllLines(configFilePath, configValues);
        }

        private void writePurchaseBillNo()
        {
            string configFilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Config\Config.txt");
            string[] configValues = File.ReadAllLines(configFilePath);
            for (int i = 0; i < configValues.Length; i++)
            {
                if (configValues[i] == "PurchaseBillNumber '" + purchaseBno + "'")
                {
                    configValues[i] = "PurchaseBillNumber '" + incrementPbno + "'";
                }
            }
            File.WriteAllLines(configFilePath, configValues);
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
                lblGstAmount.Content = "";
            }
        }

        private void calculateGstAmount()
        {
            double r;
            double g;
            if (double.TryParse(txtSellingPrice.Text, out r) && double.TryParse(txtGstPercent.Text, out g))
            {
                lblGstAmount.Content = ((r * g) / 100).ToString();
            }
        }
        private void calculateTotalPrice()
        {
            int a;
            int b;
            if (Int32.TryParse(txtItemRate.Text, out a) && Int32.TryParse(txtItemQty.Text, out b))
            {
                lblTotalPrice.Content = (a * b).ToString();
            }
        }

        private void txtItemQty_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtItemSQty.Text = txtItemQty.Text;
            calculateTotalPrice();
            if (txtItemQty.Text == "")
            {
                lblTotalPrice.Content = "";
            }
        }

        private void CalculateSellPrice_And_Profit()
        {
            float rate;
            float profit;
            float sellp;
            float sellpResult;
            float profitResult;
            if (txtItemProfit.IsFocused && float.TryParse(txtItemRate.Text, out rate) && float.TryParse(txtItemProfit.Text, out profit))
            {
                sellpResult = rate + (((rate * profit) / 100));
                txtSellingPrice.Text = Convert.ToString(sellpResult);
            }
            else if (txtSellingPrice.IsFocused && float.TryParse(txtItemRate.Text, out rate) && float.TryParse(txtSellingPrice.Text, out sellp))
            {
                profitResult = (((sellp - rate) / rate) * 100);
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
            int netAmnt = Convert.ToInt32(lblGrantTotal.Content);
            int cashPaid;
            if (Int32.TryParse(txtBox_CashPaid.Text, out cashPaid))
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

        private void txtSellingPrice_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (txtItemName.Text == "" || txtItemRate.Text == "" || txtItemQty.Text == "" || txtItemSQty.Text == "" || txtItemProfit.Text == "" || txtSellingPrice.Text == "" || txtGstPercent.Text == "")
                {
                    MessageBox.Show("Enter The Details");
                    txtItemName.Focus();
                }
                else
                {
                    cc.OpenConnection();
                    cc.DataReader("insert into Purchase(Sno,Item_ID,Item_Name,Rate,Quantity,SQuantity,Total,ProfitPercent,Selling_Price,Gst_Percent,Gst_Amount) values(' " + readSno + " ' , '" + genItemId + "','" + txtItemName.Text + "','" + txtItemRate.Text + "','" + txtItemQty.Text + "','" + txtItemSQty.Text + "','" + lblTotalPrice.Content + "','" + txtItemProfit.Text + "','" + txtSellingPrice.Text + "','" + txtGstPercent.Text + "','" + lblGstAmount.Content + "')");
                    cc.CloseReader();
                    tableDisplay();
                    count = count + Convert.ToInt32(lblTotalPrice.Content);
                    lblGrantTotal.Content = count;
                    grantTotal = Convert.ToInt32(lblGrantTotal.Content);
                    fieldsClear();
                    genSno = Convert.ToInt32(readSno);
                    genSno += 1;
                    genItemId += 1;
                    readSno = Convert.ToString(genSno);
                    cc.CloseConnection();
                    txtItemName.Focus();
                    txtGstPercent.Text = "0";
                    lblGstAmount.Content = "0";
                }
            }
        }

        private void txtGstPercent_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (txtItemName.Text == "" || txtItemRate.Text == "" || txtItemQty.Text == "" || txtItemSQty.Text == "" || txtItemProfit.Text == "" || txtSellingPrice.Text == "" || txtGstPercent.Text == "")
                {
                    MessageBox.Show("Enter The Details");
                    txtItemName.Focus();
                }
                else
                {
                    cc.OpenConnection();
                    cc.DataReader("insert into Purchase(Sno,Item_ID,Item_Name,Rate,Quantity,SQuantity,Total,ProfitPercent,Selling_Price,Gst_Percent,Gst_Amount) values(' " + readSno + " ' , '" + genItemId + "','" + txtItemName.Text + "','" + txtItemRate.Text + "','" + txtItemQty.Text + "','" + txtItemSQty.Text + "','" + lblTotalPrice.Content + "','" + txtItemProfit.Text + "','" + txtSellingPrice.Text + "','" + txtGstPercent.Text + "','" + lblGstAmount.Content + "')");
                    cc.CloseReader();
                    tableDisplay();
                    count = count + Convert.ToInt32(lblTotalPrice.Content);
                    lblGrantTotal.Content = count;
                    grantTotal = Convert.ToInt32(lblGrantTotal.Content);
                    fieldsClear();
                    genSno = Convert.ToInt32(readSno);
                    genSno += 1;
                    genItemId += 1;
                    readSno = Convert.ToString(genSno);
                    cc.CloseConnection();
                    txtItemName.Focus();
                    txtGstPercent.Text = "0";
                    lblGstAmount.Content = "0";
                }
            }
        }

        private void txtGstPercent_GotFocus(object sender, RoutedEventArgs e)
        {
            txtGstPercent.Text = "";
            lblGstAmount.Content = "";
        }

        private void txtItemProfit_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (txtItemName.Text == "" || txtItemRate.Text == "" || txtItemQty.Text == "" || txtItemSQty.Text == "" || txtItemProfit.Text == "" || txtSellingPrice.Text == "" || txtGstPercent.Text == "")
                {
                    MessageBox.Show("Enter The Details");
                    txtItemName.Focus();
                }
                else
                {
                    cc.OpenConnection();
                    cc.DataReader("insert into Purchase(Sno,Item_ID,Item_Name,Rate,Quantity,SQuantity,Total,ProfitPercent,Selling_Price,Gst_Percent,Gst_Amount) values(' " + readSno + " ' , '" + genItemId + "','" + txtItemName.Text + "','" + txtItemRate.Text + "','" + txtItemQty.Text + "','" + txtItemSQty.Text + "','" + lblTotalPrice.Content + "','" + txtItemProfit.Text + "','" + txtSellingPrice.Text + "','" + txtGstPercent.Text + "','" + lblGstAmount.Content + "')");
                    cc.CloseReader();
                    tableDisplay();
                    count = count + Convert.ToInt32(lblTotalPrice.Content);
                    lblGrantTotal.Content = count;
                    grantTotal = Convert.ToInt32(lblGrantTotal.Content);
                    fieldsClear();
                    genSno = Convert.ToInt32(readSno);
                    genSno += 1;
                    genItemId += 1;
                    readSno = Convert.ToString(genSno);
                    cc.CloseConnection();
                    txtItemName.Focus();
                    txtGstPercent.Text = "0";
                    lblGstAmount.Content = "0";
                }
            }
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

        private void txtBox_CashPaid_TextChanged(object sender, TextChangedEventArgs e)
        {
            calculateBalanceAmount();
        }

        private void btn_Purchase_History_Click(object sender, RoutedEventArgs e)
        {
            PurchaseHistory ph = new PurchaseHistory();
            this.NavigationService.Navigate(ph);
        }

        private void generateItemId()
        {
            string itemIdPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Config\Config.txt");
            string itemIdValue = File.ReadAllText(itemIdPath);
            if (itemIdValue.Contains("BarcodeId"))
            {
                string[] val = itemIdValue.Substring(itemIdValue.IndexOf("BarcodeId") + 11).Split(Convert.ToChar("'"));
                itemIdList.Add(val[0].ToString());
                foreach (var number in itemIdList)
                {
                    genItemId = Convert.ToInt32(number);
                    readItemId = genItemId;
                }
            }
        }


        private void backbutton_Click(object sender, RoutedEventArgs e)
        {
            cc.OpenConnection();
            cc.DataReader("delete * from Purchase");
            cc.CloseReader();
            lblGrantTotal.Content = "";
            count = 0;
            tableDisplay();
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
