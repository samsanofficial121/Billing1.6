using System;
using System.Collections.Generic;
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
using System.Data;
using System.Data.OleDb;
using System.Configuration;

namespace Billing
{
    /// <summary>
    /// Interaction logic for HistoryPage.xaml
    /// </summary>
    public partial class HistoryPage : Page
    {
        ConnectionClass cc = new ConnectionClass();

        public HistoryPage()
        {
            InitializeComponent();
            cc.OpenConnection();
            cc.DataGridDisplay("select * from Stock");
            histDataGrid.ItemsSource = cc.dt.AsDataView();
            histDataGrid.Visibility = System.Windows.Visibility.Visible;
        }

        private void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            SellPage sp = new SellPage();
            this.NavigationService.Navigate(sp);
        }

        private void histDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
