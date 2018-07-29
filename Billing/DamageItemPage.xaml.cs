using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Billing
{
    /// <summary>
    /// Interaction logic for DamageItemPage.xaml
    /// </summary>
    public partial class DamageItemPage : Page
    {
        public static int qty, idCount, quantityCell, isExist;
        public static string oldQty, DItemNo, DName, DRate, DQuantity, DTotal, DVid, DVname, DVphone, cellqty, selectedRowId;
        Id id = new Id();
        ConnectionClass cc = new ConnectionClass();
        DataTable dt = new DataTable();
        private int itemNo;

        public DamageItemPage()
        {
            InitializeComponent();
            DamageItemId.Focus();
            ClearDamageGrid();
            buttonDelete.Visibility = Visibility.Hidden;
        }

        private void backFromDamaged_Click(object sender, RoutedEventArgs e)
        {
            StockPage sp = new StockPage();
            this.NavigationService.Navigate(sp);
        }

        private void DamageItemId_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Return)
                {
                    cc.OpenConnection();
                    if (cc.BillPreview("select COUNT(*) from Stock where itemid = @value ", "@value", DamageItemId.Text, isExist) > 0)
                    {
                        cc.CloseConnection();
                        idCount = Convert.ToInt32(DamageItemId.Text);
                        GetFromStock();
                        if (id.list.Contains(DamageItemId.Text))
                        {
                            UpdateGridRow();
                        }
                        else
                        {
                            StoreId();
                            InsertNewRow();
                        }
                        DamageItemId.Text = "";
                        buttonDelete.Visibility = Visibility.Hidden;
                        buttonList.Visibility = Visibility.Visible;
                        buttonSaveDamaged.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        MessageBox.Show("Item does not exist in DB");
                        DamageItemId.Text = "";
                        DamageItemId.Focus();
                    }
                }
            }
            catch (Exception)
            {
                if (DamageItemId.Text == "")
                {
                    MessageBox.Show("Enter Item ID..!");
                }
            }

        }

        private void GetFromStock()
        {
            cc.OpenConnection();
            cc.CreateView("CREATE VIEW DamagedItems AS Select Stock.itemid,Stock.iname,Stock.rate,TransactionDetails.Vid,TransactionDetails.Vname,TransactionDetails.Vphone from Stock,TransactionDetails where Stock.Bno=TransactionDetails.Bno");
            cc.DataReader("select * from DamagedItems where itemid=" + DamageItemId.Text + "");
            while (cc.reader.Read())
            {
                DItemNo = cc.reader["itemid"].ToString();
                DName = cc.reader["iname"].ToString();
                DRate = cc.reader["rate"].ToString();
                DVid = cc.reader["Vid"].ToString();
                DVname = cc.reader["Vname"].ToString();
                DVphone = cc.reader["Vphone"].ToString();
            }
            cc.CloseReader();
            cc.DropView("DROP VIEW DamagedItems");
            cc.CloseConnection();
        }

        private void ClearDamageGrid()
        {
            id.list.Clear();
            dt.Clear();
        }

        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            ClearDamageGrid();
            DamageItemId.Focus();
            buttonDelete.Visibility = Visibility.Hidden;
            buttonList.Visibility = Visibility.Visible;
            buttonSaveDamaged.Visibility = Visibility.Visible;

        }

        private void buttonSaveDamaged_Click(object sender, RoutedEventArgs e)
        {
            foreach (DataRow dg in dt.Rows)
            {
                string gridId = dg[0].ToString();
                string gridName = dg[1].ToString();
                string gridRate = dg[2].ToString();
                string gridQuantity = dg[3].ToString();
                string gridTotal = dg[4].ToString();
                string gridVid = dg[5].ToString();
                string gridVname = dg[6].ToString();
                string gridVphone = dg[7].ToString();
                cc.OpenConnection();
                cc.ExecuteQuery("insert into DamagedStock(DItemNo,DName,DRate,DQuantity,DTotal,DVid,DVname,DVphone) values('" + gridId + "','" + gridName + "','" + gridRate + "','" + gridQuantity + "','" + gridTotal + "','" + gridVid + "','" + gridVname + "','" + gridVphone + "')");
                cc.ExecuteQuery("update Stock set quantity = quantity - " + Convert.ToInt32(gridQuantity) + " where itemid = " + Convert.ToInt32(gridId) + "");
                cc.CloseConnection();
            }
            MessageBox.Show("Item Inserted");
            ClearDamageGrid();
            DamageItemId.Focus();
        }

        private void StoreId()
        {
            id.StoreIId(idCount);
        }

        private void DamageItemId_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var DamageItemId = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void dataGridDamage_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dt.Columns.Clear();
            dt.Columns.Add("DItemNo", typeof(string));
            dt.Columns.Add("DName", typeof(string));
            dt.Columns.Add("DRate", typeof(double));
            dt.Columns.Add("DQuantity", typeof(string));
            dt.Columns.Add("DTotal", typeof(double));
            dt.Columns.Add("DVid", typeof(string));
            dt.Columns.Add("DVname", typeof(string));
            dt.Columns.Add("DVphone", typeof(string));
            DamageItemId.Focus();
        }

        private void dataGridDamage_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            DataRowView rowview = dataGridDamage.SelectedItem as DataRowView;
            string oldq = rowview.Row[3].ToString();
            selectedRowId = rowview.Row[0].ToString();
            FrameworkElement element_Quantity = dataGridDamage.Columns[3].GetCellContent(e.Row);
            if (element_Quantity.GetType() == typeof(TextBox))
            {
                var qty = ((TextBox)element_Quantity).Text;
                cellqty = qty;
            }
            DataRow dr = dt.Select("DItemNo=" + selectedRowId + "").FirstOrDefault();
            if (dr != null)
            {
                dr["DQuantity"] = Convert.ToInt32(cellqty);
                dr["DTotal"] = Convert.ToInt32(cellqty) * Convert.ToDouble(DRate);
            }
        }

        public void UpdateGridRow()
        {
            int index = -1;
            DataRow[] rows = dt.Select("DItemNo=" + DamageItemId.Text + "");
            if (rows.Count() > 0)
            {
                index = dt.Rows.IndexOf(rows[0]);
            }
            quantityCell = Convert.ToInt32(dt.Rows[index][3]);
            DataRow dr = dt.Select("DItemNo=" + DamageItemId.Text + "").FirstOrDefault();
            if (dr != null)
            {
                dr["DQuantity"] = quantityCell + 1;
                dr["DTotal"] = (quantityCell + 1) * Convert.ToDouble(DRate);
            }
        }

        private void InsertNewRow()
        {
            dt.Rows.Add(new string[] { DItemNo, DName, DRate, "1", DRate, DVid, DVname, DVphone });
            dataGridDamage.ItemsSource = dt.DefaultView;
        }

        private void dataGridDamage_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                try
                {
                    int selectedIndex = dataGridDamage.SelectedIndex;
                    if (dt.Rows.Count > 0)
                    {
                        dt.Rows.RemoveAt(selectedIndex);
                        id.list.RemoveAt(selectedIndex);
                        dataGridDamage.ItemsSource = null;
                        dataGridDamage.ItemsSource = dt.DefaultView;
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

        private void GetFromDamageStock()
        {
            cc.OpenConnection();
            cc.DataReader("select DItemNo,DName,DRate,DQuantity,DTotal,DVid,DVname,DVphone from DamagedStock");
            while (cc.reader.Read())
            {
                DItemNo = cc.reader["DItemNo"].ToString();
                DName = cc.reader["DName"].ToString();
                DRate = cc.reader["DRate"].ToString();
                DQuantity = cc.reader["DQuantity"].ToString();
                DTotal = cc.reader["DTotal"].ToString();
                DVid = cc.reader["DVid"].ToString();
                DVname = cc.reader["DVname"].ToString();
                DVphone = cc.reader["DVphone"].ToString();
                dt.Rows.Add(new string[] { DItemNo, DName, DRate, DQuantity, DTotal, DVid, DVname, DVphone });
            }
            cc.CloseReader();
            dataGridDamage.ItemsSource = dt.DefaultView;
            cc.CloseConnection();
        }

        private void buttonList_Click(object sender, RoutedEventArgs e)
        {
            GetFromDamageStock();
            if(dt.Rows.Count<=0)
            {
                MessageBox.Show("No Items to List");
            }
            else
            {
                buttonDelete.Visibility = Visibility.Visible;
                buttonList.Visibility = Visibility.Hidden;
                buttonSaveDamaged.Visibility = Visibility.Hidden;
            }
            DamageItemId.Focus();
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridDamage.SelectedItem != null)
            {
                object item = dataGridDamage.SelectedItem;
                string itemNoString = (dataGridDamage.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;
                itemNo = Convert.ToInt32(itemNoString);
                dt.Rows.RemoveAt(dataGridDamage.SelectedIndex);
                dataGridDamage.ItemsSource = null;
                dataGridDamage.ItemsSource = dt.DefaultView;
                databaseRowDelete();
                MessageBox.Show("Item Deleted From Damage Stock");
                DamageItemId.Focus();
            }
            else
            {
                MessageBox.Show("Please select an item");
            }
        }

        private void databaseRowDelete()
        {
            cc.OpenConnection();
            cc.ExecuteQuery("Delete from DamagedStock where DItemNo=" + itemNo + "");
            cc.CloseConnection();
        }

    }
}
