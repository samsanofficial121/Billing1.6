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
        private ReturnClass.DataAccess objDs;
        private ReturnClass.ReturnGridTable objBillToAdd;
        ConnectionClass cc = new ConnectionClass();
        public static int qty, idCount;
        public static string oldQty, selectedItemGst;
        public static double sendAmnt;
        Id id = new Id();
        public SalesReturn()
        {
            InitializeComponent();
            txtItemId.Focus();
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
            objDs = new ReturnClass.DataAccess();
            Display();
        }

        private void dataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            objBillToAdd = dataGrid1.SelectedItem as ReturnClass.ReturnGridTable;
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
                objDs.InsertReturnBill(objBillToAdd);
                Display();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void InsertDamageItem()
        {
            cc.OpenConnection();
            cc.InsertBill("insert into ReturnGridTable(Id,Name,Rate,Quantity,Amount,GSTPercent,AddedGST) select itemid,iname,sprice,?,sprice+gst_amount,gst_percent,gst_amount from Stock where itemid= " + txtItemId.Text + "", "Quantity", qty);
            cc.CloseConnection();
        }
        private void Display()
        {
            cc.OpenConnection();
            cc.DataGridDisplay("select * from ReturnGridTable");
            cc.da.Update(cc.dt);
            dataGrid1.ItemsSource = objDs.GetReturnBill();
            labelNetAmount.Text = cc.dt.Compute("Sum(Amount)", "").ToString();
            cc.CloseConnection();
        }
        private void UpdateQty()
        {
            QtySelect();
            qty = Convert.ToInt32(oldQty) + 1;
            cc.OpenConnection();
            cc.UpdateBillTable("Update ReturnGridTable set Quantity=? where Id= " + idCount + " ", "Quantity", qty);
            cc.CloseConnection();
        }

        private void QtySelect()
        {
            cc.OpenConnection();
            cc.DataReader("select Quantity from ReturnGridTable where Id= " + idCount + " ");
            while (cc.reader.Read())
            {
                oldQty = cc.reader["Quantity"].ToString();
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void UpdateAmount()
        {
            cc.OpenConnection();
            cc.ExecuteQuery("Update ReturnGridTable set Amount = Rate * Quantity + AddedGST where Id = " + idCount + " ");
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

        public void UpdateGstAmount()
        {
            GetSelectedItemGst();
            cc.OpenConnection();
            cc.UpdateGST("Update ReturnGridTable set AddedGST=?*Quantity where Id= " + idCount + " ", "ItemGST", selectedItemGst);
            cc.CloseConnection();
        }

        private void ClearReturnGrid()
        {
            id.list.Clear();
            cc.OpenConnection();
            cc.DataReader("delete * from ReturnGridTable");
            cc.CloseReader();
            cc.CloseConnection();
            Display();
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
            if (labelNetAmount.Text == "")
            {
                MessageBox.Show("Enter an item to return");
            }
            else
            {
                InputChanged(this, new DialogInputEventArgs() { Input = this.labelNetAmount.Text });
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
                    idCount = Convert.ToInt32(txtItemId.Text);
                    if (id.list.Contains(txtItemId.Text))
                    {
                        UpdateQty();
                        UpdateGstAmount();
                        UpdateAmount();
                    }
                    else
                    {
                        StoreId();
                        QtySelect();
                        qty = 1;
                        InsertDamageItem();
                    }
                    Display();
                    txtItemId.Text = "";
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
            cc.OpenConnection();
            cc.ExecuteQuery("update Stock as a inner join ReturnGridTable as b on a.itemid = b.Id set a.quantity = a.quantity + b.Quantity");
            cc.CloseConnection();
        }

    }
}