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
    /// Interaction logic for VendorPayment.xaml
    /// </summary>
    public partial class VendorPayment : Window
    {
        ConnectionClass cc = new ConnectionClass();
        public static string totalBillAmount, totalPayment, comboName, vName = null;
        public static int billAmount, paymentAmount, balanceAmount, textPayment, vendorId, vidExist;
        public static List<string> vendorList = new List<string>();

        public VendorPayment()
        {
            InitializeComponent();
            vName = VendorPurchases.vendorName;
            if (vName != null)
            {
                comboBoxVendorName.SelectedItem = vName;
                labelVendorName.Content = vName;
            }
            FilterViewModel vm = new FilterViewModel();
            this.DataContext = vm;
            vendorName();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
            vendorList.Clear();
        }

        private void btn_Update_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxVendorName.SelectedItem != null)
            {
                if (txtPayment.Text != "")
                {
                    textPayment = Convert.ToInt32(txtPayment.Text);
                    balanceAmount = balanceAmount - textPayment;
                    vendorIdStore();
                    updateBalance();
                    MessageBox.Show("Balance amount updated");
                    txtBalance.Text = Convert.ToString(balanceAmount);
                }
                else
                {
                    MessageBox.Show("Enter the payment");
                }
                txtPayment.Text = "";
            }
            else
            {
                MessageBox.Show("Select a vendor");
            }
        }

        private void comboBoxVendorName_GotFocus(object sender, RoutedEventArgs e)
        {
            comboBoxVendorName.IsDropDownOpen = true;
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

        private void updateBalance()
        {
            var date = DateTime.Now;
            string today = date.ToString("dd-MMM-yy");
            cc.OpenConnection();
            cc.ExecuteQuery("insert into TransactionDetails(Vid,Vname,Bno,gtotal,Payment,PurchaseDate) values(" + vendorId + ",'" + comboBoxVendorName.SelectedItem + "',0,0," + textPayment + ",'" + today + "')");
            cc.CloseConnection();
        }

        private void vendorIdStore()
        {
            cc.OpenConnection();
            cc.DataReader("select Vid from TransactionDetails where Vname='" + comboBoxVendorName.SelectedItem + "'");
            while (cc.reader.Read())
            {
                vendorId = Convert.ToInt32(cc.reader["Vid"]);
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void comboBoxVendorName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                cc.OpenConnection();
                if (cc.BillPreview("select Count(*) from TransactionDetails where Vname=@vname", "@vname", comboBoxVendorName.SelectedItem.ToString(), vidExist) > 0)
                {
                    calculateBillAmount();
                    calculateTotalPayment();
                    balanceAmount = billAmount - paymentAmount;
                    txtBalance.Text = Convert.ToString(balanceAmount);
                    txtPayment.Text = "";
                }
                else
                {
                    MessageBox.Show("No transaction with " + comboBoxVendorName.SelectedItem.ToString());
                }
            }

            catch
            {
                comboBoxVendorName.SelectedItem = null;
            }
            cc.CloseConnection();
            comboBoxVendorName.IsDropDownOpen = true;
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

        private void calculateBillAmount()
        {
            if (comboBoxVendorName.SelectedItem != null)
            {
                cc.OpenConnection();
                comboName = comboBoxVendorName.SelectedItem.ToString();
                cc.DataReader("select sum(gtotal) as BillAmount from TransactionDetails where Vname='" + comboName + "'");
                while (cc.reader.Read())
                {
                    totalBillAmount = cc.reader["BillAmount"].ToString();
                }
                cc.CloseReader();
                cc.CloseConnection();
                billAmount = Convert.ToInt32(totalBillAmount);
            }
            else
            {
                MessageBox.Show("Select any vendor");
            }

        }

        private class FilterViewModel
        {
            public IEnumerable<string> DataSource { get; set; }
            public FilterViewModel()
            {
                DataSource = vendorList;
            }
        }

        private void calculateTotalPayment()
        {
            if (comboBoxVendorName.SelectedItem != null)
            {
                cc.OpenConnection();
                comboName = comboBoxVendorName.SelectedItem.ToString();
                cc.DataReader("select sum(Payment) as PaymentAmount from TransactionDetails where Vname='" + comboName + "'");
                while (cc.reader.Read())
                {
                    totalPayment = cc.reader["PaymentAmount"].ToString();
                }
                cc.CloseReader();
                cc.CloseConnection();
                if (totalPayment == "0.0000")
                {
                    paymentAmount = 0;
                }
                else
                {
                    paymentAmount = Convert.ToInt32(totalPayment);
                }
            }
            else
            {
                MessageBox.Show("Select any vendor");
            }
        }
    }
}