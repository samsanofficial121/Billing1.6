using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
using System.Windows.Shapes;

namespace Billing
{
    /// <summary>
    /// Interaction logic for SalesReturn.xaml
    /// </summary>
    public partial class SalesReturn : Window
    {
        ConnectionClass cc = new ConnectionClass();
        SellPage sp = new SellPage();
        public static int idCount, quantityCell, isExist,srbno;
        public static string selectedItemGst, itemi, itname, rat, quantyty, spric, gstper, gstam, cellqty, selectedRowId;
        public static double Netgst;
        DataTable dt = new DataTable();
        public DataTable dtSR = new DataTable();
        Id id = new Id();
        public SalesReturn()
        {
            InitializeComponent();
            srbno = SellPage.srbno;
            txtItemId.Focus();
            dtSR.Clear();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
            ClearReturnGrid();
        }

        private void dataGrid1_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
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
        }

        private void dataGrid1_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
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
            DataRow dr = dt.Select("Id=" + selectedRowId + "").FirstOrDefault();
            if (dr != null)
            {
                dr["Quantity"] = Convert.ToInt32(cellqty);
                dr["AddedGST"] = Convert.ToInt32(cellqty) * Convert.ToDouble(gstam);
                dr["Amount"] = Convert.ToInt32(cellqty) * Convert.ToDouble(spric);
            }
            DisplayNetAmount();
        }

        public void UpdateGridRow()
        {
            int index = -1;
            DataRow[] rows = dt.Select("Id = " + txtItemId.Text + "");
            if (rows.Count() > 0)
            {
                index = dt.Rows.IndexOf(rows[0]);
            }
            quantityCell = Convert.ToInt32(dt.Rows[index][3]);
            DataRow dr = dt.Select("Id=" + txtItemId.Text + "").FirstOrDefault();
            if (dr != null)
            {
                dr["Quantity"] = quantityCell + 1;
                dr["AddedGST"] = (quantityCell + 1) * Convert.ToDouble(gstam);
                dr["Amount"] = (quantityCell + 1) * Convert.ToDouble(spric);
            }
        }

        private void InsertNewRow()
        {
            dt.Rows.Add(new string[] { itemi, itname, spric, "1", gstper, gstam, spric });
            dataGrid1.ItemsSource = dt.DefaultView;
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
                        ClearReturnGrid();
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

        private void DisplayNetAmount()
        {
            double NetA = Convert.ToDouble(dt.Compute("Sum(Amount)", ""));
            Netgst = Convert.ToDouble(dt.Compute("Sum(AddedGST)", ""));
            txtBlkNetAmount.Text = Convert.ToString(NetA + Netgst);
        }

        private void GetFromStock()
        {
            cc.OpenConnection();
            cc.DataReader("select itemid,iname,quantity,sprice,gst_percent,gst_amount from Stock where itemid= " + Convert.ToInt32(txtItemId.Text) + "");
            while (cc.reader.Read())
            {
                itemi = cc.reader["itemid"].ToString();
                itname = cc.reader["iname"].ToString();
                quantyty = cc.reader["quantity"].ToString();
                spric = cc.reader["sprice"].ToString();
                gstper = cc.reader["gst_percent"].ToString();
                gstam = cc.reader["gst_amount"].ToString();
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void ClearReturnGrid()
        {
            dt.Clear();
            txtBlkNetAmount.Text = "";
            id.list.Clear();
            txtItemId.Focus();
        }

        private void StoreId()
        {
            id.StoreIId(idCount);
        }

        private void txtItemId_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var DamageItemId = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void buttonClearGrid_Click(object sender, RoutedEventArgs e)
        {
            ClearReturnGrid();
        }

        public class DialogInputEventArgs : EventArgs
        {
            public string Input { get; set; }
        }

        public event EventHandler<DialogInputEventArgs> InputChanged = delegate { };

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            if (txtBlkNetAmount.Text == "")
            {
                MessageBox.Show("Enter an item to return");
            }
            else
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
                    cc.ExecuteQuery("insert into SalesReturn(billno,sritemid,sritemname,srrate,srquantity,srgstpercent,srgstamount,sramount) values(" + srbno + ",'" + gridId + "','" + gridName + "','" + gridRate + "','" + gridQuantity + "','" + gridGstPercent + "','" + gridGst + "','" + gridAmount + "')");
                    cc.OpenConnection();
                }
                dtSR = dt.Copy();
                InputChanged(this, new DialogInputEventArgs() { Input = this.txtBlkNetAmount.Text });
                StockUpdate();
                SystemCommands.CloseWindow(this);
                ClearReturnGrid();
            }
        }

        private void txtItemId_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Return)
                {
                    cc.OpenConnection();
                    if (cc.BillPreview("select COUNT(*) from Stock where itemid = @value ", "@value", txtItemId.Text, isExist) > 0)
                    {
                        cc.CloseConnection();
                        GetFromStock();
                        idCount = Convert.ToInt32(txtItemId.Text);
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
                        txtItemId.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("Item does not exist in DB");
                        txtItemId.Text = "";
                        txtItemId.Focus();
                    }
                }
            }
            catch (Exception)
            {
                if (txtItemId.Text == "")
                {
                    MessageBox.Show("Enter Item ID..!");
                }
            }
        }

        private void StockUpdate()
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
                cc.ExecuteQuery("update Stock set quantity = quantity + " + Convert.ToInt32(gridQuantity) + " where itemid = " + Convert.ToInt32(gridId) + "");
                cc.CloseConnection();
            }
        }

    }
}