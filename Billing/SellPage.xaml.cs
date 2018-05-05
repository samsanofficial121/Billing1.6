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
using System.Configuration;
using System.Data.OleDb;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;

namespace Billing
{
    /// <summary>
    /// Interaction logic for SellPage.xaml
    /// </summary>
    public partial class SellPage : Page
    {
        private Class1.DataAccess objDs;
        private Class1.BillTable objBillToAdd;
        public static int idCount = 0;
        public static string billNumber, billNo, storeBillNoPath, payCredit, payDebit, netAmount, selectedPayment, barcodeId, itemQuantity,paidCash,paymentType;
        int BillNumber;
        public static int convIntBill, stockQtyCheck, quantityNumber;
        string oldQty;
        public static string stockLeft, selectedItemGst, totalGst, itemId, selectedQuantity, billAmountString,getPhno;
        public static int billExist, bno, billExistValue, isExist;
        public static int billNumber1;
        public static double gstTotal, returnAmnt, newBillAmnt, toPay;
        public static List<string> BillList = new List<string>();
        public static List<string> billNoList = new List<string>();
        public static List<string> customerList = new List<string>();
        string[] QuantityArr = new string[] { };
        Id id = new Id();
        ConnectionClass cc = new ConnectionClass();

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

        public SellPage()
        {
            InitializeComponent();
            CustomerSuggestion();
            textBoxCustomer.TextChanged += new TextChangedEventHandler(textBoxCustomer_TextChanged);
            returnAmnt = 0;
            newBillAmnt = 0;
            HideBalance();
            comboBoxPayment.Items.Add("By Cash");
            comboBoxPayment.Items.Add("By Credit");
            comboBoxPayment.Items.Add("Multi Payment");
            DateTime today = DateTime.Today;
            lblDate.Content = today;
            txtItemId.Focus();
            ClearGrid();
            ReadBillNo();
            BillNumber = Convert.ToInt32(billNumber);
            txtBillNo.Text = BillNumber.ToString();
        }

        private void textBoxCustomer_TextChanged(object sender, TextChangedEventArgs e)
        {
            string typedString = textBoxCustomer.Text.ToUpper();
            List<string> autoList = new List<string>();
            autoList.Clear();
            foreach(string item in customerList)
            {
                if(!string.IsNullOrEmpty(textBoxCustomer.Text.ToUpper()))
                {
                    if(item.ToUpper().Contains(typedString))
                    {
                        autoList.Add(item);
                    }
                }
            }
            if(autoList.Count>0)
            {
                lbSuggestion.ItemsSource = autoList;
                lbSuggestion.Visibility = Visibility.Visible;
            }
            else if(textBoxCustomer.Text.Equals(""))
            {
                lbSuggestion.Visibility = Visibility.Visible;
                lbSuggestion.ItemsSource = customerList;
            }
            else
            {
                lbSuggestion.Visibility = Visibility.Collapsed;
                lbSuggestion.ItemsSource = null;
            }
        }

