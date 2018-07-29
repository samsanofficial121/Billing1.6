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
using System.IO;
using System.Reflection;

namespace Billing
{
    /// <summary>
    /// Interaction logic for SaveVendor.xaml
    /// </summary>
    public partial class SaveVendor : Window
    {
        public static string genVendorId;
        public static int vendorId;
        ConnectionClass cc = new ConnectionClass();
        
        public SaveVendor()
        {
            InitializeComponent();
            readVendorId();
        }

        private void readVendorId()
        {
            cc.OpenConnection();
            cc.DataReader("select ConfigValue from ConfigTable where ConfigId = 6 ");
            while (cc.reader.Read())
            {
                genVendorId = cc.reader["ConfigValue"].ToString();
            }
            cc.CloseReader();
            cc.CloseConnection();
            txt_Vendor_Id.Text = genVendorId;
            txt_Vendor_Name.Focus();
        }

        private void writeVendorId()
        {
            cc.OpenConnection();
            cc.ExecuteQuery("update ConfigTable set ConfigValue=" + vendorId + " where ConfigId = 6 ");
            cc.CloseConnection();
        }

        public class DialogInputEventArgs : EventArgs
        {
            public string Input { get; set; }
        }

        public event EventHandler<DialogInputEventArgs> InputChanged = delegate { };

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            if (txt_Vendor_Name.Text == "" || txt_Vendor_Place.Text == "" || txt_Vendor_Phone.Text == "")
            {
                MessageBox.Show("Enter Vendor Details..!");
            }
            else
            {
                cc.OpenConnection();
                vendorId = Convert.ToInt32(genVendorId) + 1;
                cc.ExecuteQuery("insert into VendorDetails(Vid,Vname,Vphone,Vplace) values( ' " + genVendorId + " ' , '" + txt_Vendor_Name.Text + "' , ' " + txt_Vendor_Phone.Text + " ' , ' " + txt_Vendor_Place.Text + " ' )");
                cc.CloseConnection();
                MessageBox.Show("Vendor Details Added");
                writeVendorId();
                readVendorId();
                InputChanged(this, new DialogInputEventArgs() { Input = this.txt_Vendor_Name.Text });
                clearVendorDetails();
                txt_Vendor_Name.Focus();
                SystemCommands.CloseWindow(this);
            }
        }

        private void clearVendorDetails()
        {
            txt_Vendor_Name.Text = "";
            txt_Vendor_Phone.Text = "";
            txt_Vendor_Place.Text = "";
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void txt_Vendor_Name_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                txt_Vendor_Place.Focus();
            }
        }

        private void txt_Vendor_Place_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                txt_Vendor_Phone.Focus();
            }
        }

        private void txt_Vendor_Phone_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                btn_Save.Focus();
            }
        }
    }
}
