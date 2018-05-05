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
        public static string formattedDate, itemName;
        public CustomerOrdering()
        {
            InitializeComponent();
            ClearGrid();
            textBoxItemName.Focus();
            var time = DateTime.Now;
            string formattedDate = time.ToString("dd-MMM-yy");
            labelDate.Content = formattedDate;
            tableDisplay();
        }

        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            CustomerTab ct = new CustomerTab();
            this.NavigationService.Navigate(ct);
        }

        private void tableDisplay()
        {
            cc.OpenConnection();
            cc.DataGridDisplay("select * from CustomerOrderGrid");
            dataCustomerOrder.ItemsSource = cc.dt.AsDataView();
            dataCustomerOrder.Visibility = System.Windows.Visibility.Visible;
            cc.CloseConnection();
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
            cc.OpenConnection();
            cc.DataReader("delete * from CustomerOrderGrid");
            cc.CloseReader();
            cc.CloseConnection();
            tableDisplay();
        }

        private void textBoxAdvance_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //if (textBoxItemName.Text == "" || textBoxQuantity.Text == "" || textBoxName.Text == "" || textBoxPhone.Text == "" || textBoxAdvance.Text == "")
            //try
            {
                if (e.Key == Key.Return)
                {
                    cc.OpenConnection();
                    cc.ExecuteQuery("insert into CustomerOrderGrid(OItemName,OQuantity,OCustomerName,OCustomerPhone,OAdvance,OrderDate) values('" + textBoxItemName.Text + "','" + textBoxQuantity.Text + "','" + textBoxName.Text + "','" + textBoxPhone.Text + "','" + textBoxAdvance.Text + "','" + labelDate.Content + "')");
                    cc.CloseConnection();
                    tableDisplay();
                    ClearTextBoxes();
                    textBoxItemName.Focus();
                }
            }
            /*catch(Exception)
            {
                MessageBox.Show("Enter the details..!");
            }*/
        }

        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            ClearGrid();
            textBoxItemName.Focus();
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            cc.OpenConnection();
            cc.ExecuteQuery("insert into OrderedItems(OItemName,OQuantity,OCustomerName,OCustomerPhone,OAdvance,OrderDate) select OItemName,OQuantity,OCustomerName,OCustomerPhone,OAdvance,OrderDate from CustomerOrderGrid");
            cc.CloseConnection();
            MessageBox.Show("Order Saved..!");
            ClearGrid();
            textBoxItemName.Focus();
        }

        private void buttonListOrder_Click(object sender, RoutedEventArgs e)
        {
            cc.OpenConnection();
            cc.ExecuteQuery("insert into CustomerOrderGrid(OItemName,OQuantity,OCustomerName,OCustomerPhone,OAdvance,OrderDate) select OItemName,OQuantity,OCustomerName,OCustomerPhone,OAdvance,OrderDate from OrderedItems");
            cc.CloseConnection();
            tableDisplay();
        }

        private void btn_Delete_Row_Click(object sender, RoutedEventArgs e)
        {
            if(dataCustomerOrder.SelectedItem != null)
            {
                object item = dataCustomerOrder.SelectedItem;
                itemName = (dataCustomerOrder.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;
                tableRowDelete();
                databaseRowDelete();
                tableDisplay();
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
            cc.ExecuteQuery("Delete * from OrderedItems where OItemName='" + itemName + "'");
            cc.CloseConnection();
        }

        private void dataCustomerOrder_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void tableRowDelete()
        {
            cc.OpenConnection();
            cc.ExecuteQuery("Delete * from CustomerOrderGrid where OItemName='" + itemName + "'");
            cc.CloseConnection();
        }
    }
}
