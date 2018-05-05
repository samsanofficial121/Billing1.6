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
        public static List<string> expenseIdList = new List<string>();
        ConnectionClass cc = new ConnectionClass();

        public AddExpenseWindow()
        {
            InitializeComponent();
            expenseIdList.Clear();
            readVendorId();
        }

        public class DialogInputEventArgs : EventArgs
        {
            public string Input { get; set; }
        }
        //public event EventHandler<DialogInputEventArgs> InputChanged = delegate { };

        private void readVendorId()
        {
            string vendorIdPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Config\Config.txt");
            string vendorIdValue = File.ReadAllText(vendorIdPath);
            if (vendorIdValue.Contains("ExpenseId"))
            {
                string[] val = vendorIdValue.Substring(vendorIdValue.IndexOf("ExpenseId") + 11).Split(Convert.ToChar("'"));
                expenseIdList.Add(val[0].ToString());
                foreach (var number in expenseIdList)
                {
                    genExpenseId = Convert.ToString(number);
                    txt_Expense_Id.Text = genExpenseId;
                    txt_Expense_Name.Focus();
                }
            }
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
                readVendorId();
                // InputChanged(this, new DialogInputEventArgs() { Input = this.txt_Expense_Name });
                clearExpenseDetails();
                txt_Expense_Name.Focus();
            }
        }
        private void writeExpenseId()
        {
            string configFilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Config\Config.txt");
            string[] configValues = File.ReadAllLines(configFilePath);
            for (int i = 0; i < configValues.Length; i++)
            {
                if (configValues[i] == "ExpenseId '" + genExpenseId + "'")
                {
                    configValues[i] = "ExpenseId '" + expenseId + "'";
                }
            }
            File.WriteAllLines(configFilePath, configValues);
        }
    }
}
