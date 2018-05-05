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
    /// Interaction logic for SPpage.xaml
    /// </summary>
    public partial class SPpage : Page
    {
        ConnectionClass cc = new ConnectionClass();
        public SPpage()
        {
            InitializeComponent();
            VendorPage vp = new VendorPage();
            vendorframe.NavigationService.Navigate(vp);
            CustomerTab cl = new CustomerTab();
            customerFrame.NavigationService.Navigate(cl);
            StockPage sp = new StockPage();
            frameStock.NavigationService.Navigate(sp);
            ExpenseAccountPage eap = new ExpenseAccountPage();
            ExpenseFrame.NavigationService.Navigate(eap);
        }


        private void btn_Sell_Click(object sender, RoutedEventArgs e)
        {
            SellPage sp = new SellPage();
            this.NavigationService.Navigate(sp);
        }

        private void btn_Purchase_Click(object sender, RoutedEventArgs e)
        {
            PurchasePage pp = new PurchasePage();
            this.NavigationService.Navigate(pp);
        }

        private void btnListAllVendor_Click(object sender, RoutedEventArgs e)
        {
            VendorList vl = new VendorList();
            vendorframe.NavigationService.Navigate(vl);
        }

        private void searchbtn_Click(object sender, RoutedEventArgs e)
        {
            SearchPage sp = new SearchPage();
            this.NavigationService.Navigate(sp);
        }

        private void reportbtn_Click(object sender, RoutedEventArgs e)
        {
            ReportsPage rp = new ReportsPage();
            this.NavigationService.Navigate(rp);
        }


    }
}