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
using System.Windows.Shapes;

namespace Billing
{
    /// <summary>
    /// Interaction logic for RemoveVendor.xaml
    /// </summary>
    public partial class RemoveVendor : Window
    {
        ConnectionClass cc = new ConnectionClass();
        FilterViewModel vm = new FilterViewModel();
        public static List<string> vendorList = new List<string>();

        public RemoveVendor()
        {
            InitializeComponent();
            vendorList.Clear();
            this.DataContext = vm;
            vendorName();
        }

        private void vendorName()
        {
            cc.OpenConnection();
            cc.DataReader("select Vname from VendorDetails");
            while (cc.reader.Read())
            {
                vendorList.Add(cc.reader["Vname"].ToString());
                vendorList.Sort();
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if(comboBoxVendorName.SelectedItem != null)
            {
                cc.OpenConnection();
                cc.ExecuteQuery("Delete from VendorDetails where Vname = '" + comboBoxVendorName.SelectedItem + "' ");
                cc.CloseConnection();
                MessageBox.Show("Vendor removed");
                vendorList.Remove(comboBoxVendorName.SelectedItem.ToString());
                comboBoxVendorName.Items.Refresh();
                comboBoxVendorName.SelectedItem = null;
            }
            else
            {
                MessageBox.Show("Select any vendor");
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void comboBoxVendorName_GotFocus(object sender, RoutedEventArgs e)
        {
            comboBoxVendorName.IsDropDownOpen = true;
        }

        private class FilterViewModel
        {
            public IEnumerable<string> DataSource { get; set; }
            public FilterViewModel()
            {
                DataSource = vendorList;
            }
        }

        private void comboBoxVendorName_KeyUp(object sender, KeyEventArgs e)
        {
            CollectionView itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(comboBoxVendorName.ItemsSource);

            itemsViewOriginal.Filter = ((a) =>
            {
                if (String.IsNullOrEmpty(comboBoxVendorName.Text)) return true;
                else
                {
                    if (((string)a.ToString().ToUpper()).Contains(comboBoxVendorName.Text.ToUpper())) return true;
                    else return false;
                }
            });
            itemsViewOriginal.Refresh();
        }

        private void comboBoxVendorName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnDelete.Focus();
        }
    }
}
