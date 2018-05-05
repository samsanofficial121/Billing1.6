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
    /// Interaction logic for ExpenseListPage.xaml
    /// </summary>
    public partial class ExpenseListPage : Page
    {
        ConnectionClass cc = new ConnectionClass();
        public static string expenseId, totalCredit;
        public static int vidExist;
        public ExpenseListPage()
        {
            InitializeComponent();

            cc.OpenConnection();
            cc.DataGridDisplay("select distinct Eid,Ename from ExpenseDetails");
            dataGridVendor.ItemsSource = cc.dt.AsDataView();
            dataGridVendor.Visibility = System.Windows.Visibility.Visible;
            cc.CloseConnection();
        }

        private void dataGridVendor_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void dataGridVendor_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            cc.OpenConnection();
            try
            {
                expenseId = ((DataRowView)dataGridVendor.SelectedItem).Row["Eid"].ToString();
                if (cc.BillPreview("select Count(*) from ExpenseDetails where Eid=@Eid", "@Eid", expenseId, vidExist) > 0)
                {
                    ExpenseListWindow elp = new ExpenseListWindow();
                    elp.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Transaction not found");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Click on the Vendor");
            }
            cc.CloseConnection();
        }



        private void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            ExpenseAccountPage eap = new ExpenseAccountPage();
            this.NavigationService.Navigate(eap);
        }
    }
}
