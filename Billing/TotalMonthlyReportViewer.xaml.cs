using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for TotalMonthlyReportViewer.xaml
    /// </summary>
    public partial class TotalMonthlyReportViewer : Page
    {
        ConnectionClass cc = new ConnectionClass();
        public static List<double> totalSalesList = new List<double>();
        public static List<double> totalCreditList = new List<double>();
        public static List<string> expenseNameList = new List<string>();
        public static List<double> expenseAmountList = new List<double>();
        public static List<string> expenseDateList = new List<string>();
        public static string companyName, companyDetails;
        public static DateTime? startDate = null, endDate = null;
        public static double cashBalance;

        public TotalMonthlyReportViewer(DateTime? sdt, DateTime? edt)
        {
            InitializeComponent();
            startDate = sdt;
            endDate = edt;
            clearList();
            generateReport();
        }

        private void generateReport()
        {
            TotalMonthlyData reportDetails = new TotalMonthlyData();
            TotalMonthlyReport report = new TotalMonthlyReport();
            DataTable dataTable = reportDetails.TotalMonthlyTable;
            readCompanyDetails();
            storeSales();
            storeExpense();
            DataRow drow = dataTable.NewRow();
            drow["CompanyName"] = companyName;
            drow["CompanyDetails"] = companyDetails;
            string[] sDate = Regex.Split(startDate.ToString(), " ");
            drow["StartDate"] = sDate[0];
            string[] eDate = Regex.Split(endDate.ToString(), " ");
            drow["EndDate"] = eDate[0];
            drow["TotalSales"] = totalSalesList.Sum();
            drow["TotalCredit"] = totalCreditList.Sum();
            dataTable.Rows.Add(drow);
            for (int i = 0; i < expenseNameList.Count; i++)
            {
                DataRow dr = dataTable.NewRow();
                string[] exDate = Regex.Split(expenseDateList[i], " ");
                dr["ExpenseName"] = expenseNameList[i] + ", " + exDate[0];
                dr["ExpenseAmount"] = expenseAmountList[i];
                dataTable.Rows.Add(dr);
            }
            cashBalance = totalSalesList.Sum() - totalCreditList.Sum() - expenseAmountList.Sum();
            DataRow dro = dataTable.NewRow();
            dro["TotalExpense"] = expenseAmountList.Sum();
            dro["CashBalance"] = cashBalance;
            dataTable.Rows.Add(dro);
            report.Database.Tables["TotalMonthlyTable"].SetDataSource((DataTable)dataTable);
            totalMonthlyReportViewer.ViewerCore.ReportSource = report;
            report.Refresh();
        }

        private void storeExpense()
        {
            cc.OpenConnection();
            cc.DataReader("select Ename,Epayment,Edate from ExpenseTransactionDetails where Edate between '" + startDate + "' and '" + endDate + "'");
            while (cc.reader.Read())
            {
                expenseAmountList.Add(Convert.ToDouble(cc.reader["Epayment"]));
                expenseNameList.Add(Convert.ToString(cc.reader["Ename"]));
                expenseDateList.Add(Convert.ToString(cc.reader["Edate"]));
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void storeSales()
        {
            cc.OpenConnection();
            if (MainWindow.userName == "admin")
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
            //string companyNamePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Config\Config.txt");
            //string companyNameValue = File.ReadAllText(companyNamePath);
            //if (companyNameValue.Contains("CompanyName"))
            //{
            //    string[] val = companyNameValue.Substring(companyNameValue.IndexOf("CompanyName") + 13).Split(Convert.ToChar("'"));
            //    companyNameList.Add(val[0].ToString());
            //    foreach (var number in companyNameList)
            //    {
            //        companyName = Convert.ToString(number);
            //    }
            //}

            //string companyDetailsPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Config\Config.txt");
            //string companyDetailsValue = File.ReadAllText(companyDetailsPath);
            //if (companyDetailsValue.Contains("CompanyDetails"))
            //{
            //    string[] val = companyDetailsValue.Substring(companyDetailsValue.IndexOf("CompanyDetails") + 16).Split(Convert.ToChar("'"));
            //    companyDetailsList.Add(val[0].ToString());
            //    foreach (var number in companyDetailsList)
            //    {
            //        companyDetails = Convert.ToString(number);
            //    }
            //}
        }

        private void clearList()
        {
            totalSalesList.Clear();
            totalCreditList.Clear();
            expenseNameList.Clear();
            expenseAmountList.Clear();
            expenseDateList.Clear();
        }
    }
}
