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
    /// Interaction logic for SalesReporterViewerFrame.xaml
    /// </summary>
    public partial class SalesReporterViewerFrame : Window
    {
        public string report;
        public DateTime? dateFrom, dateTo;

        public SalesReporterViewerFrame(string rpt,DateTime? dfrm,DateTime? dto)
        {
            InitializeComponent();
            report = rpt;
            dateFrom = dfrm;
            dateTo = dto;
            SalesReporterViewer srv = new SalesReporterViewer(report,dateFrom,dateTo);
            frame.NavigationService.Navigate(srv);
        }

        private void closebtn_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }
    }
}
