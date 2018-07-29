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
    /// Interaction logic for ExpenseReportOption.xaml
    /// </summary>
    public partial class ExpenseReportOption : Window
    {
        public DateTime? fromDate = null, toDate = null;

        public ExpenseReportOption()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            if(fromDate != null || toDate != null)
            {
                ExpenseFrame ef = new ExpenseFrame(fromDate, toDate);
                ef.Show();
                SystemCommands.CloseWindow(this);
            }
            else
            {
                MessageBox.Show("Select start and end dates");
            }
        }

        private void startDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            fromDate = startDate.SelectedDate.Value.Date;
        }

        private void endDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            toDate = endDate.SelectedDate.Value.Date.AddDays(1).AddSeconds(-1);
        }
    }
}
