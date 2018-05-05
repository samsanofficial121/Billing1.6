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
    /// Interaction logic for ListTheVendor.xaml
    /// </summary>
    public partial class ListTheVendor : Page
    {
        ConnectionClass cc = new ConnectionClass();
        public static string vendorId, totalCredit;
        public static int vidExist;
        public ListTheVendor()
        {
            InitializeComponent();
            calculateCredit();
            cc.OpenConnection();
            cc.DataGridDisplay("select distinct Vid,Vname from VendorDetails");
            dataGridVendor.ItemsSource = cc.dt.AsDataView();
            dataGridVendor.Visibility = System.Windows.Visibility.Visible;
            cc.CloseConnection();
        }

        private void calculateCredit()
        {
            cc.OpenConnection();
            cc.DataReader("select sum(CreditAmount) as CreditSum from TransactionDetails");
            while (cc.reader.Read())
            {
                totalCredit = cc.reader["CreditSum"].ToString();
            }
            cc.CloseReader();
            cc.CloseConnection();
            txt_Balance.Text = totalCredit;
        }

        private void dataGridVendor_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            cc.OpenConnection();
            try
            {
                vendorId = ((DataRowView)dataGridVendor.SelectedItem).Row["Vid"].ToString();
                if (cc.BillPreview("select Count(*) from TransactionDetails where Vid=@vid", "@vid", vendorId, vidExist) > 0)
                {
                    VendorPurchases vp = new VendorPurchases();
                    vp.ShowDialog();
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

        private void dataGridVendor_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            VendorPage vp = new VendorPage();
            this.NavigationService.Navigate(vp);
        }
    }
}
