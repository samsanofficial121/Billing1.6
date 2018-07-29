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

namespace Billing
{
    /// <summary>
    /// Interaction logic for ReportsPage.xaml
    /// </summary>
    public partial class ReportsPage : Page
    {
        ConnectionClass cc = new ConnectionClass();
        int total;

        public ReportsPage()
        {
            InitializeComponent();
        }

        private void btn_Sales_Report_Click(object sender, RoutedEventArgs e)
        {
            SalesReportOptions sro = new SalesReportOptions();
            sro.ShowDialog();
        }

        private void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            SPpage spp = new SPpage();
            this.NavigationService.Navigate(spp);
        }

        private void btn_Stock_Report_Click(object sender, RoutedEventArgs e)
        {
            StockReportPage srp = new StockReportPage();
            this.NavigationService.Navigate(srp);
        }

        private void btn_Customer_Orders_Click(object sender, RoutedEventArgs e)
        {
            CustomerOrderViewer cov = new CustomerOrderViewer();
            this.NavigationService.Navigate(cov);
        }

        private void btn_Daily_Report_Click(object sender, RoutedEventArgs e)
        {
            DailyReportViewer drv = new DailyReportViewer();
            this.NavigationService.Navigate(drv);
        }

        private void btn_Expense_Report_Click(object sender, RoutedEventArgs e)
        {
            ExpenseReportOption ero = new ExpenseReportOption();
            ero.ShowDialog();
        }

        private void btn_Monthly_Report_Click(object sender, RoutedEventArgs e)
        {
            MonthlyReportOption mro = new MonthlyReportOption();
            mro.ShowDialog();
        }
    }
}
