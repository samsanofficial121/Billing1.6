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
    /// Interaction logic for ExpensePaymentWindow.xaml
    /// </summary>
    public partial class ExpensePaymentWindow : Window
    {

        ConnectionClass cc = new ConnectionClass();
        public static string totalBillAmount, totalPayment, comboName, EName = null;
        public static int billAmount, paymentAmount, balanceAmount, textPayment, ExpenseId, vidExist;
        public static List<string> ExpenseList = new List<string>();

        public ExpensePaymentWindow()
        {
            InitializeComponent();
            FilterViewModel vm = new FilterViewModel();
            this.DataContext = vm;
            ExpenseName();
        }

        private void comboBoxExpenseName_GotFocus(object sender, RoutedEventArgs e)
        {
            comboBoxExpenseName.IsDropDownOpen = true;
        }

        private void comboBoxExpenseName_KeyUp(object sender, KeyEventArgs e)
        {
            CollectionView itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(comboBoxExpenseName.ItemsSource);

            itemsViewOriginal.Filter = ((a) =>
            {
                if (String.IsNullOrEmpty(comboBoxExpenseName.Text)) return true;
                else
                {
                    if (((string)a.ToString().ToUpper()).Contains(comboBoxExpenseName.Text.ToUpper())) return true;
                    else return false;
                }
            });

            itemsViewOriginal.Refresh();
        }

        private void btn_Update_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxExpenseName.SelectedItem != null)
            {
                if (txtPayment.Text != "")
                {
                    var date = DateTime.Now;
                    string expenseDate = date.ToString("dd-MMM-yy hh:mm:ss tt");
                    cc.OpenConnection();
                    cc.ExecuteQuery("insert into ExpenseTransactionDetails(Eid,Ename,Epayment,EDate) values (" + Convert.ToInt32(labelExpenseId.Content) + ",'" + comboBoxExpenseName.SelectedItem.ToString() + "','" + Convert.ToDouble(txtPayment.Text) + "','" + expenseDate + "')");
                    MessageBox.Show("Expense Added");
                }
                else
                {
                    MessageBox.Show("Enter the payment");
                }
                txtPayment.Text = "";
            }
            else
            {
                MessageBox.Show("Select an Expense");
            }
        }

        private void ExpenseIdStore()
        {
            cc.OpenConnection();
            cc.DataReader("select Eid from ExpenseDetails where Ename='" + comboBoxExpenseName.SelectedItem + "'");
            while (cc.reader.Read())
            {
                labelExpenseId.Content = Convert.ToString(cc.reader["Eid"]);
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void comboBoxExpenseName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ExpenseIdStore();
            txtPayment.Focus();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
            ExpenseList.Clear();
        }

        private class FilterViewModel
        {
            public IEnumerable<string> DataSource { get; set; }
            public FilterViewModel()
            {
                DataSource = ExpenseList;
            }
        }
        private void ExpenseName()
        {
            cc.OpenConnection();
            cc.DataReader("select Ename from ExpenseDetails");
            while (cc.reader.Read())
            {
                ExpenseList.Add(cc.reader["Ename"].ToString());
                ExpenseList.Sort();
            }
            cc.CloseReader();
            cc.CloseConnection();
        }
    }
}
