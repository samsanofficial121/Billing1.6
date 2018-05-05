using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace Billing
{
    /// <summary>
    /// Interaction logic for CustomerPayment.xaml
    /// </summary>
    public partial class CustomerPayment : Window
    {
        ConnectionClass cc = new ConnectionClass();
        public static List<string> customerNameList = new List<string>();
        public static string comboName, totalBillAmount, totalPayment;
        public static int count, billAmount, balanceAmount, paymentAmount, textPayment;
        public static string[] customerPhone;

        public CustomerPayment()
        {
            InitializeComponent();
            FilterViewModel vm = new FilterViewModel();
            this.DataContext = vm;
            customerName();
        }

        private void customerName()
        {
            cc.OpenConnection();
            cc.DataReader("select distinct CustomerName,CustomerPhone from CustomerTransactions where CustomerName is not null");
            while (cc.reader.Read())
            {
                customerNameList.Add(cc.reader["CustomerName"].ToString()+"  "+cc.reader["CustomerPhone"]);
                customerNameList.Sort();
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
            customerNameList.Clear();
        }

        private void btn_Update_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxCustomerName.SelectedItem != null)
            {
                if (txtPayment.Text != "")
                {
                    textPayment = Convert.ToInt32(txtPayment.Text);
                    balanceAmount = balanceAmount - textPayment;
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
                MessageBox.Show("Select a customer");
            }
        }

        private void updateBalance()
        {
            cc.OpenConnection();
            cc.ExecuteQuery("insert into CustomerTransactions(BillNo,CustomerName,CustomerPhone,NetAmount,AmountPaid) values(0,'" + customerPhone[0] + "','" + customerPhone[1] + "',0," + textPayment + ")");
            cc.CloseConnection();
        }

        private class FilterViewModel
        {
            public IEnumerable<string> DataSource { get; set; }
            public FilterViewModel()
            {
                DataSource = customerNameList;
            }
        }

        private void comboBoxCustomerName_GotFocus(object sender, RoutedEventArgs e)
        {
            comboBoxCustomerName.IsDropDownOpen = true;
        }

        private void comboBoxCustomerName_KeyUp(object sender, KeyEventArgs e)
        {
            CollectionView itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(comboBoxCustomerName.ItemsSource);

            itemsViewOriginal.Filter = ((a) =>
            {
                if (String.IsNullOrEmpty(comboBoxCustomerName.Text)) return true;
                else
                {
                    if (((string)a.ToString().ToUpper()).Contains(comboBoxCustomerName.Text.ToUpper())) return true;
                    else return false;
                }
            });

            itemsViewOriginal.Refresh();
        }

        private void comboBoxCustomerName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                comboName = comboBoxCustomerName.SelectedItem.ToString();
                customerPhone = Regex.Split(comboName, "  ");
                cc.OpenConnection();
                if (cc.BillPreview("select count(*) from CustomerTransactions where CustomerPhone=@phone", "@phone", customerPhone[1], count) > 0)
                {
                    calculateBillAmount();
                    calculateTotalPayment();
                    balanceAmount = billAmount - paymentAmount;
                    txtBalance.Text = Convert.ToString(balanceAmount);
                    txtPayment.Text = "";
                }
                else
                {
                    MessageBox.Show("No transactions with " + comboName);
                }
            }

            catch
            {
                comboBoxCustomerName.SelectedItem = null;
            }
            cc.CloseConnection();
            comboBoxCustomerName.IsDropDownOpen = true;
        }

        private void calculateTotalPayment()
        {
            if (comboBoxCustomerName.SelectedItem != null)
            {
                cc.OpenConnection();
                comboName = comboBoxCustomerName.SelectedItem.ToString();
                cc.DataReader("select sum(AmountPaid) as PaymentAmount from CustomerTransactions where CustomerPhone='" + customerPhone[1] + "'");
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
                MessageBox.Show("Select any customer");
            }
        }

        private void calculateBillAmount()
        {
            if (comboBoxCustomerName.SelectedItem != null)
            {
                cc.OpenConnection();
                comboName = comboBoxCustomerName.SelectedItem.ToString();
                cc.DataReader("select sum(NetAmount) as BillAmount from CustomerTransactions where CustomerPhone='" + customerPhone[1] + "'");
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
                MessageBox.Show("Select any customer");
            }
        }
    }
}
