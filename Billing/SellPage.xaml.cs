using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Billing
{
    /// <summary>
    /// Interaction logic for SellPage.xaml
    /// </summary>
    public partial class SellPage : Page
    {
        public static int idCount = 0, convIntBill, stockQtyCheck, BillNumber, billNumber1, quantityCell, billExist, bno, isExist, bNO, srbno, billExistValue;
        public static string billNumber, storeBillNoPath, payCredit, payDebit, netAmount, selectedPayment, barcodeId, paidCash,paymentType, billType;
        public static string itemi, itname, rat, quantyty, spric, gstper, gstam, cellqty, selectedRowId, stockLeftForCell, formattedDate, cellgstp;
        public static string stockLeft, totalGst, getPhno, cnameSelected, customerName, customerPhone, oldQty, correctBillAmount, itemScale;
        public static double gstTotal, returnAmnt, newBillAmnt, toPay, roundCorrectBill, roundAfterDiscount, Netgst, CardPoints, sellingPrice;
        public static List<string> customerList = new List<string>();
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
        public static List<double> discountList = new List<double>();
        public static string[] customerPh;
        public static BillingReport Report = new BillingReport();
        DataTable dt = new DataTable();
        DataTable dt1 = new DataTable();

        public SellPage()
        {
            InitializeComponent();
            if(MainWindow.userName=="admin")
            {
                checkBoxGST.IsChecked = false;
                billType = "NON_GST";
                dataGrid1.Visibility = Visibility.Collapsed;
                dataGrid2.Visibility = Visibility.Visible;
            }
            else
            {
                checkBoxGST.IsChecked = true;
                checkBoxGST.Visibility = Visibility.Collapsed;
                billType = "GST";
                dataGrid1.Visibility = Visibility.Visible;
                dataGrid2.Visibility = Visibility.Collapsed;
            }
            comboBoxPayment.SelectedItem = "By Cash";
            CustomerSuggestion();
            textBoxCustomer.TextChanged += new TextChangedEventHandler(textBoxCustomer_TextChanged);
            returnAmnt = 0;
            newBillAmnt = 0;
            HideBalance();
            comboBoxPayment.Items.Add("By Cash");
            comboBoxPayment.Items.Add("By Credit");
            comboBoxPayment.Items.Add("Multi Payment");
            var date = DateTime.Now;
            string presentDate = date.ToString("dd-MMM-yy");
            lblDate.Content = presentDate;
            txtItemId.Focus();
            ClearGrid();
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
            cc.DataReader("select CustomerName,CustomerPhone from CustomerDetails");
            while (cc.reader.Read())
            {
                customerList.Add(cc.reader["CustomerName"].ToString()+"  "+cc.reader["CustomerPhone"]);
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void GetPhno()
        {
            cnameSelected = lbSuggestion.SelectedItem.ToString();
            customerPh = Regex.Split(cnameSelected, "  ");
            cc.OpenConnection();
            cc.DataReader("select CustomerPhone from CustomerDetails where CustomerPhone='"+customerPh[1]+"'");
            while (cc.reader.Read())
            {
                getPhno = cc.reader["CustomerPhone"].ToString();
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dt.Columns.Add("Id", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Rate", typeof(double));
            dt.Columns.Add("Quantity", typeof(string));
            dt.Columns.Add("GSTPercent", typeof(string));
            dt.Columns.Add("AddedGST", typeof(double));
            dt.Columns.Add("Amount", typeof(double));

            dt1.Columns.Add("Id", typeof(string));
            dt1.Columns.Add("Name", typeof(string));
            dt1.Columns.Add("Rate", typeof(double));
            dt1.Columns.Add("Quantity", typeof(string));
            dt1.Columns.Add("Amount", typeof(double));
        }
        private void StoreId()
        {
            id.StoreIId(idCount);
        }
        
        private void DisplayNetAmount()
        {
            if(checkBoxGST.IsChecked == true)
            {
                double NetA = Convert.ToDouble(dt.Compute("Sum(Amount)", ""));
                Netgst = Convert.ToDouble(dt.Compute("Sum(AddedGST)", ""));
                txtBlkNetAmount.Text = Convert.ToString(NetA + Netgst);
            }
            else
            {
                double NetA = Convert.ToDouble(dt1.Compute("Sum(Amount)", ""));
                txtBlkNetAmount.Text = Convert.ToString(NetA);
            }
            if (txtBlkNetAmount.Text != "")
            {
                correctBillAmount = txtBlkNetAmount.Text;
                roundCorrectBill = Convert.ToDouble(txtBlkNetAmount.Text);
            }
        }
        public void UpdateGridRow()
        {
            if(checkBoxGST.IsChecked == true)
            {
                int index = -1;
                DataRow[] rows = dt.Select("Id = " + txtItemId.Text + "");
                if (rows.Count() > 0)
                {
                    index = dt.Rows.IndexOf(rows[0]);
                }
                quantityCell = Convert.ToInt32(dt.Rows[index][3]);
                double gstpCell = Convert.ToDouble(dt.Rows[index][4]);
                StockQuantityCheck();
                if (stockQtyCheck <= 0)
                {
                    MessageBox.Show("Out Of Stock!.");
                }
                else
                {
                    DataRow dr = dt.Select("Id=" + txtItemId.Text + "").FirstOrDefault();
                    if (dr != null)
                    {
                        dr["Quantity"] = quantityCell + 1;
                        dr["AddedGST"] = (quantityCell + 1) * (((Convert.ToDouble(sellingPrice) + Convert.ToDouble(gstam)) / 100) * Convert.ToDouble(gstpCell));
                        dr["Amount"] = (quantityCell + 1) * Convert.ToDouble(sellingPrice);
                    }
                }
            }
            else
            {
                int index = -1;
                DataRow[] rows = dt1.Select("Id = " + txtItemId.Text + "");
                if (rows.Count() > 0)
                {
                    index = dt1.Rows.IndexOf(rows[0]);
                }
                quantityCell = Convert.ToInt32(dt1.Rows[index][3]);
                StockQuantityCheck();
                if (stockQtyCheck <= 0)
                {
                    MessageBox.Show("Out Of Stock!.");
                }
                else
                {
                    DataRow dr = dt1.Select("Id=" + txtItemId.Text + "").FirstOrDefault();
                    if (dr != null)
                    {
                        dr["Quantity"] = quantityCell + 1;
                        dr["Amount"] = (quantityCell + 1) * Convert.ToDouble(spric);
                    }
                }
            }
        }
        private void InsertNewRow()
        {
            if(checkBoxGST.IsChecked == true)
            {
                dt.Rows.Add(new string[] { itemi, itname, sellingPrice.ToString(), "1", gstper, gstam, sellingPrice.ToString() });
                dataGrid1.ItemsSource = dt.DefaultView;
            }
            else
            {
                dt1.Rows.Add(new string[] { itemi, itname, spric, "1", spric });
                dataGrid2.ItemsSource = dt1.DefaultView;
            }
        }

        private void GetFromStock()
        {
            cc.OpenConnection();
            cc.DataReader("select itemid,iname,quantity,sprice,salegstpercent,salegstamount from Stock where itemid= " + Convert.ToInt32(txtItemId.Text) + "");
            while (cc.reader.Read())
            {
                itemi = cc.reader["itemid"].ToString();
                itname = cc.reader["iname"].ToString();
                quantyty = cc.reader["quantity"].ToString();
                spric = cc.reader["sprice"].ToString();
                gstper = cc.reader["salegstpercent"].ToString();
                gstam = cc.reader["salegstamount"].ToString();
            }
            cc.CloseReader();
            cc.CloseConnection();
            sellingPrice = Convert.ToDouble(spric) - Convert.ToDouble(gstam);
        }

        private void dataGrid1_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            string col = ((DataGrid)sender).CurrentCell.Column.Header.ToString();
            if (col == "Quantity")
            {
                DataRowView rowview = dataGrid1.SelectedItem as DataRowView;
                string oldq = rowview.Row[3].ToString();
                selectedRowId = rowview.Row[0].ToString();
                FrameworkElement element_Quantity = dataGrid1.Columns[3].GetCellContent(e.Row);
                if (element_Quantity.GetType() == typeof(TextBox))
                {
                    var qty = ((TextBox)element_Quantity).Text;
                    cellqty = qty;
                }
                StockLeftForCellEdit();
                int stk = Convert.ToInt32(stockLeftForCell);
                int check = stk - Convert.ToInt32(cellqty);
                if (stk <= 0)
                {
                    MessageBox.Show("Out Of Stock!.");
                }
                else if (check < 0)
                {
                    MessageBox.Show("Sorry...! Only " + stockLeftForCell + " Items Left ");
                    DataRow dr = dt.Select("Id=" + selectedRowId + "").FirstOrDefault();
                    if (dr != null)
                    {
                        dr["Quantity"] = Convert.ToInt32(oldq);
                    }
                }
                else
                {
                    string cgstp = rowview.Row[4].ToString();
                    DataRow dr = dt.Select("Id=" + selectedRowId + "").FirstOrDefault();
                    if (dr != null)
                    {
                        dr["Quantity"] = Convert.ToInt32(cellqty);
                        dr["AddedGST"] = Convert.ToInt32(cellqty) * (((Convert.ToDouble(sellingPrice) + Convert.ToDouble(gstam)) / 100) * Convert.ToDouble(cgstp));
                        dr["Amount"] = Convert.ToInt32(cellqty) * Convert.ToDouble(sellingPrice);
                    }
                    DisplayNetAmount();
                }
            }
             else
            {
                DataRowView rowview = dataGrid1.SelectedItem as DataRowView;
                string oldq = rowview.Row[3].ToString();
                selectedRowId = rowview.Row[0].ToString();
                FrameworkElement element_GST = dataGrid1.Columns[4].GetCellContent(e.Row);
                if (element_GST.GetType() == typeof(TextBox))
                {
                    var gstp = ((TextBox)element_GST).Text;
                    cellgstp = gstp;
                }
                DataRow dr = dt.Select("Id=" + selectedRowId + "").FirstOrDefault();
                if (dr != null)
                {
                    dr["AddedGST"] = ((Convert.ToDouble(spric)/100) * Convert.ToDouble(cellgstp))* Convert.ToInt32(oldq);
                }
                DisplayNetAmount();
            }   
        }

        private void ScaleCheck()
        {
            cc.OpenConnection();
            cc.DataReader("select scale from Stock where itemid = '" + txtItemId.Text + "'");
            while (cc.reader.Read())
            {
                itemScale = cc.reader["scale"].ToString();
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void EnterClicked(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Return)
                {
                    cc.OpenConnection();
                    if (cc.BillPreview("select COUNT(*) from Stock where itemid = @value ", "@value", txtItemId.Text, isExist) > 0)
                    {
                        cc.CloseConnection();
                        ScaleCheck();
                        if(itemScale == "Mtr")
                        {
                            ScalePopUp spu = new ScalePopUp();
                            spu.Show();
                        }
                        else
                        {
                            idCount = Convert.ToInt32(txtItemId.Text);
                            StockLeft();
                            if (Convert.ToInt32(stockLeft) <= 0)
                            {
                                MessageBox.Show("Out Of Stock!.");
                            }
                            else
                            {
                                GetFromStock();
                                if (id.list.Contains(txtItemId.Text))
                                {
                                    UpdateGridRow();
                                }
                                else
                                {
                                    StoreId();
                                    InsertNewRow();
                                }
                                DisplayNetAmount();
                            }
                            txtItemId.Text = "";
                        }
                    }
                    else
                    {
                        MessageBox.Show("Item does not exist in DB");
                        Clearid();
                        txtItemId.Focus();
                    }
                }
            }
            catch (Exception)
            {
                if (txtItemId.Text == "")
                    MessageBox.Show("Enter The ID!.");
            }

        }

        private void StockLeftForCellEdit()
        {
            cc.OpenConnection();
            cc.DataReader("select quantity from Stock where itemid= " + selectedRowId + " ");
            while (cc.reader.Read())
            {
                stockLeftForCell = cc.reader["quantity"].ToString();
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void dataGrid2_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void dataGrid2_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            DataRowView rowview = dataGrid2.SelectedItem as DataRowView;
            string oldq = rowview.Row[3].ToString();
            selectedRowId = rowview.Row[0].ToString();
            FrameworkElement element_Quantity = dataGrid2.Columns[3].GetCellContent(e.Row);
            if (element_Quantity.GetType() == typeof(TextBox))
            {
                var qty = ((TextBox)element_Quantity).Text;
                cellqty = qty;
            }
            StockLeftForCellEdit();
            int stk = Convert.ToInt32(stockLeftForCell);
            int check = stk - Convert.ToInt32(cellqty);
            if (stk <= 0)
            {
                MessageBox.Show("Out Of Stock!.");
            }
            else if (check < 0)
            {
                MessageBox.Show("Sorry...! Only " + stockLeftForCell + " Items Left ");
                DataRow dr = dt1.Select("Id=" + selectedRowId + "").FirstOrDefault();
                if (dr != null)
                {
                    dr["Quantity"] = Convert.ToInt32(oldq);
                }
            }
            else
            {
                DataRow dr = dt1.Select("Id=" + selectedRowId + "").FirstOrDefault();
                if (dr != null)
                {
                    dr["Quantity"] = Convert.ToInt32(cellqty);
                    dr["Amount"] = Convert.ToInt32(cellqty) * Convert.ToDouble(spric);
                }
                DisplayNetAmount();
            }
        }

        private void dataGrid2_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                try
                {
                    int selectedIndex = dataGrid2.SelectedIndex;
                    if (dt1.Rows.Count == 1 && selectedIndex == 0)
                    {
                        dt1.Clear();
                        txtBlkNetAmount.Text = "";
                        id.list.Clear();
                    }
                    else if (dt1.Rows.Count > 1)
                    {
                        id.list.RemoveAt(selectedIndex);
                        dt1.Rows.RemoveAt(selectedIndex);
                        dataGrid2.ItemsSource = null;
                        dataGrid2.ItemsSource = dt1.DefaultView;
                        DisplayNetAmount();
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
                txtItemId.Focus();
            }
        }

        private void txtBox_CashPaid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                btn_Save.Focus();
            }
        }

        private void txtBox_CashPaid_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtBlkNetAmount.Text == "")
            {
                MessageBox.Show("Do some transaction");
                txtItemId.Focus();
            }
        }

        private void checkBoxGST_Checked(object sender, RoutedEventArgs e)
        {
            txtItemId.Focus();
            ClearGrid();
            billType = "GST";
            dataGrid1.Visibility = Visibility.Visible;
            dataGrid2.Visibility = Visibility.Collapsed;
        }

        private void checkBoxGST_Unchecked(object sender, RoutedEventArgs e)
        {
            txtItemId.Focus();
            ClearGrid();
            billType = "NON_GST";
            dataGrid1.Visibility = Visibility.Collapsed;
            dataGrid2.Visibility = Visibility.Visible;
        }

        private void StockLeft()
        {
            cc.OpenConnection();
            cc.DataReader("select quantity from Stock where itemid= " + idCount + " ");
            while (cc.reader.Read())
            {
                stockLeft = cc.reader["quantity"].ToString();
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void StockQuantityCheck()
        {
            StockLeft();
            int id = Convert.ToInt32(txtItemId.Text);
            oldQty =Convert.ToString(quantityCell);
            stockQtyCheck = Convert.ToInt32(stockLeft) - Convert.ToInt32(oldQty);
        }

        private void Clearid()
        {
            txtItemId.Text = "";
        }
        public void ClearGrid()
        {
            dt.Clear();
            dt1.Clear();
            txtBlkNetAmount.Text = "";
            id.list.Clear();
            lbl_Credit.Visibility = Visibility.Hidden;
            textBox_Credit.Visibility = Visibility.Hidden;
            cashPay.Visibility = Visibility.Hidden;
            txtBox_CashPaid.Visibility = Visibility.Hidden;
            txtBox_Discount.Text = "";
            txtBox_DiscountPercent.Text = "";
            checkBox.IsChecked = false;
            dataGridSR.Visibility = Visibility.Collapsed;
            salereturnhead.Visibility = Visibility.Collapsed;
        }
        private void ClearCustomerTxtBox()
        {
            textBoxCustomer.Text = "Customer Name";
            textBoxCustomerPh.Text = "Phone No";
            textBlockCard.Visibility = Visibility.Collapsed;
            textBlockCardBalance.Visibility = Visibility.Collapsed;
            textBoxDiscountCard.Text = "Enter Card Id";
            checkBoxCard.IsEnabled = false;
        }

        private void btn_Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearGrid();
            ClearCustomerTxtBox();
            lbSuggestion.Visibility = Visibility.Collapsed;
            txtItemId.Focus();
        }

        private void SaveBillFromGrid()
        {
            if(checkBoxGST.IsChecked == true)
            {
                foreach (DataRow dg in dt.Rows)
                {
                    string gridId = dg[0].ToString();
                    string gridName = dg[1].ToString();
                    string gridRate = dg[2].ToString();
                    string gridQuantity = dg[3].ToString();
                    string gridGstPercent = dg[4].ToString();
                    string gridGst = dg[5].ToString();
                    string gridAmount = dg[6].ToString();
                    cc.OpenConnection();
                    cc.DataReader("insert into BillStock(BillNo,ItemNo,Product,Price,ProductQuantity,TotalPrice,gstPercent,gstRate,netGst,NetAmount,Discount,SaleDate,CustomerName,CustomerPhone,Credit,Debit,SalesReturn,ActualBillAmnt,CashPaid,Particulars,BillType) values('" + billNumber + "','" + gridId + "','" + gridName + "','" + gridRate + "','" + gridQuantity + "','" + gridAmount + "','" + gridGstPercent + "','" + gridGst + "','" + Netgst + "','" + netAmount + "','" + txtBox_Discount.Text + "','" + formattedDate + "','" + textBoxCustomer.Text + "','" + textBoxCustomerPh.Text + "','" + payCredit + "','" + payDebit + "','" + Convert.ToString(returnAmnt) + "','" + Convert.ToString(newBillAmnt) + "','" + paidCash + "','" + paymentType + "','" + billType + "')");
                    cc.CloseReader();
                    cc.ExecuteQuery("update Stock set quantity = quantity - " + Convert.ToInt32(gridQuantity) + " where itemid = " + Convert.ToInt32(gridId) + "");
                    if (Convert.ToDouble(netAmount) > 0)
                    {
                        cc.CustomerTransactions("insert into CustomerTransactions(BillNo,CustomerName,CustomerPhone,NetAmount,AmountPaid,Date,Credit,Debit,BillType,Discount) values(@BillNo,@CustomerName,@CustomerPhone,@NetAmount,@AmountPaid,@Date,@Credit,@Debit,@BillType,@Discount)", "BillNo", Convert.ToInt32(billNumber), "CustomerName", textBoxCustomer.Text, "CustomerPhone", textBoxCustomerPh.Text, "NetAmount", Convert.ToDouble(netAmount), "AmountPaid", Convert.ToDouble(paidCash), "Date", formattedDate, "Credit", payCredit, "Debit", payDebit, "BillType", billType, "Discount", Convert.ToString(0));
                    }
                    else
                    {
                        cc.CustomerTransactions("insert into CustomerTransactions(BillNo,CustomerName,CustomerPhone,NetAmount,AmountPaid,Date,Credit,Debit,BillType,Discount) values(@BillNo,@CustomerName,@CustomerPhone,@NetAmount,@AmountPaid,@Date,@Credit,@Debit,@BillType,@Discount)", "BillNo", Convert.ToInt32(billNumber), "CustomerName", textBoxCustomer.Text, "CustomerPhone", textBoxCustomerPh.Text, "NetAmount", Convert.ToDouble(0), "AmountPaid", Convert.ToDouble(0), "Date", formattedDate, "Credit", payCredit, "Debit", payDebit, "BillType", billType, "Discount", Convert.ToString(0));
                    }
                    cc.CloseReader();
                    cc.CloseConnection();
                }
            }
            else
            {
                foreach (DataRow dg in dt1.Rows)
                {
                    string gridId = dg[0].ToString();
                    string gridName = dg[1].ToString();
                    string gridRate = dg[2].ToString();
                    string gridQuantity = dg[3].ToString();
                    string gridAmount = dg[4].ToString();
                    cc.OpenConnection();
                    cc.DataReader("insert into BillStock(BillNo,ItemNo,Product,Price,ProductQuantity,TotalPrice,gstPercent,gstRate,netGst,NetAmount,Discount,SaleDate,CustomerName,CustomerPhone,Credit,Debit,SalesReturn,ActualBillAmnt,CashPaid,Particulars,BillType) values('" + billNumber + "','" + gridId + "','" + gridName + "','" + gridRate + "','" + gridQuantity + "','" + gridAmount + "',0,0,0,'" + netAmount + "','" + txtBox_Discount.Text + "','" + formattedDate + "','" + textBoxCustomer.Text + "','" + textBoxCustomerPh.Text + "','" + payCredit + "','" + payDebit + "','" + Convert.ToString(returnAmnt) + "','" + Convert.ToString(newBillAmnt) + "','" + paidCash + "','" + paymentType + "','" + billType + "')");
                    cc.CloseReader();
                    cc.ExecuteQuery("update Stock set quantity = quantity - " + Convert.ToInt32(gridQuantity) + " where itemid = " + Convert.ToInt32(gridId) + "");
                    if (Convert.ToDouble(netAmount) > 0)
                    {
                        cc.CustomerTransactions("insert into CustomerTransactions(BillNo,CustomerName,CustomerPhone,NetAmount,AmountPaid,Date,Credit,Debit,BillType,Discount) values(@BillNo,@CustomerName,@CustomerPhone,@NetAmount,@AmountPaid,@Date,@Credit,@Debit,@BillType,@Discount)", "BillNo", Convert.ToInt32(billNumber), "CustomerName", textBoxCustomer.Text, "CustomerPhone", textBoxCustomerPh.Text, "NetAmount", Convert.ToDouble(netAmount), "AmountPaid", Convert.ToDouble(paidCash), "Date", formattedDate, "Credit", payCredit, "Debit", payDebit, "BillType", billType, "Discount", Convert.ToString(0));
                    }
                    else
                    {
                        cc.CustomerTransactions("insert into CustomerTransactions(BillNo,CustomerName,CustomerPhone,NetAmount,AmountPaid,Date,Credit,Debit,BillType,Discount) values(@BillNo,@CustomerName,@CustomerPhone,@NetAmount,@AmountPaid,@Date,@Credit,@Debit,@BillType,@Discount)", "BillNo", Convert.ToInt32(billNumber), "CustomerName", textBoxCustomer.Text, "CustomerPhone", textBoxCustomerPh.Text, "NetAmount", Convert.ToDouble(0), "AmountPaid", Convert.ToDouble(0), "Date", formattedDate, "Credit", payCredit, "Debit", payDebit, "BillType", billType, "Discount", Convert.ToString(0));
                    }
                    cc.CloseReader();
                    cc.CloseConnection();
                }
            }
        }

        private void PrintTheBill()
        {
            if(textBoxDiscountCard.Text != "Enter Card Id" && textBoxDiscountCard.Text != "")
            {
                discountCardAmount();
            }
            MessageBox.Show("Bill No :'" + billNumber + "' Saved Successfully.....");
            bno = Convert.ToInt32(billNumber);
            string msg = "Do you want to print bill?";
            string txt = "Bill Print";
            MessageBoxButton btn = MessageBoxButton.YesNo;
            MessageBoxResult rslt = MessageBox.Show(msg, txt, btn);
            switch (rslt)
            {
                case MessageBoxResult.Yes:
                    convIntBill = Convert.ToInt32(billNumber);
                    clearList();
                    bNO = Convert.ToInt32(bno);
                    generateBill();
                    break;

                case MessageBoxResult.No:
                    ClearGrid();
                    ClearCustomerTxtBox();
                    break;
            }
            ClearGrid();
            ClearCustomerTxtBox();
            lbSuggestion.Visibility = Visibility.Collapsed;
            txtItemId.Focus();
            comboBoxPayment.SelectedItem = "By Cash";
        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            newBillNo();
            if (txtBox_Discount.Text == "")
            {
                txtBox_Discount.Text = "0";
            }
            if (dataGrid1.Items.Count == 0 && dataGrid2.Items.Count == 0)
            {
                MessageBox.Show("No Bill To Save...!");
            }
            else
            {
                var time = DateTime.Now;
                formattedDate = time.ToString("dd-MMM-yy hh:mm:ss tt");
                netAmount = Convert.ToString(txtBlkNetAmount.Text);
                gstTotal = Convert.ToDouble(totalGst);
                string txtbn = Convert.ToString(billNumber);
                int tbn = Convert.ToInt32(billNumber);
                if (selectedPayment == "By Cash")
                {
                    payCredit = Convert.ToString(0);
                    payDebit = Convert.ToString(0);
                    paymentType = "By Cash";
                    paidCash = netAmount;
                    if (textBoxCustomer.Text == "Customer Name" || textBoxCustomerPh.Text == "Phone No")
                    {
                        textBoxCustomer.Text = "Cust_" + billNumber;
                        var date = DateTime.Now;
                        string presentDate = date.ToString("dd-MMM-yy hh:mm:ss tt");
                        textBoxCustomerPh.Text = "Ph_" + presentDate;
                        SaveBillFromGrid();
                    }
                    else if (textBoxCustomer.Text == "" || textBoxCustomerPh.Text == "")
                    {
                        textBoxCustomer.Text = "Cust_" + billNumber;
                        var date = DateTime.Now;
                        string presentDate = date.ToString("dd-MMM-yy hh:mm:ss tt");
                        textBoxCustomerPh.Text = "Ph_" + presentDate;
                        SaveBillFromGrid();
                    }
                    else
                    {
                        SaveBillFromGrid();
                    }
                    PrintTheBill();
                }
                else if (selectedPayment == "By Credit")
                {
                    if (textBoxCustomer.Text == "" || textBoxCustomerPh.Text == "")
                    {
                        MessageBox.Show("Enter customer details");
                    }
                    else if (textBoxCustomer.Text != "Customer Name" || textBoxCustomerPh.Text != "Phone No")
                    {
                        payCredit = Convert.ToString(txtBlkNetAmount.Text);
                        payDebit = Convert.ToString(0);
                        paymentType = "By Credit";
                        paidCash = Convert.ToString(0);
                        SaveBillFromGrid();
                        PrintTheBill();
                    }
                    else
                    {
                        MessageBox.Show("Enter customer details");
                    }

                }
                else if (selectedPayment == "Multi Payment")
                {
                    if (textBoxCustomer.Text == "" || textBoxCustomerPh.Text == "")
                    {
                        MessageBox.Show("Enter customer details");
                    }
                    else if (textBoxCustomer.Text != "Customer Name" || textBoxCustomerPh.Text != "Phone No")
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
                        SaveBillFromGrid();
                        PrintTheBill();
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

        private void newBillNo()
        {
            cc.OpenConnection();
            if (checkBoxGST.IsChecked == true)
            {
                cc.DataReader("select top 1 BillNo from BillStock where BillType='GST' order by BillNo desc");
            }
            else
            {
                cc.DataReader("select top 1 BillNo from BillStock where BillType='NON_GST' order by BillNo desc");
            }
            if (cc.reader.HasRows)
            {
                while (cc.reader.Read())
                {
                    int bnum = Convert.ToInt32(cc.reader["BillNo"]);
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

        private void dataGrid1_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void textBoxCustomer_LostFocus(object sender, RoutedEventArgs e)
        {
            lbSuggestion.Visibility = Visibility.Collapsed;
        }

        private void dataGrid1_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                try
                {
                    int selectedIndex = dataGrid1.SelectedIndex;
                    if (dt.Rows.Count == 1 && selectedIndex == 0)
                    {
                        dt.Clear();
                        txtBlkNetAmount.Text = "";
                        id.list.Clear();
                    }
                    else if (dt.Rows.Count > 1)
                    {
                        id.list.RemoveAt(selectedIndex);
                        dt.Rows.RemoveAt(selectedIndex);
                        dataGrid1.ItemsSource = null;
                        dataGrid1.ItemsSource = dt.DefaultView;
                        DisplayNetAmount();
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
                txtItemId.Focus();
            }
        }

        private void btn_Sales_Return_Click(object sender, RoutedEventArgs e)
        {
            srbno = Convert.ToInt32(billNumber);
            SalesReturn sr = new SalesReturn();
            sr.InputChanged += OnDialogInputChanged;
            sr.Show();
        }

        private void OnDialogInputChanged(object sender, SalesReturn.DialogInputEventArgs e)
        {
            var sr = sender as SalesReturn;
            if (txtBlkNetAmount.Text == "")
            {
                txtBlkNetAmount.Text = "0";
            }
            dataGridSR.ItemsSource = sr.dtSR.DefaultView;
            dataGridSR.Visibility = Visibility.Visible;
            salereturnhead.Visibility = Visibility.Visible;
            returnAmnt = Convert.ToDouble(sr.txtBlkNetAmount.Text);
            newBillAmnt = Convert.ToDouble(txtBlkNetAmount.Text);
            toPay = newBillAmnt - returnAmnt;
            correctBillAmount = Convert.ToString(toPay);
            txtBlkNetAmount.Text = Convert.ToString(toPay);
        }

        //private void GetCustomerDetails()
        //{
        //    lbl_Credit.Visibility = Visibility.Visible;
        //    textBox_Credit.Visibility = Visibility.Visible;
        //    cc.OpenConnection();
        //    cc.DataReader("select CustomerName,CustomerPhone,Credit from BillStock where BillNo = " + billNumber1 + " ");
        //    while (cc.reader.Read())
        //    {
        //        textBoxCustomer.Text = cc.reader["CustomerName"].ToString();
        //        textBoxCustomerPh.Text = cc.reader["CustomerPhone"].ToString();
        //        textBox_Credit.Text = cc.reader["Credit"].ToString();
        //    }
        //    cc.CloseReader();
        //    cc.CloseConnection();
        //}

        private void txtItemId_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var txtItemId = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void textBoxCustomerPh_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBoxCustomerPh = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void btn_Sales_History_Click(object sender, RoutedEventArgs e)
        {
            if(MainWindow.userName=="admin")
            {
                cc.OpenConnection();
                if (cc.BillPreview1("select COUNT(*) from BillStock", billExistValue) > 0)
                {
                    cc.CloseConnection();
                    SalesHistory sh = new SalesHistory();
                    this.NavigationService.Navigate(sh);
                }
                else
                {
                    MessageBox.Show("No Bills to Show");
                }
            }
            else
            {
                cc.OpenConnection();
                if (cc.BillPreview1("select COUNT(*) from BillStock where BillType='GST'", billExistValue) > 0)
                {
                    cc.CloseConnection();
                    SalesHistory sh = new SalesHistory();
                    this.NavigationService.Navigate(sh);
                }
                else
                {
                    MessageBox.Show("No Bills to Show");
                }
            }
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
            double netAmnt = Convert.ToDouble(txtBlkNetAmount.Text);
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
                Report.PrintToPrinter(1, false, 0, 15);
            }
            catch (Exception)
            {
                MessageBox.Show("No printer connected");
            }
            
        }

        private void addBill()
        {
            cc.OpenConnection();
            if(checkBoxGST.IsChecked==true)
            {
                cc.DataReader("select * from BillStock where BillNo = " + bNO + " and BillType='GST'");
            }
            else
            {
                cc.DataReader("select * from BillStock where BillNo = " + bNO + " and BillType='NON_GST'");
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

        private void CustomerAddbutton_Click(object sender, RoutedEventArgs e)
        {
            CustomerAddWindow caw = new CustomerAddWindow();
            lbSuggestion.SelectedItem = null;
            textBoxCustomer.Text = "";
            textBoxCustomerPh.Text = "";
            lbSuggestion.Visibility = Visibility.Collapsed;
            caw.InputChanged += OnDialogInputChanged;
            caw.Show();
        }

        private void OnDialogInputChanged(object sender, CustomerAddWindow.DialogInputEventArgs e)
        {
            var csw = sender as CustomerAddWindow;
            customerPhone = csw.textBoxCustomerPhone.Text;
            customerName = csw.textBoxCustomerName.Text+"  "+customerPhone;
            customerList.Add(customerName);
            customerList.Sort();
            lbSuggestion.SelectedItem = customerName;
            textBoxCustomerPh.Text = customerPhone;
        }

        private void txtBox_DiscountPercent_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateDiscount_And_DiscountPercent();
            if (txtBox_DiscountPercent.Text == "")
            {
                txtBox_Discount.Text = "";
            }
        }

        private void txtBox_Discount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(dataGrid1.Items.Count != 1)
            {
                if (txtBox_Discount.Text == "")
                {
                    txtBlkNetAmount.Text = Convert.ToString(correctBillAmount);
                }
                else
                {
                    double disc = Convert.ToDouble(txtBox_Discount.Text);
                    double result = Convert.ToDouble(correctBillAmount) - disc;
                    txtBlkNetAmount.Text = Convert.ToString(result);
                    CalculateDiscount_And_DiscountPercent();
                    if (txtBox_Discount.Text == "")
                    {
                        txtBox_DiscountPercent.Text = "";
                    }
                }
                if (txtBox_Discount.Text == "")
                {
                    txtBox_DiscountPercent.Text = "";
                }
                roundAfterDiscount = Convert.ToDouble(txtBlkNetAmount.Text);
            }
        }

        private void CalculateDiscount_And_DiscountPercent()
        {
            float originalRate;
            float discountCash;
            float discountPercent;
            float dpResult;
            float dResult;
            if (txtBox_Discount.IsFocused && float.TryParse(correctBillAmount, out originalRate) && float.TryParse(txtBox_Discount.Text, out discountCash))
            {
                dpResult = ((100 * discountCash) / originalRate);
                txtBox_DiscountPercent.Text = Convert.ToString(dpResult);
            }
            else if (txtBox_DiscountPercent.IsFocused && float.TryParse(correctBillAmount, out originalRate) && float.TryParse(txtBox_DiscountPercent.Text, out discountPercent))
            {
                dResult = ((originalRate / 100) * discountPercent);
                txtBox_Discount.Text = Convert.ToString(dResult);
            }
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            int netBillAmount;
            if (txtBox_Discount.Text == "")
            {
                netBillAmount = Convert.ToInt32(roundCorrectBill);
                txtBlkNetAmount.Text = Convert.ToString(netBillAmount);
            }
            else
            {
                netBillAmount = Convert.ToInt32(roundAfterDiscount);
                txtBlkNetAmount.Text = Convert.ToString(netBillAmount);
            }
        }

        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (txtBox_Discount.Text == "")
            {
                txtBlkNetAmount.Text = Convert.ToString(correctBillAmount);
            }
            else
            {
                txtBlkNetAmount.Text = Convert.ToString(roundAfterDiscount);
            }
        }

        private void discountCardAmount()
        {
            cc.OpenConnection();
            string custphone = textBoxCustomerPh.Text;
            double finalamount = Convert.ToDouble(txtBlkNetAmount.Text);
            double discountCardAmount = ((finalamount / 100) * 10) + Convert.ToDouble(textBlockCardBalance.Text);
            cc.ExecuteQuery("update CustomerDetails set CardPoints ='" + discountCardAmount.ToString() + "' where CustomerPhone='" + custphone + "' ");
            cc.CloseConnection();
        }

        private void DiscountCardCalculation()
        {
            double balance = Convert.ToDouble(textBlockCardBalance.Text);
            double netamount = Convert.ToDouble(txtBlkNetAmount.Text);
            double newnetamount;
            if (balance >= 500)
            {
                if (((netamount / 100) * 20) > balance)
                {
                    textBlockCardBalance.Text = "0";
                    txtBlkNetAmount.Text = Convert.ToString(netamount - balance);
                    txtBox_Discount.Text = Convert.ToString(balance);
                }
                else
                {
                    newnetamount = balance - ((netamount / 100) * 20);
                    textBlockCardBalance.Text = Convert.ToString(newnetamount);
                    txtBlkNetAmount.Text = Convert.ToString(netamount - ((netamount / 100) * 20));
                    txtBox_Discount.Text = Convert.ToString((netamount / 100) * 20);
                }

            }
            else
            {
                MessageBox.Show("Insufficient Card balance!");
                checkBoxCard.IsChecked = false;

            }
        }

        private void textBoxDiscountCard_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            try
            {
                if (e.Key == Key.Return)
                {
                    cc.OpenConnection();
                    string cardId;
                    cardId = textBoxDiscountCard.Text;
                    cc.DataReader("select CustomerName,CustomerPhone,CardPoints from CustomerDetails where CardId ='" + cardId + "' ");
                    while (cc.reader.Read())
                    {

                        string custName = cc.reader["CustomerName"].ToString();
                        string custPhn = cc.reader["CustomerPhone"].ToString();
                        CardPoints = Convert.ToDouble(cc.reader["CardPoints"].ToString());
                        textBlockCardBalance.Text = Convert.ToString(CardPoints);
                        textBoxCustomer.Text = custName + "  " + custPhn;
                        textBoxCustomerPh.Text = cc.reader["CustomerPhone"].ToString();
                        lbSuggestion.Visibility = Visibility.Collapsed;
                    }
                    cc.CloseReader();
                    cc.CloseConnection();
                    textBlockCard.Visibility = Visibility.Visible;
                    textBlockCardBalance.Visibility = Visibility.Visible;
                    checkBoxCard.IsEnabled = true;
                }
            }
            catch (Exception)
            {
                if (textBoxDiscountCard.Text == "")
                    MessageBox.Show("Enter The Card ID!.");
            }
        }

        private void checkBoxCard_Checked(object sender, RoutedEventArgs e)
        {
            if(dt.Rows.Count>0)
            {
                DiscountCardCalculation();
            }
            else
            {
                MessageBox.Show("No Items are Purchased");
                checkBoxCard.IsChecked = false;
            }
        }

        private void checkBoxCard_Unchecked(object sender, RoutedEventArgs e)
        {
            txtBlkNetAmount.Text = correctBillAmount;
            textBlockCardBalance.Text = Convert.ToString(CardPoints);
            txtBox_Discount.Text = "";
        }

        private void textBoxDiscountCard_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            textBoxDiscountCard.Text = string.Empty;
        }

    }
}
