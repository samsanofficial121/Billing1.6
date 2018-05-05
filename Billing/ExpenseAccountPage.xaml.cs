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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Billing
{
    /// <summary>
    /// Interaction logic for ExpenseAccountPage.xaml
    /// </summary>
    public partial class ExpenseAccountPage : Page
    {
        public ExpenseAccountPage()
        {
            InitializeComponent();
        }

        private void btn_AddExpense_Click(object sender, RoutedEventArgs e)
        {
            AddExpenseWindow aew = new AddExpenseWindow();
            aew.ShowDialog();
        }

        private void btn_ExpensePayment_Click(object sender, RoutedEventArgs e)
        {
            ExpensePaymentWindow epw = new ExpensePaymentWindow();
            epw.ShowDialog();
        }

        private void btn_ExpenseList_Click(object sender, RoutedEventArgs e)
        {
            ExpenseListPage elp = new ExpenseListPage();
            this.NavigationService.Navigate(elp);
        }

        private void btn_ExpenseRemove_Click(object sender, RoutedEventArgs e)
        {
            RemoveExpense re = new RemoveExpense();
            re.ShowDialog();
        }
    }
}
