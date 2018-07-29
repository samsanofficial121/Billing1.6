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
    /// Interaction logic for CustomerList.xaml
    /// </summary>
    public partial class CustomerList : Page
    {
        ConnectionClass cc = new ConnectionClass();
        public static string custPhone;
        public CustomerList()
        {
            InitializeComponent();
            MainWindow.isgst = 0;
            if (MainWindow.userName != "admin")
            {
                checkBoxGST.Visibility = Visibility.Collapsed;
            }
            LoadGrid();
        }

        private void LoadGrid()
        {
            cc.OpenConnection();
            if (MainWindow.userName == "admin")
            {
                if(checkBoxGST.IsChecked==true)
                {
                    cc.DataGridDisplay("select distinct CustomerName,CustomerPhone from BillStock where CustomerName not like 'Cust_%' and BillType='GST'");
                }
                else
                {
                    cc.DataGridDisplay("select distinct CustomerName,CustomerPhone from BillStock where CustomerName not like 'Cust_%' and BillType='NON_GST'");
                }
            }
            else
            {
                cc.DataGridDisplay("select distinct CustomerName,CustomerPhone from BillStock where CustomerName not like 'Cust_%' and BillType='GST'");
            }
            dataGridCustomer.ItemsSource = cc.dt.AsDataView();
            dataGridCustomer.Visibility = System.Windows.Visibility.Visible;
            cc.CloseConnection();
        }

        private void dataGridCustomer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if(checkBoxGST.IsChecked==true)
                {
                    MainWindow.isgst = 1;
                }
                else
                {
                    MainWindow.isgst = 0;
                }
                custPhone = ((DataRowView)dataGridCustomer.SelectedItem).Row["CustomerPhone"].ToString();
                CustomerDetails cd = new CustomerDetails();
                cd.ShowDialog();
            }
            catch (Exception)
            {
                MessageBox.Show("Click on a customer");
            }
        }

        private void dataGridCustomer_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            CustomerTab ct = new CustomerTab();
            this.NavigationService.Navigate(ct);
        }

        private void checkBoxGST_Checked(object sender, RoutedEventArgs e)
        {
            LoadGrid();
        }

        private void checkBoxGST_Unchecked(object sender, RoutedEventArgs e)
        {
            LoadGrid();
        }
    }
}
