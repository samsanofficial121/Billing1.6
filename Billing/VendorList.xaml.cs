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
    /// Interaction logic for VendorList.xaml
    /// </summary>
    public partial class VendorList : Page
    {
        private VendorGridClass.DataAccess objDs;
        private VendorGridClass.VendorTable objVendorToAdd;
        public VendorList()
        {
            InitializeComponent();
            objDs = new VendorGridClass.DataAccess();
            dataGrid2.ItemsSource = objDs.GetVendor();
        }
        /*private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }*/
        private void dataGrid2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            objVendorToAdd = dataGrid2.SelectedItem as VendorGridClass.VendorTable;
        }

        private void dataGrid2_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                FrameworkElement element_Id = dataGrid2.Columns[0].GetCellContent(e.Row);
                if (element_Id.GetType() == typeof(TextBox))
                {
                    var id = ((TextBox)element_Id).Text;
                    objVendorToAdd.Vid = Convert.ToInt32(id);
                }
                FrameworkElement element_Name = dataGrid2.Columns[1].GetCellContent(e.Row);
                if (element_Name.GetType() == typeof(TextBox))
                {
                    var name = ((TextBox)element_Name).Text;
                    objVendorToAdd.Vname = name;
                }
                FrameworkElement element_Phone = dataGrid2.Columns[2].GetCellContent(e.Row);
                if (element_Phone.GetType() == typeof(TextBox))
                {
                    var phone = ((TextBox)element_Phone).Text;
                    objVendorToAdd.Vphone = phone;
                }
                FrameworkElement element_Place = dataGrid2.Columns[3].GetCellContent(e.Row);
                if (element_Place.GetType() == typeof(TextBox))
                {
                    var place = ((TextBox)element_Place).Text;
                    objVendorToAdd.Vplace = place;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void dataGrid2_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            objDs.InsertVendor(objVendorToAdd);
        }
    }
}
