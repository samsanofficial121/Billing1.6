using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for AddExpenseWindow.xaml
    /// </summary>
    public partial class AddExpenseWindow : Window
    {

        public static string genExpenseId;
        public static int expenseId;
        ConnectionClass cc = new ConnectionClass();

        public AddExpenseWindow()
        {
            InitializeComponent();
            readExpenseId();
        }

        public class DialogInputEventArgs : EventArgs
        {
            public string Input { get; set; }
        }
        //public event EventHandler<DialogInputEventArgs> InputChanged = delegate { };

        private void readExpenseId()
        {
            cc.OpenConnection();
            cc.DataReader("select ConfigValue from ConfigTable where ConfigId = 7 ");
            while (cc.reader.Read())
            {
                genExpenseId = cc.reader["ConfigValue"].ToString();
            }
            cc.CloseReader();
            cc.CloseConnection();
            txt_Expense_Id.Text = genExpenseId;
            txt_Expense_Name.Focus();
        }

        private void writeExpenseId()
        {
            cc.OpenConnection();
            cc.ExecuteQuery("update ConfigTable set ConfigValue=" + expenseId + " where ConfigId = 7 ");
            cc.CloseConnection();
        }

        private void clearExpenseDetails()
        {
            txt_Expense_Name.Text = "";
        }

        private void btn_Expense_CloseButton_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void btn_Expense_Save_Click(object sender, RoutedEventArgs e)
        {
            if (txt_Expense_Name.Text == "")
            {
                MessageBox.Show("Enter Expense Details..!");
            }
            else
            {
                cc.OpenConnection();
                expenseId = Convert.ToInt32(genExpenseId) + 1;
                cc.ExecuteQuery("insert into ExpenseDetails(Eid,Ename) values( ' " + genExpenseId + " ' , '" + txt_Expense_Name.Text + "' )");
                cc.CloseConnection();
                MessageBox.Show("Expense Details Added");
                writeExpenseId();
                readExpenseId();
                clearExpenseDetails();
                SystemCommands.CloseWindow(this);
            }
        }

        private void txt_Expense_Name_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                btn_Expense_Save.Focus();
            }
        }
    }
}
