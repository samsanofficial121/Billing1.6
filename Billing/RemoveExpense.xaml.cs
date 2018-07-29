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
    /// Interaction logic for RemoveExpense.xaml
    /// </summary>
    public partial class RemoveExpense : Window
    {
        ConnectionClass cc = new ConnectionClass();
        public static List<string> expenseList = new List<string>();

        public RemoveExpense()
        {
            InitializeComponent();
            expenseList.Clear();
            FilterViewModel vm = new FilterViewModel();
            this.DataContext = vm;
            ExpenseName();
        }

        private void ExpenseName()
        {
            cc.OpenConnection();
            cc.DataReader("select Ename from ExpenseDetails");
            while (cc.reader.Read())
            {
                expenseList.Add(cc.reader["Ename"].ToString());
                expenseList.Sort();
            }
            cc.CloseReader();
            cc.CloseConnection();
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxExpenseName.SelectedItem != null)
            {
                cc.OpenConnection();
                cc.ExecuteQuery("Delete from ExpenseDetails where Ename = '" + comboBoxExpenseName.SelectedItem + "' ");
                cc.CloseConnection();
                MessageBox.Show("Expense removed");
                expenseList.Remove(comboBoxExpenseName.SelectedItem.ToString());
                comboBoxExpenseName.Items.Refresh();
                comboBoxExpenseName.SelectedItem = null;
            }
            else
            {
                MessageBox.Show("Select any type of Expense");
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {

            SystemCommands.CloseWindow(this);

        }

        private class FilterViewModel
        {
            public IEnumerable<string> DSource { get; set; }
            public FilterViewModel()
            {
                DSource = expenseList;
            }
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

        private void comboBoxExpenseName_GotFocus(object sender, RoutedEventArgs e)
        {
            comboBoxExpenseName.IsDropDownOpen = true;
        }
    }
}
