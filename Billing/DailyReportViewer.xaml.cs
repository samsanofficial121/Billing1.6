using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Data;
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
    /// Interaction logic for DailyReportViewer.xaml
    /// </summary>
    public partial class DailyReportViewer : Page
    {
        ConnectionClass cc = new ConnectionClass();
        public static List<double> totalSalesList = new List<double>();
        public static List<double> totalCreditList = new List<double>();
        public static List<string> expenseNameList = new List<string>();
        public static List<double> expenseAmountList = new List<double>();
        public static string stDate;
        public static double cashBalance;

        public DailyReportViewer()
        {
            InitializeComponent();
            clearList();
            generateReport();
        }

        private void generateReport()
        {
            var date = DateTime.Now;
            stDate = date.ToString("dd-MMM-yy");
            DailyReportData reportDetails = new DailyReportData();
            DataTable dataTable = reportDetails._DailyReportData;
            DailyReport report = new DailyReport();
            storeSales();
            storeExpense();
            DataRow drow = dataTable.NewRow();
            drow["StDate"] = stDate;
            drow["TotalSales"] = totalSalesList.Sum();
            drow["TotalCredit"] = totalCreditList.Sum();
            dataTable.Rows.Add(drow);
            for (int i = 0; i < expenseNameList.Count; i++)
            {
                DataRow drrow = dataTable.NewRow();
                drrow["ExpenseName"] = expenseNameList[i];
                drrow["ExpenseAmount"] = expenseAmountList[i];
                dataTable.Rows.Add(drrow);
            }
            cashBalance = totalSalesList.Sum() - totalCreditList.Sum() - expenseAmountList.Sum();
            DataRow dr = dataTable.NewRow();
            dr["CashBalance"] = cashBalance;
            dataTable.Rows.Add(dr);
            report.Database.Tables["DailyReportData"].SetDataSource((DataTable)dataTable);
            dailyReportViewer.ViewerCore.ReportSource = report;
            report.Refresh();
        }

        private void storeExpense()
        {
            cc.OpenConnection();
            cc.DataReader("select distinct Ename,Epayment from ExpenseTransactionDetails where EDate = #" + stDate + "# Group By Ename,Epayment");
            while (cc.reader.Read())
            {
                expenseAmountList.Add(Convert.ToDouble(cc.reader["Epayment"]));
                expenseNameList.Add(Convert.ToString(cc.reader["Ename"]));
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void storeSales()
        {
            cc.OpenConnection();
            cc.DataReader("select distinct NetAmount,Credit from BillStock where SaleDate = #" + stDate + "# Group By NetAmount,Credit");
            while (cc.reader.Read())
            {
                totalSalesList.Add(Convert.ToDouble(cc.reader["NetAmount"]));
                totalCreditList.Add(Convert.ToDouble(cc.reader["Credit"]));
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void clearList()
        {
            totalSalesList.Clear();
            totalCreditList.Clear();
            expenseNameList.Clear();
            expenseAmountList.Clear();
        }

        private void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            clearList();
            ReportsPage rp = new ReportsPage();
            this.NavigationService.Navigate(rp);
        }
    }
}
