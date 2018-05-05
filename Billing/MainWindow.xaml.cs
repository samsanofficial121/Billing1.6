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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static RoutedCommand sellPageCommand = new RoutedCommand();
        public static RoutedCommand purchasePageCommand = new RoutedCommand();
        public static RoutedCommand salesHistoryCommand = new RoutedCommand();
        public static RoutedCommand purchaseHistoryCommand = new RoutedCommand();
        public static RoutedCommand reportsPageCommand = new RoutedCommand();
        public static RoutedCommand salesReportCommand = new RoutedCommand();
        public static RoutedCommand stockReportCommand = new RoutedCommand();
        public static RoutedCommand customerOrderReportCommand = new RoutedCommand();
        public static RoutedCommand damageItemPageCommand = new RoutedCommand();
        public static RoutedCommand vendorPageCommand = new RoutedCommand();
        public static RoutedCommand customerPageCommand = new RoutedCommand();
        public static RoutedCommand SpPageCommand = new RoutedCommand();

        public MainWindow()
        {
            InitializeComponent();
            keyActions();
            SPpage sp = new SPpage();
            frame.NavigationService.Navigate(sp);
        }

        private void keyActions()
        {
            sellPageCommand.InputGestures.Add(new KeyGesture(Key.F1, ModifierKeys.Control));
            purchasePageCommand.InputGestures.Add(new KeyGesture(Key.F2, ModifierKeys.Control));
            salesHistoryCommand.InputGestures.Add(new KeyGesture(Key.F3, ModifierKeys.Control));
            purchaseHistoryCommand.InputGestures.Add(new KeyGesture(Key.F4, ModifierKeys.Control));
            reportsPageCommand.InputGestures.Add(new KeyGesture(Key.F5, ModifierKeys.Control));
            salesReportCommand.InputGestures.Add(new KeyGesture(Key.F6, ModifierKeys.Control));
            stockReportCommand.InputGestures.Add(new KeyGesture(Key.F7, ModifierKeys.Control));
            customerOrderReportCommand.InputGestures.Add(new KeyGesture(Key.F8, ModifierKeys.Control));
            damageItemPageCommand.InputGestures.Add(new KeyGesture(Key.F9, ModifierKeys.Control));
            vendorPageCommand.InputGestures.Add(new KeyGesture(Key.F10, ModifierKeys.Control));
            customerPageCommand.InputGestures.Add(new KeyGesture(Key.F11, ModifierKeys.Control));
            SpPageCommand.InputGestures.Add(new KeyGesture(Key.F12, ModifierKeys.Control));
        }

        private void closebtn_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void minimizebtn_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SellPage sell = new SellPage();
            frame.NavigationService.Navigate(sell);
        }

        private void CommandBinding_Executed_1(object sender, ExecutedRoutedEventArgs e)
        {
            PurchasePage pp = new PurchasePage();
            frame.NavigationService.Navigate(pp);
        }

        private void CommandBinding_Executed_2(object sender, ExecutedRoutedEventArgs e)
        {
            SalesHistory sh = new SalesHistory();
            frame.NavigationService.Navigate(sh);
        }

        private void CommandBinding_Executed_3(object sender, ExecutedRoutedEventArgs e)
        {
            PurchaseHistory ph = new PurchaseHistory();
            frame.NavigationService.Navigate(ph);
        }

        private void CommandBinding_Executed_4(object sender, ExecutedRoutedEventArgs e)
        {
            ReportsPage rp = new ReportsPage();
            frame.NavigationService.Navigate(rp);
        }

        private void CommandBinding_Executed_5(object sender, ExecutedRoutedEventArgs e)
        {
            SalesReportOptions sro = new SalesReportOptions();
            sro.ShowDialog();
        }

        private void CommandBinding_Executed_6(object sender, ExecutedRoutedEventArgs e)
        {
            StockReportPage srp = new StockReportPage();
            frame.NavigationService.Navigate(srp);
        }

        private void CommandBinding_Executed_7(object sender, ExecutedRoutedEventArgs e)
        {
            CustomerOrderViewer cov = new CustomerOrderViewer();
            frame.NavigationService.Navigate(cov);
        }

        private void CommandBinding_Executed_8(object sender, ExecutedRoutedEventArgs e)
        {
            DamageItemPage dip = new DamageItemPage();
            frame.NavigationService.Navigate(dip);
        }

        private void CommandBinding_Executed_9(object sender, ExecutedRoutedEventArgs e)
        {
            VendorPage vp = new VendorPage();
            frame.NavigationService.Navigate(vp);
        }

        private void CommandBinding_Executed_10(object sender, ExecutedRoutedEventArgs e)
        {
            CustomerTab ct = new CustomerTab();
            frame.NavigationService.Navigate(ct);
        }

        private void CommandBinding_Executed_11(object sender, ExecutedRoutedEventArgs e)
        {
            SPpage spp = new SPpage();
            frame.NavigationService.Navigate(spp);
        }
    }
}
