using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for ExpenseListWindow.xaml
    /// </summary>
    public partial class ExpenseListWindow : Window
    {
        ConnectionClass cc = new ConnectionClass();
        public static string expenseID, expenseName;
        public static int cashPaid;

        public ExpenseListWindow()
        {
            InitializeComponent();
            expenseID = ExpenseListPage.expenseId;
            ShowGrid();
        }

        private void ShowGrid()
        {
            LoadExpense();
            cc.OpenConnection();
            cc.DataGridDisplay("select Epayment,EDate from ExpenseTransactionDetails where Eid=" + expenseID + "");
            dataGridExpenseList.ItemsSource = cc.dt.AsDataView();
            dataGridExpenseList.Visibility = System.Windows.Visibility.Visible;
            cc.CloseConnection();
        }

        private void LoadExpense()
        {
            cc.OpenConnection();
            cc.DataReader("select Eid,Ename from ExpenseTransactionDetails where Eid=" + expenseID + "");
            while (cc.reader.Read())
            {
                textBlockEId.Text = cc.reader["Eid"].ToString();
                textBlockEName.Text = cc.reader["Ename"].ToString();

            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void dataGridExpenseList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
