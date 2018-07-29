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
    /// Interaction logic for ExpenseFrame.xaml
    /// </summary>
    public partial class ExpenseFrame : Window
    {
        public DateTime? dateFrom, dateTo;

        private void closebtn_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        public ExpenseFrame(DateTime? dfrm, DateTime? dto)
        {
            InitializeComponent();
            dateFrom = dfrm;
            dateTo = dto;
            ExpenseReportViewer erv = new ExpenseReportViewer(dateFrom, dateTo);
            frame.NavigationService.Navigate(erv);
        }
    }
}
