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
    /// Interaction logic for SalesReportOptions.xaml
    /// </summary>
    public partial class SalesReportOptions : Window
    {
        public DateTime? startDate = null, endDate = null;
        public string reportType;

        public SalesReportOptions()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void dateFrom_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            startDate = dateFrom.SelectedDate.Value.Date;
        }

        private void dateTo_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            endDate = dateTo.SelectedDate.Value.Date;
        }

        private void dateFrom1_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            startDate = dateFrom1.SelectedDate.Value.Date;
        }

        private void dateTo1_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            endDate = dateTo1.SelectedDate.Value.Date;
        }

        private void dateFrom2_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            startDate = dateFrom2.SelectedDate.Value.Date;
        }

        private void dateTo2_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            endDate = dateTo2.SelectedDate.Value.Date;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (startDate != null && endDate != null)
            {
                if (rbTotalReport.IsChecked == true)
                {
                    reportType = "Total Report";
                    SalesReporterViewerFrame srvf = new SalesReporterViewerFrame(reportType, startDate, endDate);
                    srvf.Show();
                    SystemCommands.CloseWindow(this);
                }
                else if (rbDailyReport.IsChecked == true)
                {
                    reportType = "Daily Report";
                    SalesReporterViewerFrame srvf = new SalesReporterViewerFrame(reportType, startDate, endDate);
                    srvf.Show();
                    SystemCommands.CloseWindow(this);
                }
                else if (rbMonthlyReport.IsChecked == true)
                {
                    reportType = "Monthly Report";
                    SalesReporterViewerFrame srvf = new SalesReporterViewerFrame(reportType, startDate, endDate);
                    srvf.Show();
                    SystemCommands.CloseWindow(this);
                }
                else
                {
                    MessageBox.Show("Select any options above");
                }
            }
            else
            {
                MessageBox.Show("Select start and end dates");
            }
        }

        private void rbTotalReport_Click(object sender, RoutedEventArgs e)
        {
            rbTotalReport.IsChecked = true;
            dateFrom.Visibility = Visibility.Visible;
            dateTo.Visibility = Visibility.Visible;
            dateFrom1.Visibility = Visibility.Hidden;
            dateTo1.Visibility = Visibility.Hidden;
            dateFrom2.Visibility = Visibility.Hidden;
            dateTo2.Visibility = Visibility.Hidden;
        }

        private void rbDailyReport_Click(object sender, RoutedEventArgs e)
        {
            rbDailyReport.IsChecked = true;
            dateFrom.Visibility = Visibility.Hidden;
            dateTo.Visibility = Visibility.Hidden;
            dateFrom1.Visibility = Visibility.Visible;
            dateTo1.Visibility = Visibility.Visible;
            dateFrom2.Visibility = Visibility.Hidden;
            dateTo2.Visibility = Visibility.Hidden;
        }

        private void rbMonthlyReport_Click(object sender, RoutedEventArgs e)
        {
            rbMonthlyReport.IsChecked = true;
            dateFrom.Visibility = Visibility.Hidden;
            dateTo.Visibility = Visibility.Hidden;
            dateFrom1.Visibility = Visibility.Hidden;
            dateTo1.Visibility = Visibility.Hidden;
            dateFrom2.Visibility = Visibility.Visible;
            dateTo2.Visibility = Visibility.Visible;
        }
    }
}
