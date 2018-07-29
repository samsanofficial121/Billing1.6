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
    /// Interaction logic for MonthlyReportOption.xaml
    /// </summary>
    public partial class MonthlyReportOption : Window
    {
        public DateTime? fromDate = null, toDate = null;

        public MonthlyReportOption()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void startDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            fromDate = startDate.SelectedDate.Value.Date;
        }

        private void endDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            toDate = endDate.SelectedDate.Value.Date.AddDays(1).AddSeconds(-1);
        }

        private void btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            if (fromDate != null || toDate != null)
            {
                MonthlyFrame mf = new MonthlyFrame(fromDate, toDate);
                mf.Show();
                SystemCommands.CloseWindow(this);
            }
            else
            {
                MessageBox.Show("Select start and end dates");
            }
        }
    }
}
