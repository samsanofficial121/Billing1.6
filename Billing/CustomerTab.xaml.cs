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
    /// Interaction logic for CustomerTab.xaml
    /// </summary>
    public partial class CustomerTab : Page
    {
        public CustomerTab()
        {
            InitializeComponent();
        }

        private void btn_CustomerList_Click(object sender, RoutedEventArgs e)
        {
            CustomerList cl = new CustomerList();
            this.NavigationService.Navigate(cl);
        }

        private void btn_CustomerOrder_Click(object sender, RoutedEventArgs e)
        {
            CustomerOrdering co = new CustomerOrdering();
            this.NavigationService.Navigate(co);
        }

        private void btn_CustomerPayment_Click(object sender, RoutedEventArgs e)
        {
            CustomerDetails.customerName = null;
            CustomerPayment cp = new CustomerPayment();
            cp.ShowDialog();
        }

        private void btn_CustomerReport_Click(object sender, RoutedEventArgs e)
        {
            CustomerReportViewer crv = new CustomerReportViewer();
            this.NavigationService.Navigate(crv);
        }
    }
}
