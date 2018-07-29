using System;
using System.Collections.Generic;
using System.Data;
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

namespace Billing
{
    /// <summary>
    /// Interaction logic for CustomerOrdering.xaml
    /// </summary>
    public partial class CustomerOrdering : Page
    {
        ConnectionClass cc = new ConnectionClass();
        DataTable dt = new DataTable();
        DataTable listdt = new DataTable();
        public static string formattedDate, itemName, OName, OQuantity, OCname, OCphone, OAdvanceAmnt, ODate;

        public CustomerOrdering()
        {
            InitializeComponent();
            ClearGrid();
            textBoxItemName.Focus();
            var time = DateTime.Now;
            string formattedDate = time.ToString("dd-MMM-yy hh:mm:ss tt");
            tblockDate.Text = formattedDate;
        }

        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            CustomerTab ct = new CustomerTab();
            this.NavigationService.Navigate(ct);
        }

        private void ClearTextBoxes()
        {
            textBoxItemName.Text = "";
            textBoxQuantity.Text = "";
            textBoxName.Text = "";
            textBoxPhone.Text = "";
            textBoxAdvance.Text = "";
        }

        private void ClearGrid()
        {
            dt.Clear();
            listdt.Clear();
        }

        private void textBoxAdvance_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Return)
                {
                    if (textBoxItemName.Text == "" || textBoxQuantity.Text == "" || textBoxName.Text == "" || textBoxPhone.Text == "" || textBoxAdvance.Text == "")
                    {
                        MessageBox.Show("Enter the details..!");
                    }
                    else
                    {
                        dt.Rows.Add(new string[] { textBoxItemName.Text, textBoxQuantity.Text, textBoxName.Text, textBoxPhone.Text, textBoxAdvance.Text, tblockDate.Text });
                        dataCustomerOrder.ItemsSource = dt.DefaultView;
                        ClearTextBoxes();
                    }
                    textBoxItemName.Focus();
                }
            }
            catch(Exception)
            {
                MessageBox.Show("Enter the details..!");
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            dt.Columns.Add("OItemName", typeof(string));
            dt.Columns.Add("OQuantity", typeof(int));
            dt.Columns.Add("OCustomerName", typeof(string));
            dt.Columns.Add("OCustomerPhone", typeof(string));
            dt.Columns.Add("OAdvance", typeof(double));
            dt.Columns.Add("OrderDate", typeof(string));

            listdt.Columns.Add("OItemName", typeof(string));
            listdt.Columns.Add("OQuantity", typeof(int));
            listdt.Columns.Add("OCustomerName", typeof(string));
            listdt.Columns.Add("OCustomerPhone", typeof(string));
            listdt.Columns.Add("OAdvance", typeof(double));
            listdt.Columns.Add("OrderDate", typeof(string));
        }

        private void textBoxItemName_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                textBoxQuantity.Focus();
            }
        }

        private void textBoxQuantity_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                textBoxName.Focus();
            }
        }

        private void textBoxName_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                textBoxPhone.Focus();
            }
        }

        private void textBoxPhone_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                textBoxAdvance.Focus();
            }
        }

        private void dataCustomerOrder_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (dt.Rows.Count == 1)
                {
                    dt.Rows.RemoveAt(dataCustomerOrder.SelectedIndex);
                    dataCustomerOrder.ItemsSource = null;
                    dataCustomerOrder.ItemsSource = dt.DefaultView;
                }
                else if (dt.Rows.Count > 1)
                {
                    dt.Rows.RemoveAt(dataCustomerOrder.SelectedIndex);
                    dataCustomerOrder.ItemsSource = null;
                    dataCustomerOrder.ItemsSource = dt.DefaultView;
                }
                else
                {
                    MessageBox.Show("No items to delete");
                }
            }
        }

        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            ClearGrid();
            textBoxItemName.Focus();
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            foreach (DataRow dg in dt.Rows)
            {
                string gridName = dg[0].ToString();
                string gridQuantity = dg[1].ToString();
                string gridCname = dg[2].ToString();
                string gridCphone = dg[3].ToString();
                string gridAdvance = dg[4].ToString();
                string gridOdate = dg[5].ToString();
                cc.OpenConnection();
                cc.ExecuteQuery("insert into OrderedItems(OItemName,OQuantity,OCustomerName,OCustomerPhone,OAdvance,OrderDate) values ('" + gridName + "', " + Convert.ToInt32(gridQuantity) + ",'" + gridCname + "','" + gridCphone + "','" + gridAdvance + "','" + gridOdate + "')");
                cc.CloseConnection();
            }
            MessageBox.Show("Order Saved..!");
            ClearGrid();
            textBoxItemName.Focus();
        }

        private void buttonListOrder_Click(object sender, RoutedEventArgs e)
        {
            listdt.Clear();
            cc.OpenConnection();
            cc.DataReader("select OItemName,OQuantity,OCustomerName,OCustomerPhone,OAdvance,OrderDate from OrderedItems");
            while (cc.reader.Read())
            {
                OName = cc.reader["OItemName"].ToString();
                OQuantity = cc.reader["OQuantity"].ToString();
                OCname = cc.reader["OCustomerName"].ToString();
                OCphone = cc.reader["OCustomerPhone"].ToString();
                OAdvanceAmnt = cc.reader["OAdvance"].ToString();
                ODate = cc.reader["OrderDate"].ToString();
                listdt.Rows.Add(new string[] { OName, OQuantity, OCname, OCphone, OAdvanceAmnt, ODate });
            }
            cc.CloseReader();
            cc.CloseConnection();
            if (listdt.Rows.Count <= 0)
            {
                MessageBox.Show("No Items to List");
            }
            else
            {
                dataCustomerOrder.ItemsSource = listdt.DefaultView;
            }
        }

        private void btn_Delete_Row_Click(object sender, RoutedEventArgs e)
        {
            if(dataCustomerOrder.SelectedItem != null)
            {
                object item = dataCustomerOrder.SelectedItem;
                itemName = (dataCustomerOrder.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;
                listdt.Rows.RemoveAt(dataCustomerOrder.SelectedIndex);
                dataCustomerOrder.ItemsSource = null;
                dataCustomerOrder.ItemsSource = listdt.DefaultView;
                databaseRowDelete();
                MessageBox.Show("Order Cleared");
            }
            else
            {
                MessageBox.Show("Please select an order");
            }
            
        }

        private void databaseRowDelete()
        {
            cc.OpenConnection();
            cc.ExecuteQuery("Delete from OrderedItems where OItemName='" + itemName + "'");
            cc.CloseConnection();
        }

        private void dataCustomerOrder_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
