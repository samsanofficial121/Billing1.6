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
    /// Interaction logic for SPpage.xaml
    /// </summary>
    public partial class SPpage : Page
    {
        ConnectionClass cc = new ConnectionClass();
        public SPpage()
        {
            InitializeComponent();
            VendorPage vp = new VendorPage();
            vendorframe.NavigationService.Navigate(vp);
            CustomerTab cl = new CustomerTab();
            customerFrame.NavigationService.Navigate(cl);
            StockPage sp = new StockPage();
            frameStock.NavigationService.Navigate(sp);
            ExpenseAccountPage eap = new ExpenseAccountPage();
            ExpenseFrame.NavigationService.Navigate(eap);
            ChartPage cp = new ChartPage();
            chartFrame.NavigationService.Navigate(cp);
        }

        private void btn_Sell_Click(object sender, RoutedEventArgs e)
        {
            SellPage sp = new SellPage();
            this.NavigationService.Navigate(sp);
        }

        private void btn_Purchase_Click(object sender, RoutedEventArgs e)
        {
            PurchasePage pp = new PurchasePage();
            this.NavigationService.Navigate(pp);
        }

        private void searchbtn_Click(object sender, RoutedEventArgs e)
        {
            SearchPage sp = new SearchPage();
            this.NavigationService.Navigate(sp);
        }

        private void reportbtn_Click(object sender, RoutedEventArgs e)
        {
            ReportsPage rp = new ReportsPage();
            this.NavigationService.Navigate(rp);
        }

        private void StatisticsTab_GotFocus(object sender, RoutedEventArgs e)
        {
            ChartPage cp = new ChartPage();
            chartFrame.NavigationService.Navigate(cp);
        }

        private void btn_Backup_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();

            dlg.Description = "Choose a directory to save the backup";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    string baseDirectory = dlg.SelectedPath;
                    string saveFileName = "Billing";
                    cc.OpenConnection();
                    cc.ExecuteQuery("use master backup database Billing to disk = '" + baseDirectory + "\\" + saveFileName + ".bak'");
                    cc.CloseConnection();
                    MessageBox.Show("Backup Successful");
                }
                catch
                {
                    MessageBox.Show("Can't Backuped to this Location...!");
                }
            }
        }

        private void btn_Restore_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dlg.DefaultExt = ".bak";
            dlg.Filter = "(.bak)|*.bak";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                try
                {
                    string filename = dlg.FileName;
                    cc.OpenConnection();
                    cc.ExecuteQuery("ALTER DATABASE Billing SET SINGLE_USER WITH ROLLBACK IMMEDIATE use master restore database Billing from disk = '" + filename + "'");
                    cc.CloseConnection();
                    MessageBox.Show("Restore Successful");
                }
                catch
                {
                    MessageBox.Show("Restoring Failed...!");
                }
            }
        }

        private void LogoutTab_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            LoginWindow log = new LoginWindow();
            log.Show();
            var window = Window.GetWindow(this);
            window.Close();
        }
    }
}