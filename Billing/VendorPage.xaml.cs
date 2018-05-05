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
    /// Interaction logic for VendorPage.xaml
    /// </summary>
    public partial class VendorPage : Page
    {
        public VendorPage()
        {
            InitializeComponent();
        }

        private void btnListAllVendor_Click(object sender, RoutedEventArgs e)
        {
            ListTheVendor ltv = new ListTheVendor();
            this.NavigationService.Navigate(ltv);
        }

        private void btnAddVendor_Click(object sender, RoutedEventArgs e)
        {
            SaveVendor sv = new SaveVendor();
            sv.ShowDialog();
        }

        private void btnRemoveVendor_Click(object sender, RoutedEventArgs e)
        {
            RemoveVendor rv = new RemoveVendor();
            rv.ShowDialog();
        }

        private void btnVendorPayment_Click(object sender, RoutedEventArgs e)
        {
            VendorPurchases.vendorName = null;
            VendorPayment vp = new VendorPayment();
            vp.ShowDialog();
        }
    }
}