        private void lbSuggestion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbSuggestion.ItemsSource != null)
            {
                lbSuggestion.Visibility = Visibility.Collapsed;
                textBoxCustomer.TextChanged -= new TextChangedEventHandler(textBoxCustomer_TextChanged);
                if (lbSuggestion.SelectedIndex != -1)
                {
                    textBoxCustomer.Text = lbSuggestion.SelectedItem.ToString();
                    GetPhno();
                    textBoxCustomerPh.Text = getPhno;
                }
                textBoxCustomer.TextChanged += new TextChangedEventHandler(textBoxCustomer_TextChanged);
            }
        }

        private void CustomerSuggestion()
        {
            cc.OpenConnection();
            cc.DataReader("select distinct CustomerName from BillStock where CustomerName not like 'Cust_%'");
            while (cc.reader.Read())
            {
                customerList.Add(cc.reader["CustomerName"].ToString());
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void GetPhno()
        {
            cc.OpenConnection();
            cc.DataReader("select CustomerPhone from BillStock where CustomerName='"+lbSuggestion.SelectedItem+"'");
            while (cc.reader.Read())
            {
                getPhno = cc.reader["CustomerPhone"].ToString();
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            objDs = new Class1.DataAccess();
            Display();
        }
        private void dataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            objBillToAdd = dataGrid1.SelectedItem as Class1.BillTable;
        }

        private void dataGrid1_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                FrameworkElement element_Id = dataGrid1.Columns[0].GetCellContent(e.Row);
                if (element_Id.GetType() == typeof(TextBox))
                {
                    var id = ((TextBox)element_Id).Text;
                    objBillToAdd.Id = Convert.ToInt32(id);
                }
                FrameworkElement element_Name = dataGrid1.Columns[1].GetCellContent(e.Row);
                if (element_Name.GetType() == typeof(TextBox))
                {
                    var name = ((TextBox)element_Name).Text;
                    objBillToAdd.Name = name;
                }
                FrameworkElement element_Rate = dataGrid1.Columns[2].GetCellContent(e.Row);
                if (element_Rate.GetType() == typeof(TextBox))
                {
                    var rate = ((TextBox)element_Rate).Text;
                    objBillToAdd.Rate = Convert.ToInt32(rate);
                }
                FrameworkElement element_Quantity = dataGrid1.Columns[3].GetCellContent(e.Row);
                if (element_Quantity.GetType() == typeof(TextBox))
                {
                    var qty = ((TextBox)element_Quantity).Text;
                    objBillToAdd.Quantity = Convert.ToInt32(qty);
                }
                FrameworkElement element_Amount = dataGrid1.Columns[4].GetCellContent(e.Row);
                if (element_Amount.GetType() == typeof(TextBox))
                {
                    var amount = ((TextBox)element_Amount).Text;
                    objBillToAdd.Amount = Convert.ToInt32(amount);
                }
                FrameworkElement element_GSTPercent = dataGrid1.Columns[5].GetCellContent(e.Row);
                if (element_GSTPercent.GetType() == typeof(TextBox))
                {
                    var per = ((TextBox)element_GSTPercent).Text;
                    objBillToAdd.GSTPercent = per;
                }
                FrameworkElement element_AddedGST = dataGrid1.Columns[6].GetCellContent(e.Row);
                if (element_AddedGST.GetType() == typeof(TextBox))
                {
                    var gst = ((TextBox)element_AddedGST).Text;
                    objBillToAdd.AddedGST = Convert.ToInt32(gst);
                }
                objDs.InsertBill(objBillToAdd);
                Display();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void dataGrid1_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            //objDs.InsertBill(objBillToAdd);
        }
        private void StoreId()
        {
            id.StoreIId(idCount);
        }
        private void QtySelect()
        {
            cc.OpenConnection();
            cc.DataReader("select Quantity from BillTable where Id= " + idCount + " ");
            while (cc.reader.Read())
            {
                oldQty = cc.reader["Quantity"].ToString();
            }
            cc.CloseReader();
            cc.CloseConnection();
        }
        private void UpdateQty()
        {
            QtySelect();
            qty = Convert.ToInt32(oldQty) + 1;
            cc.OpenConnection();
            cc.UpdateBillTable("Update BillTable set Quantity=? where Id= " + idCount + " ", "Quantity", qty);
            cc.CloseConnection();
        }
        private void UpdateAmount()
        {
            cc.OpenConnection();
            cc.ExecuteQuery("Update BillTable set Amount = Rate * Quantity + AddedGST where Id = " + idCount + " ");
            cc.CloseConnection();
        }
        public void UpdateGstAmount()
        {
            GetSelectedItemGst();
            cc.OpenConnection();
            cc.UpdateGST("Update BillTable set AddedGST=?*Quantity where Id= " + idCount + " ", "ItemGST", selectedItemGst);
            cc.CloseConnection();
        }
        private void InsertBill()
        {
            cc.OpenConnection();
            cc.InsertBill("insert into BillTable(Id,Name,Rate,Quantity,Amount,GSTPercent,AddedGST) select itemid,iname,sprice,?,sprice+gst_amount,gst_percent,gst_amount from Stock where itemid= " + txtItemId.Text + "", "Quantity", qty);
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void Display()
        {
            cc.OpenConnection();
            cc.DataGridDisplay("select * from BillTable");
            cc.da.Update(cc.dt);
            dataGrid1.ItemsSource = objDs.GetBill();
            billAmountString = cc.dt.Compute("Sum(Amount)", "").ToString();
            labelNetAmount.Content = billAmountString;
            totalGst = cc.dt.Compute("Sum(AddedGST)", "").ToString();
            cc.CloseConnection();
        }

        private void DisplayBill()
        {
            billNumber1 = Convert.ToInt32(txtBillNo.Text);
            GetCustomerDetails();
            cc.OpenConnection();
            cc.ExecuteQuery("insert into BillTable(Id,Name,Rate,Quantity,Amount,GSTPercent,AddedGST) select ItemNo,Product,Price,ProductQuantity,TotalPrice,gstPercent,gstRate from BillStock where BillNo = " + billNumber1 + " ");
            cc.CloseConnection();
        }
        int qty;
        private void EnterClicked(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Return)
                {
                    cc.OpenConnection();
                    if (cc.BillPreview("select COUNT(*) from Stock where itemid = @value ", "@value", txtItemId.Text, isExist) > 0)
                    {
                        StockQuantityCheck();
                        if (stockQtyCheck <= 0)
                        {
                            MessageBox.Show("Out Of Stock!.");
                        }
                        else
                        {

                            idCount = Convert.ToInt32(txtItemId.Text);
                            if (id.list.Contains(txtItemId.Text))
                            {
                                QtySelect();
                                UpdateQty();
                                UpdateGstAmount();
                                UpdateAmount();
                            }
                            else
                            {
                                StoreId();
                                QtySelect();
                                qty = 1;
                                InsertBill();
                            }
                            Display();
                        }
                        Clearid();
                    }
                    else
                    {
                        MessageBox.Show("Item does not exist in DB");
                        Clearid();
                        txtItemId.Focus();
                    }
                    cc.CloseConnection();
                }
            }
            catch (Exception)
            {
                if (txtItemId.Text == "")
                    MessageBox.Show("Enter The ID!.");
            }

        }

        private void StockQuantityCheck()
        {
            StockLeft();
            cc.OpenConnection();
            int id = Convert.ToInt32(txtItemId.Text);
            cc.DataReader("select Quantity from BillTable where Id= " + idCount + " ");
            while (cc.reader.Read())
            {
                oldQty = cc.reader["Quantity"].ToString();
            }
            cc.CloseReader();
            cc.CloseConnection();
            stockQtyCheck = Convert.ToInt32(stockLeft) - Convert.ToInt32(oldQty);
        }

        private void Clearid()
        {
            txtItemId.Text = "";
        }
        public void ClearGrid()
        {
            id.list.Clear();
            cc.OpenConnection();
            cc.ExecuteQuery("delete * from BillTable");
            cc.CloseConnection();
            lbl_Credit.Visibility = Visibility.Hidden;
            textBox_Credit.Visibility = Visibility.Hidden;
            cashPay.Visibility = Visibility.Hidden;
            txtBox_CashPaid.Visibility = Visibility.Hidden;
        }
        private void ClearCustomerTxtBox()
        {
            textBoxCustomer.Text = "";
            textBoxCustomerPh.Text = "";
        }

        private void btn_Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearGrid();
            ClearCustomerTxtBox();
            Display();
            ReadBillNo();
            txtBillNo.Text = BillNumber.ToString();
            txtItemId.Focus();
        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid1.Items.Count == 1)
            {
                MessageBox.Show("No Bill To Save...!");
            }
            else
            {
                var time = DateTime.Now;
                string formattedDate = time.ToString("dd-MMM-yy");
                string netAmount = Convert.ToString(labelNetAmount.Content);
                gstTotal = Convert.ToDouble(totalGst);
                cc.OpenConnection();
                string txtbn = Convert.ToString(txtBillNo.Text);
                int tbn = Convert.ToInt32(txtBillNo.Text);
                if (selectedPayment == "By Credit")
                {
                    if (textBoxCustomer.Text == "" & textBoxCustomerPh.Text == "")
                    {
                        MessageBox.Show("Enter customer details");
                    }
                    else if (textBoxCustomer.Text != "Customer Name" & textBoxCustomerPh.Text != "Phone No")
                    {
                        payCredit = Convert.ToString(labelNetAmount.Content);
                        payDebit = Convert.ToString(0);
                        paymentType = "By Credit";
                        paidCash = payCredit;
                        cc.SaveBill("insert into BillStock(BillNo,ItemNo,Product,Price,ProductQuantity,TotalPrice,gstPercent,gstRate,netGst,NetAmount,SaleDate,CustomerName,CustomerPhone,Credit,Debit,SalesReturn,ActualBillAmnt,CashPaid,Particulars) select ?,Id,Name,Rate,Quantity,Amount,GSTPercent,AddedGST,?,?,?,?,?,?,?,?,?,?,? from BillTable", "BillNo", txtBillNo.Text, "netGst", gstTotal, "NetAmount", netAmount, "SaleDate", formattedDate, "CustomerName", textBoxCustomer.Text, "CustomerPhone", textBoxCustomerPh.Text, "Credit", payCredit, "Debit", payDebit, "SalesReturn", Convert.ToString(returnAmnt), "ActualBillAmnt", Convert.ToString(newBillAmnt), "CashPaid", paidCash, "Particulars", paymentType);
                        if (Convert.ToDouble(netAmount) > 0)
                        {
                            cc.CustomerTransactions("insert into CustomerTransactions(BillNo,CustomerName,CustomerPhone,NetAmount,AmountPaid) values(?,?,?,?,?)", "BillNo", Convert.ToInt32(txtBillNo.Text), "CustomerName", textBoxCustomer.Text, "CustomerPhone", textBoxCustomerPh.Text, "NetAmount", Convert.ToDouble(netAmount), "AmountPaid", Convert.ToDouble(0));
                        }
                        else
                        {
                            cc.CustomerTransactions("insert into CustomerTransactions(BillNo,CustomerName,CustomerPhone,NetAmount,AmountPaid) values(?,?,?,?,?)", "BillNo", Convert.ToInt32(txtBillNo.Text), "CustomerName", textBoxCustomer.Text, "CustomerPhone", textBoxCustomerPh.Text, "NetAmount", Convert.ToDouble(0), "AmountPaid", Convert.ToDouble(0));
                        }
                        cc.CloseReader();
                        StockUpdate();
                        BillNumber = Convert.ToInt32(BillNumber) + 1;
                        billNumber = Convert.ToString(BillNumber);
                        writeSaleBno();
                        MessageBox.Show("Bill Saved Successfully.....");
                        bno = Convert.ToInt32(txtBillNo.Text);
                        string msg = "Do you want to print bill?";
                        string txt = "Bill Print";
                        MessageBoxButton btn = MessageBoxButton.YesNo;
                        MessageBoxResult rslt = MessageBox.Show(msg, txt, btn);
                        switch (rslt)
                        {
                            case MessageBoxResult.Yes:
                                convIntBill = Convert.ToInt32(txtBillNo.Text);
                                clearList();
                                bNO = Convert.ToInt32(bno);
                                generateBill();
                                break;

                            case MessageBoxResult.No:
                                ClearGrid();
                                ClearCustomerTxtBox();
                                Display();
                                txtBillNo.Text = billNumber.ToString();
                                break;
                        }
                        ClearGrid();
                        ClearCustomerTxtBox();
                        Display();
                        txtBillNo.Text = billNumber.ToString();
                        txtItemId.Focus();
                    }
                    else
                    {
                        MessageBox.Show("Enter customer details");
                    }

                }
                else if (selectedPayment == "By Cash")
                {
                    payCredit = Convert.ToString(0);
                    payDebit = Convert.ToString(0);
                    paymentType = "By Cash";
                    paidCash = netAmount;
                    if (textBoxCustomer.Text == "" & textBoxCustomerPh.Text == "")
                    {
                        textBoxCustomer.Text = "Cust_" + txtBillNo.Text;
                        var date = DateTime.Now;
                        string presentDate = date.ToString("dd-MMM-yy");
                        textBoxCustomerPh.Text = "Ph_" + presentDate;
                        cc.SaveBill("insert into BillStock(BillNo,ItemNo,Product,Price,ProductQuantity,TotalPrice,gstPercent,gstRate,netGst,NetAmount,SaleDate,CustomerName,CustomerPhone,Credit,Debit,SalesReturn,ActualBillAmnt,CashPaid,Particulars) select ?,Id,Name,Rate,Quantity,Amount,GSTPercent,AddedGST,?,?,?,?,?,?,?,?,?,?,? from BillTable", "BillNo", txtBillNo.Text, "netGst", gstTotal, "NetAmount", netAmount, "SaleDate", formattedDate, "CustomerName", textBoxCustomer.Text, "CustomerPhone", textBoxCustomerPh.Text, "Credit", payCredit, "Debit", payDebit, "SalesReturn", Convert.ToString(returnAmnt), "ActualBillAmnt", Convert.ToString(newBillAmnt), "CashPaid", paidCash, "Particulars", paymentType);
                        if (Convert.ToDouble(netAmount) > 0)
                        {
                            cc.CustomerTransactions("insert into CustomerTransactions(BillNo,CustomerName,CustomerPhone,NetAmount,AmountPaid) values(?,?,?,?,?)", "BillNo", Convert.ToInt32(txtBillNo.Text), "CustomerName", textBoxCustomer.Text, "CustomerPhone", textBoxCustomerPh.Text, "NetAmount", Convert.ToDouble(netAmount), "AmountPaid", Convert.ToDouble(netAmount));
                        }
                        else
                        {
                            cc.CustomerTransactions("insert into CustomerTransactions(BillNo,CustomerName,CustomerPhone,NetAmount,AmountPaid) values(?,?,?,?,?)", "BillNo", Convert.ToInt32(txtBillNo.Text), "CustomerName", textBoxCustomer.Text, "CustomerPhone", textBoxCustomerPh.Text, "NetAmount", Convert.ToDouble(0), "AmountPaid", Convert.ToDouble(0));
                        }
                        cc.CloseReader();
                    }
                    else if(textBoxCustomer.Text != "Customer Name" & textBoxCustomerPh.Text != "Phone No")
                    {
                        cc.SaveBill("insert into BillStock(BillNo,ItemNo,Product,Price,ProductQuantity,TotalPrice,gstPercent,gstRate,netGst,NetAmount,SaleDate,CustomerName,CustomerPhone,Credit,Debit,SalesReturn,ActualBillAmnt,CashPaid,Particulars) select ?,Id,Name,Rate,Quantity,Amount,GSTPercent,AddedGST,?,?,?,?,?,?,?,?,?,?,? from BillTable", "BillNo", txtBillNo.Text, "netGst", gstTotal, "NetAmount", netAmount, "SaleDate", formattedDate, "CustomerName", textBoxCustomer.Text, "CustomerPhone", textBoxCustomerPh.Text, "Credit", payCredit, "Debit", payDebit, "SalesReturn", Convert.ToString(returnAmnt), "ActualBillAmnt", Convert.ToString(newBillAmnt), "CashPaid", paidCash, "Particulars", paymentType);
                        if (Convert.ToDouble(netAmount) > 0)
                        {
                            cc.CustomerTransactions("insert into CustomerTransactions(BillNo,CustomerName,CustomerPhone,NetAmount,AmountPaid) values(?,?,?,?,?)", "BillNo", Convert.ToInt32(txtBillNo.Text), "CustomerName", textBoxCustomer.Text, "CustomerPhone", textBoxCustomerPh.Text, "NetAmount", Convert.ToDouble(netAmount), "AmountPaid", Convert.ToDouble(netAmount));
                        }
                        else
                        {
                            cc.CustomerTransactions("insert into CustomerTransactions(BillNo,CustomerName,CustomerPhone,NetAmount,AmountPaid) values(?,?,?,?,?)", "BillNo", Convert.ToInt32(txtBillNo.Text), "CustomerName", textBoxCustomer.Text, "CustomerPhone", textBoxCustomerPh.Text, "NetAmount", Convert.ToDouble(0), "AmountPaid", Convert.ToDouble(0));
                        }
                        cc.CloseReader();
                    }
                    else
                    {
                        textBoxCustomer.Text = "Cust_" + txtBillNo.Text;
                        var date = DateTime.Now;
                        string presentDate = date.ToString("dd-MMM-yy");
                        textBoxCustomerPh.Text = "Ph_" + presentDate;
                        cc.SaveBill("insert into BillStock(BillNo,ItemNo,Product,Price,ProductQuantity,TotalPrice,gstPercent,gstRate,netGst,NetAmount,SaleDate,CustomerName,CustomerPhone,Credit,Debit,SalesReturn,ActualBillAmnt,CashPaid,Particulars) select ?,Id,Name,Rate,Quantity,Amount,GSTPercent,AddedGST,?,?,?,?,?,?,?,?,?,?,? from BillTable", "BillNo", txtBillNo.Text, "netGst", gstTotal, "NetAmount", netAmount, "SaleDate", formattedDate, "CustomerName", textBoxCustomer.Text, "CustomerPhone", textBoxCustomerPh.Text, "Credit", payCredit, "Debit", payDebit, "SalesReturn", Convert.ToString(returnAmnt), "ActualBillAmnt", Convert.ToString(newBillAmnt), "CashPaid", paidCash, "Particulars", paymentType);
                        if (Convert.ToDouble(netAmount) > 0)
                        {
                            cc.CustomerTransactions("insert into CustomerTransactions(BillNo,CustomerName,CustomerPhone,NetAmount,AmountPaid) values(?,?,?,?,?)", "BillNo", Convert.ToInt32(txtBillNo.Text), "CustomerName", textBoxCustomer.Text, "CustomerPhone", textBoxCustomerPh.Text, "NetAmount", Convert.ToDouble(netAmount), "AmountPaid", Convert.ToDouble(netAmount));
                        }
                        else
                        {
                            cc.CustomerTransactions("insert into CustomerTransactions(BillNo,CustomerName,CustomerPhone,NetAmount,AmountPaid) values(?,?,?,?,?)", "BillNo", Convert.ToInt32(txtBillNo.Text), "CustomerName", textBoxCustomer.Text, "CustomerPhone", textBoxCustomerPh.Text, "NetAmount", Convert.ToDouble(0), "AmountPaid", Convert.ToDouble(0));
                        }
                        cc.CloseReader();
                    }
                    StockUpdate();
                    BillNumber = Convert.ToInt32(BillNumber) + 1;
                    billNumber = Convert.ToString(BillNumber);
                    writeSaleBno();
                    MessageBox.Show("Bill Saved Successfully.....");
                    bno = Convert.ToInt32(txtBillNo.Text);
                    string msg = "Do you want to print bill?";
                    string txt = "Bill Print";
                    MessageBoxButton btn = MessageBoxButton.YesNo;
                    MessageBoxResult rslt = MessageBox.Show(msg, txt, btn);
                    switch (rslt)
                    {
                        case MessageBoxResult.Yes:
                            convIntBill = Convert.ToInt32(txtBillNo.Text);
                            clearList();
                            bNO = Convert.ToInt32(bno);
                            generateBill();
                            break;

                        case MessageBoxResult.No:
                            ClearGrid();
                            ClearCustomerTxtBox();
                            Display();
                            txtBillNo.Text = billNumber.ToString();
                            break;
                    }
                    ClearGrid();
                    ClearCustomerTxtBox();
                    lbSuggestion.Visibility = Visibility.Collapsed;
                    Display();
                    txtBillNo.Text = billNumber.ToString();
                    txtItemId.Focus();
                }
                else if (selectedPayment == "Multi Payment")
                {
                    if (textBoxCustomer.Text == "" & textBoxCustomerPh.Text == "")
                    {
                        MessageBox.Show("Enter customer details");
                    }
                    else if (textBoxCustomer.Text != "Customer Name" & textBoxCustomerPh.Text != "Phone No")
                    {
                        string lastContent = lbl_Credit.Content.ToString();
                        paymentType = "Multi Payment";
                        paidCash = txtBox_CashPaid.Text;
                        if (lastContent == "Credit")
                        {
                            payCredit = textBox_Credit.Text;
                            payDebit = Convert.ToString(0);
                        }
                        else
                        {
                            payCredit = Convert.ToString(0);
                            payDebit = textBox_Credit.Text;
                        }
                        cc.SaveBill("insert into BillStock(BillNo,ItemNo,Product,Price,ProductQuantity,TotalPrice,gstPercent,gstRate,netGst,NetAmount,SaleDate,CustomerName,CustomerPhone,Credit,Debit,SalesReturn,ActualBillAmnt,CashPaid,Particulars) select ?,Id,Name,Rate,Quantity,Amount,GSTPercent,AddedGST,?,?,?,?,?,?,?,?,?,?,? from BillTable", "BillNo", txtBillNo.Text, "netGst", gstTotal, "NetAmount", netAmount, "SaleDate", formattedDate, "CustomerName", textBoxCustomer.Text, "CustomerPhone", textBoxCustomerPh.Text, "Credit", payCredit, "Debit", payDebit, "SalesReturn", Convert.ToString(returnAmnt), "ActualBillAmnt", Convert.ToString(newBillAmnt), "CashPaid", paidCash, "Particulars", paymentType);
                        if (Convert.ToDouble(netAmount) > 0)
                        {
                            cc.CustomerTransactions("insert into CustomerTransactions(BillNo,CustomerName,CustomerPhone,NetAmount,AmountPaid) values(?,?,?,?,?)", "BillNo", Convert.ToInt32(txtBillNo.Text), "CustomerName", textBoxCustomer.Text, "CustomerPhone", textBoxCustomerPh.Text, "NetAmount", Convert.ToDouble(netAmount), "AmountPaid", Convert.ToDouble(txtBox_CashPaid.Text));
                        }
                        else
                        {
                            cc.CustomerTransactions("insert into CustomerTransactions(BillNo,CustomerName,CustomerPhone,NetAmount,AmountPaid) values(?,?,?,?,?)", "BillNo", Convert.ToInt32(txtBillNo.Text), "CustomerName", textBoxCustomer.Text, "CustomerPhone", textBoxCustomerPh.Text, "NetAmount", Convert.ToDouble(0), "AmountPaid", Convert.ToDouble(0));
                        }
                        cc.CloseReader();
                        StockUpdate();
                        BillNumber = Convert.ToInt32(BillNumber) + 1;
                        billNumber = Convert.ToString(BillNumber);
                        writeSaleBno();
                        MessageBox.Show("Bill Saved Successfully.....");
                        bno = Convert.ToInt32(txtBillNo.Text);
                        string msg = "Do you want to print bill?";
                        string txt = "Bill Print";
                        MessageBoxButton btn = MessageBoxButton.YesNo;
                        MessageBoxResult rslt = MessageBox.Show(msg, txt, btn);
                        switch (rslt)
                        {
                            case MessageBoxResult.Yes:
                                convIntBill = Convert.ToInt32(txtBillNo.Text);
                                clearList();
                                bNO = Convert.ToInt32(bno);
                                generateBill();
                                break;

                            case MessageBoxResult.No:
                                ClearGrid();
                                ClearCustomerTxtBox();
                                Display();
                                txtBillNo.Text = billNumber.ToString();
                                break;
                        }
                        ClearGrid();
                        ClearCustomerTxtBox();
                        Display();
                        txtBillNo.Text = billNumber.ToString();
                        txtItemId.Focus();
                        cc.CloseConnection();
                    }
                    else
                    {
                        MessageBox.Show("Enter customer details");
                    }
                }
                else
                {
                    MessageBox.Show("Select a payment option");
                }
            }
            returnAmnt = 0;
            newBillAmnt = 0;
        }

        private void writeSaleBno()
        {
            string configFilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Config\Config.txt");
            string[] configValues = File.ReadAllLines(configFilePath);
            for (int i = 0; i < configValues.Length; i++)
            {
                if (configValues[i] == "SalesBillNumber '" + billNo + "'")
                {
                    configValues[i] = "SalesBillNumber '" + billNumber + "'";
                }
            }
            File.WriteAllLines(configFilePath, configValues);
        }

        private void StockUpdate()
        {
            cc.OpenConnection();
            cc.ExecuteQuery("update Stock as a inner join BillTable as b on a.itemid = b.Id set a.quantity = a.quantity - b.Quantity");
            cc.CloseConnection();
        }
        private void ReadBillNo()
        {
            string saleBillNoPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Config\Config.txt");
            string saleBillNoValue = File.ReadAllText(saleBillNoPath);
            if (saleBillNoValue.Contains("SalesBillNumber"))
            {
                string[] val = saleBillNoValue.Substring(saleBillNoValue.IndexOf("SalesBillNumber") + 17).Split(Convert.ToChar("'"));
                billNoList.Add(val[0].ToString());
                foreach (var number in billNoList)
                {
                    billNumber = Convert.ToString(number);
                    billNo = billNumber;
                }
            }
        }

        private void button_Copy2_Click(object sender, RoutedEventArgs e)
        {
            string mac = "";
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {

                if (nic.OperationalStatus == OperationalStatus.Up && (!nic.Description.Contains("Virtual") && !nic.Description.Contains("Pseudo")))
                {
                    if (nic.GetPhysicalAddress().ToString() != "")
                    {
                        mac = nic.GetPhysicalAddress().ToString();
                    }
                }
            }
            string result = mac.Substring(0, 3);
            MessageBox.Show(result);
        }

        private void dataGrid1_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void textBoxCustomer_LostFocus(object sender, RoutedEventArgs e)
        {
            lbSuggestion.Visibility = Visibility.Collapsed;
        }

        private void StockLeft()
        {
            cc.OpenConnection();
            int id = Convert.ToInt32(txtItemId.Text);
            cc.DataReader("select quantity from Stock where itemid= " + id + " ");
            while (cc.reader.Read())
            {
                stockLeft = cc.reader["quantity"].ToString();
            }
            cc.CloseReader();
            cc.CloseConnection();
        }
        private void GetSelectedItemGst()
        {
            cc.OpenConnection();
            int id = Convert.ToInt32(txtItemId.Text);
            cc.DataReader("select gst_amount from Stock where itemid= " + id + " ");
            while (cc.reader.Read())
            {
                selectedItemGst = cc.reader["gst_amount"].ToString();
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void btn_Sales_Return_Click(object sender, RoutedEventArgs e)
        {
            SalesReturn sr = new SalesReturn();
            sr.InputChanged += OnDialogInputChanged;
            sr.Show();
        }

        private void OnDialogInputChanged(object sender, SalesReturn.DialogInputEventArgs e)
        {
            var sr = sender as SalesReturn;
            if (labelNetAmount.Content.ToString() == "")
            {
                labelNetAmount.Content = 0;
            }
            returnAmnt = Convert.ToDouble(sr.labelNetAmount.Text);
            newBillAmnt = Convert.ToDouble(labelNetAmount.Content);
            toPay = newBillAmnt - returnAmnt;
            labelNetAmount.Content = Convert.ToString(toPay);
        }

        private void GetCustomerDetails()
        {
            lbl_Credit.Visibility = Visibility.Visible;
            textBox_Credit.Visibility = Visibility.Visible;
            cc.OpenConnection();
            cc.DataReader("select CustomerName,CustomerPhone,Credit from BillStock where BillNo = " + billNumber1 + " ");
            while (cc.reader.Read())
            {
                textBoxCustomer.Text = cc.reader["CustomerName"].ToString();
                textBoxCustomerPh.Text = cc.reader["CustomerPhone"].ToString();
                textBox_Credit.Text = cc.reader["Credit"].ToString();
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void txtItemId_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var txtItemId = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void txtBillNo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var txtBillNo = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void textBoxCustomerPh_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBoxCustomerPh = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void btn_Sales_History_Click(object sender, RoutedEventArgs e)
        {
            SalesHistory sh = new SalesHistory();
            this.NavigationService.Navigate(sh);
        }

        private void backbutton_Click(object sender, RoutedEventArgs e)
        {
            customerList.Clear();
            SPpage sp = new SPpage();
            this.NavigationService.Navigate(sp);
        }

        private void comboBoxPayment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedPayment = comboBoxPayment.SelectedItem.ToString();
            if (selectedPayment == "Multi Payment")
            {
                ShowBalance();
            }
            else if (selectedPayment == "By Credit")
            {
                HideBalance();
            }
            else if (selectedPayment == "By Cash")
            {
                HideBalance();
            }
        }

        private void HideBalance()
        {
            lbl_Credit.Visibility = Visibility.Hidden;
            textBox_Credit.Visibility = Visibility.Hidden;
            cashPay.Visibility = Visibility.Hidden;
            txtBox_CashPaid.Visibility = Visibility.Hidden;
        }

        private void ShowBalance()
        {
            lbl_Credit.Visibility = Visibility.Visible;
            textBox_Credit.Visibility = Visibility.Visible;
            cashPay.Visibility = Visibility.Visible;
            txtBox_CashPaid.Visibility = Visibility.Visible;
        }

        private void txtBox_CashPaid_TextChanged(object sender, TextChangedEventArgs e)
        {
            calculateBalanceAmount();
        }

        private void calculateBalanceAmount()
        {
            double netAmnt = Convert.ToDouble(labelNetAmount.Content);
            double cashPaid;
            if (Double.TryParse(txtBox_CashPaid.Text, out cashPaid))
            {
                if (netAmnt >= cashPaid)
                {
                    lbl_Credit.Content = "Credit";
                    textBox_Credit.Text = (netAmnt - cashPaid).ToString();
                }
                else
                {
                    lbl_Credit.Content = "Debit";
                    textBox_Credit.Text = ((netAmnt - cashPaid) * -1).ToString();
                }
            }
        }

        private void ViewBill()
        {
            cc.OpenConnection();
            string txtbn = Convert.ToString(txtBillNo.Text);
            if (cc.BillPreview("select COUNT(*) from BillStock where BillNo = @bno ", "@bno", txtbn, billExist) > 0)
            {
                ClearCustomerTxtBox();
                DisplayBill();
                Display();
            }
            else
            {
                MessageBox.Show("Bill does not exist");
                ClearGrid();
            }
            cc.CloseConnection();
        }

        private void textBoxCustomer_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox textBoxCustomer = sender as TextBox;
            if (textBoxCustomer.Text == "Customer Name")
                textBoxCustomer.Text = string.Empty;
            if(textBoxCustomer.Text=="")
            {
                lbSuggestion.ItemsSource = customerList;
                lbSuggestion.Visibility = Visibility.Visible;
            }
        }

        private void textBoxCustomerPh_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox textBoxCustomer = sender as TextBox;
            if (textBoxCustomer.Text == "Phone No")
                textBoxCustomer.Text = string.Empty;
        }

        private void btn_Print_Click(object sender, RoutedEventArgs e)
        {
            bno = Convert.ToInt32(txtBillNo.Text);
            string msg = "Do you want to print bill?";
            string txt = "Bill Print";
            MessageBoxButton btn = MessageBoxButton.YesNo;
            MessageBoxResult rslt = MessageBox.Show(msg, txt, btn);
            switch (rslt)
            {
                case MessageBoxResult.Yes:
                    convIntBill = Convert.ToInt32(txtBillNo.Text);
                    clearList();
                    bNO = Convert.ToInt32(bno);
                    generateBill();
                    break;

                case MessageBoxResult.No:
                    break;
            }
            txtItemId.Focus();
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
            saleDateList.Clear();
            gstPercentList.Clear();
            gstRateList.Clear();
            totalGstList.Clear();
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
            Report.PrintToPrinter(1, false, 0, 15);
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

        private void textBox_Credit_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (textBox_Credit.Text == "")
                    textBox_Credit.Text = "0";
                cc.OpenConnection();
                cc.ExecuteQuery("update BillStock set Credit=" + textBox_Credit.Text + " where BillNo=" + billNumber1 + "");
                cc.CloseConnection();
                MessageBox.Show("Credit Updated");
            }
        }

        private void textBox_Credit_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_Credit.ToolTip = "Changes in here will be saved...";
        }

    }
}
