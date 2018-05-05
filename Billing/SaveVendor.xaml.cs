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
        public static List<string> vendorIdList = new List<string>();
        ConnectionClass cc = new ConnectionClass();
        
        public SaveVendor()
        {
            InitializeComponent();
            vendorIdList.Clear();
            readVendorId();
        }

        private void readVendorId()
        {
            string vendorIdPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Config\Config.txt");
            string vendorIdValue = File.ReadAllText(vendorIdPath);
            if (vendorIdValue.Contains("VendorId"))
            {
                string[] val = vendorIdValue.Substring(vendorIdValue.IndexOf("VendorId") + 10).Split(Convert.ToChar("'"));
                vendorIdList.Add(val[0].ToString());
                foreach (var number in vendorIdList)
                {
                    genVendorId = Convert.ToString(number);
                    txt_Vendor_Id.Text = genVendorId;
                    txt_Vendor_Name.Focus();
                }
            }
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
            }
        }

        private void writeVendorId()
        {
            string configFilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Config\Config.txt");
            string[] configValues = File.ReadAllLines(configFilePath);
            for (int i = 0; i < configValues.Length; i++)
            {
                if (configValues[i] == "VendorId '" + genVendorId + "'")
                {
                    configValues[i] = "VendorId '" + vendorId + "'";
                }
            }
            File.WriteAllLines(configFilePath, configValues);
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
    }
}
