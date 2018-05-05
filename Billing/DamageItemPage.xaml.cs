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
        public static int qty, idCount;
        public static string oldQty;
        Id id = new Id();
        ConnectionClass cc = new ConnectionClass();
        private int itemNo;

        public DamageItemPage()
        {
            InitializeComponent();
            DamageItemId.Focus();
            ClearDamageGrid();
            Display();
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
                    idCount = Convert.ToInt32(DamageItemId.Text);
                    if (id.list.Contains(DamageItemId.Text))
                    {
                        //QtySelect();
                        UpdateQty();
                        UpdateTotal();
                    }
                    else
                    {
                        StoreId();
                        QtySelect();
                        qty = 1;
                        InsertDamageItem();
                    }
                    Display();
                    DamageItemId.Text = "";
                }
            }
            catch(Exception)
            {
                if(DamageItemId.Text=="")
                {
                    MessageBox.Show("Enter Item ID..!");
                }
            }
            
        }
        private void InsertDamageItem()
        {
            cc.OpenConnection();
            cc.CreateView("CREATE VIEW DamagedItems AS Select Stock.itemid,Stock.iname,Stock.rate,TransactionDetails.Vid,TransactionDetails.Vname,TransactionDetails.Vphone from Stock,TransactionDetails where Stock.Bno=TransactionDetails.Bno");
            //cc.InsertBill("insert into DamageItems(DItemNo,DName,DRate,DQuantity,DTotal,DVid,DVname,DVphone) select itemid,iname,rate,?,rate,Vid,Vname,Vphone from Stock where itemid="+DamageItemId.Text+"","DQuantity",qty);
            cc.InsertBill("insert into DamageItems(DItemNo,DName,DRate,DQuantity,DTotal,DVid,DVname,DVphone) select itemid,iname,rate,?,rate,Vid,Vname,Vphone from DamagedItems where itemid=" + DamageItemId.Text + "", "DQuantity", qty);
            cc.DropView("DROP VIEW DamagedItems");
            cc.CloseConnection();
        }
        private void Display()
        {
            cc.OpenConnection();
            cc.DataGridDisplay("select * from DamageItems");
            dataGridDamage.ItemsSource = cc.dt.AsDataView();
            dataGridDamage.Visibility = System.Windows.Visibility.Visible;
            cc.CloseConnection();
        }
        private void UpdateQty()
        {
            QtySelect();
            qty = Convert.ToInt32(oldQty) + 1;
            cc.OpenConnection();
            cc.UpdateBillTable("Update DamageItems set DQuantity=? where DItemNo= " + idCount + " ", "DQuantity", qty);
            cc.CloseConnection();
        }

        private void QtySelect()
        {
            cc.OpenConnection();
            cc.DataReader("select DQuantity from DamageItems where DItemNo= " + idCount + " ");
            while (cc.reader.Read())
            {
                oldQty = cc.reader["DQuantity"].ToString();
            }
            cc.CloseReader();
            cc.CloseConnection();
        }
        private void UpdateTotal()
        {
            cc.OpenConnection();
            cc.ExecuteQuery("Update DamageItems set DTotal=DRate*DQuantity where DItemNo= " + idCount + " ");
            cc.CloseConnection();
        }

        private void ClearDamageGrid()
        {
            id.list.Clear();
            cc.OpenConnection();
            cc.DataReader("delete * from DamageItems");
            cc.CloseReader();
            cc.CloseConnection();
            Display();
        }

        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            ClearDamageGrid();
            DamageItemId.Focus();
        }

        private void buttonSaveDamaged_Click(object sender, RoutedEventArgs e)
        {
            cc.OpenConnection();
            cc.ExecuteQuery("insert into DamagedStock(DItemNo,DName,DRate,DQuantity,DTotal,DVid,DVname,DVphone) select DItemNo,DName,DRate,DQuantity,DTotal,DVid,DVname,DVphone from DamageItems");
            cc.CloseConnection();
            StockUpdate();
            MessageBox.Show("Item inserted");
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

        private void buttonList_Click(object sender, RoutedEventArgs e)
        {
            cc.OpenConnection();
            cc.ExecuteQuery("insert into DamageItems(DItemNo,DName,DRate,DQuantity,DTotal,DVid,DVname,DVphone) select DItemNo,DName,DRate,DQuantity,DTotal,DVid,DVname,DVphone from DamagedStock");
            cc.CloseConnection();
            Display();
            DamageItemId.Focus();
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridDamage.SelectedItem != null)
            {
                object item = dataGridDamage.SelectedItem;
                string itemNoString = (dataGridDamage.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;
                itemNo = Convert.ToInt32(itemNoString);
                tableRowDelete();
                databaseRowDelete();
                Display();
                MessageBox.Show("Item Cleared");
                DamageItemId.Focus();
            }
            else
            {
                MessageBox.Show("Please select an order");
            }
        }

        private void databaseRowDelete()
        {
            cc.OpenConnection();
            cc.ExecuteQuery("Delete * from DamagedStock where DItemNo=" + itemNo + "");
            cc.CloseConnection();
        }

        private void tableRowDelete()
        {
            cc.OpenConnection();
            cc.ExecuteQuery("Delete * from DamageItems where DItemNo=" + itemNo + "");
            cc.CloseConnection();
        }

        private void StockUpdate()
        {
            cc.OpenConnection();
            cc.ExecuteQuery("update Stock as a inner join DamageItems as b on a.itemid = b.DItemNo set a.quantity = a.quantity - b.DQuantity");
            cc.CloseConnection();
        }
    }
}
