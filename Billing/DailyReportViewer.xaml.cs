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
using System.IO;
using System.Reflection;

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
        public static string endDate, companyName, companyDetails,startDate;
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
            endDate = date.ToString("dd-MMM-yy hh:mm:ss tt");
            //endDate = "28-May-18 23:59:59 PM";
            var dat = DateTime.Today;
            startDate = dat.ToString("dd-MMM-yy hh:mm:ss tt");
            DailyReportData reportDetails = new DailyReportData();
            DailyReport report = new DailyReport();
            DataTable dtTable = reportDetails.CompanyDetails;
            readCompanyDetails();
            DataRow drr = dtTable.NewRow();
            drr["CompanyName"] = companyName;
            drr["CompanyDetails"] = companyDetails;
            dtTable.Rows.Add(drr);
            report.Database.Tables["CompanyDetails"].SetDataSource((DataTable)dtTable);
            storeSales();
            storeExpense();
            DataTable dataTable = reportDetails._DailyReportData;
            DataRow drow = dataTable.NewRow();
            drow["StDate"] = endDate;
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
            dr["TotalExpense"] = expenseAmountList.Sum();
            dr["CashBalance"] = cashBalance;
            dataTable.Rows.Add(dr);
            report.Database.Tables["DailyReportData"].SetDataSource((DataTable)dataTable);
            dailyReportViewer.ViewerCore.ReportSource = report;
            report.Refresh();
        }

        private void readCompanyDetails()
        {
            cc.OpenConnection();
            cc.DataReader("select ConfigValue from ConfigTable where ConfigId = 1");
            while (cc.reader.Read())
            {
                companyName = cc.reader["ConfigValue"].ToString();
            }
            cc.CloseReader();
            cc.DataReader("select ConfigValue from ConfigTable where ConfigId = 2");
            while (cc.reader.Read())
            {
                companyDetails = cc.reader["ConfigValue"].ToString();
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void storeExpense()
        {
            cc.OpenConnection();
            cc.DataReader("select Ename,Epayment from ExpenseTransactionDetails where Edate between '" + startDate + "' and '" + endDate + "'");
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
            if(MainWindow.userName=="admin")
            {
                cc.DataReader("select distinct BillNo,NetAmount,Credit from BillStock where SaleDate between '" + startDate + "' and '" + endDate + "' Group By BillNo,NetAmount,Credit");
            }
            else
            {
                cc.DataReader("select distinct BillNo,NetAmount,Credit from BillStock where SaleDate between '" + startDate + "' and '" + endDate + "' and BillType='GST' Group By BillNo,NetAmount,Credit");
            }
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
