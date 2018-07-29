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
    /// Interaction logic for MonthlyFrame.xaml
    /// </summary>
    public partial class MonthlyFrame : Window
    {
        public DateTime? dateFrom, dateTo;

        public MonthlyFrame(DateTime? dfrm, DateTime? dto)
        {
            InitializeComponent();
            dateFrom = dfrm;
            dateTo = dto;
            TotalMonthlyReportViewer tmrv = new TotalMonthlyReportViewer(dateFrom, dateTo);
            frame.NavigationService.Navigate(tmrv);
        }

        private void closebtn_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }
    }
}
