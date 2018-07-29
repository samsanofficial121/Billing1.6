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
        public static string comboName, totalBillAmount, totalPayment, cName = null, totalSDiscount;
        public static int count;
        public double billAmount, balanceAmount, paymentAmount, textPayment, specialDiscount;
        public static string[] customerPhone;

        public CustomerPayment()
        {
            InitializeComponent();
            txtPayment.Focus();
            cName = CustomerDetails.customerName;
            if (cName != null)
            {
                comboBoxCustomerName.SelectedItem = cName;
                labelCustomerName.Content = cName;
            }
            FilterViewModel vm = new FilterViewModel();
            this.DataContext = vm;
            customerName();
        }

        private void customerName()
        {
            cc.OpenConnection();
            if(MainWindow.userName=="admin")
            {
                cc.DataReader("select distinct CustomerName from CustomerTransactions where CustomerName not like 'Cust_%'");
            }
            else
            {
                cc.DataReader("select distinct CustomerName from CustomerTransactions where CustomerName not like 'Cust_%' and BillType='GST'");
            }
            while (cc.reader.Read())
            {
                customerNameList.Add(cc.reader["CustomerName"].ToString());
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

        public class DialogInputEventArgs : EventArgs
        {
            public string Input { get; set; }
        }

        public event EventHandler<DialogInputEventArgs> InputChanged = delegate { };

        private void btn_Update_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxCustomerName.SelectedItem != null)
            {
                if (txtPayment.Text != "")
                {
                    if(txtDiscount.Text=="")
                    {
                        txtDiscount.Text = "0";
                    }
                    textPayment = Convert.ToDouble(txtPayment.Text);
                    balanceAmount = (balanceAmount - textPayment) - Convert.ToDouble(txtDiscount.Text);
                    updateBalance();
                    MessageBox.Show("Balance amount updated");
                    txtBalance.Text = Convert.ToString(balanceAmount);
                    InputChanged(this, new DialogInputEventArgs() { Input = this.txtBalance.Text });
                    SystemCommands.CloseWindow(this);
                    customerNameList.Clear();
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
            if(txtDiscount.Text=="")
            {
                txtDiscount.Text = "0";
            }
            var date = DateTime.Now;
            string today = date.ToString("dd-MMM-yy hh:mm:ss tt");
            cc.OpenConnection();
            cc.ExecuteQuery("insert into CustomerTransactions(BillNo,CustomerName,CustomerPhone,NetAmount,AmountPaid,Date,Credit,Debit,Discount) values(0,'" + comboName + "','" + customerPhone[1] + "',0," + textPayment + ",'"+ today + "',0,0,'"+txtDiscount.Text+"')");
            cc.CloseConnection();
        }

        private void txtPayment_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                txtDiscount.Focus();
            }
        }

        private void txtDiscount_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                btn_Update.Focus();
            }
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
                if(MainWindow.userName=="admin")
                {
                    if (cc.BillPreview("select count(*) from CustomerTransactions where CustomerPhone=@phone", "@phone", customerPhone[1], count) > 0)
                    {
                        calculateBillAmount();
                        calculateTotalPayment();
                        calculateSpecialDiscount();
                        balanceAmount = (billAmount - paymentAmount) - specialDiscount;
                        txtBalance.Text = Convert.ToString(balanceAmount);
                        txtPayment.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("No transactions with " + comboName);
                    }
                }
                else
                {
                    if (cc.BillPreview("select count(*) from CustomerTransactions where CustomerPhone=@phone and BillType='GST'", "@phone", customerPhone[1], count) > 0)
                    {
                        calculateBillAmount();
                        calculateTotalPayment();
                        calculateSpecialDiscount();
                        balanceAmount = (billAmount - paymentAmount) - specialDiscount;
                        txtBalance.Text = Convert.ToString(balanceAmount);
                        txtPayment.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("No transactions with " + comboName);
                    }
                }
            }

            catch
            {
                comboBoxCustomerName.SelectedItem = null;
            }
            cc.CloseConnection();
            comboBoxCustomerName.IsDropDownOpen = true;
            txtPayment.Focus();
        }

        private void calculateTotalPayment()
        {
            if (comboBoxCustomerName.SelectedItem != null)
            {
                cc.OpenConnection();
                comboName = comboBoxCustomerName.SelectedItem.ToString();
                if(MainWindow.userName=="admin")
                {
                    cc.DataReader("select sum(AmountPaid) as PaymentAmount from CustomerTransactions where CustomerPhone='" + customerPhone[1] + "'");
                }
                else
                {
                    cc.DataReader("select sum(AmountPaid) as PaymentAmount from CustomerTransactions where CustomerPhone='" + customerPhone[1] + "' and BillType='GST'");
                }
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
                    paymentAmount = Convert.ToDouble(totalPayment);
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
                if (MainWindow.userName == "admin")
                {
                    cc.DataReader("select sum(NetAmount) as BillAmount from CustomerTransactions where CustomerPhone='" + customerPhone[1] + "'");
                }
                else
                {
                    cc.DataReader("select sum(NetAmount) as BillAmount from CustomerTransactions where CustomerPhone='" + customerPhone[1] + "' and BillType='GST'");
                }
                while (cc.reader.Read())
                {
                    totalBillAmount = cc.reader["BillAmount"].ToString();
                }
                cc.CloseReader();
                cc.CloseConnection();
                billAmount = Convert.ToDouble(totalBillAmount);
            }
            else
            {
                MessageBox.Show("Select any customer");
            }
        }

        private void calculateSpecialDiscount()
        {
            if (comboBoxCustomerName.SelectedItem != null)
            {
                cc.OpenConnection();
                comboName = comboBoxCustomerName.SelectedItem.ToString();
                if (MainWindow.userName == "admin")
                {
                    cc.DataReader("select sum(Discount) as SpecialDisc from CustomerTransactions where CustomerPhone='" + customerPhone[1] + "'");
                }
                else
                {
                    cc.DataReader("select sum(Discount) as SpecialDisc from CustomerTransactions where CustomerPhone='" + customerPhone[1] + "' and BillType='GST'");
                }
                while (cc.reader.Read())
                {
                    totalSDiscount = cc.reader["SpecialDisc"].ToString();
                }
                cc.CloseReader();
                cc.CloseConnection();
                specialDiscount = Convert.ToDouble(totalSDiscount);
            }
            else
            {
                MessageBox.Show("Select any customer");
            }
        }

    }
}
