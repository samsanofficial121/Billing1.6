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
    /// Interaction logic for ExpenseReportViewer.xaml
    /// </summary>
    public partial class ExpenseReportViewer : Page
    {
        ConnectionClass cc = new ConnectionClass();
        public DateTime? fromDate = null, toDate = null;
        public string companyName, companyDetails;
        public static List<string> expenseNameList = new List<string>();
        public static List<string> expenseDateList = new List<string>();
        public static List<double> expenseAmountList = new List<double>();

        public ExpenseReportViewer(DateTime? frmDat, DateTime? toDat)
        {
            InitializeComponent();
            clearExpenseData();
            fromDate = frmDat;
            toDate = toDat;
            clearLists();
            generateReport();
        }

        private void clearExpenseData()
        {
            cc.OpenConnection();
            cc.ExecuteQuery("Delete from ExpenseData");
            cc.CloseConnection();
        }

        private void generateReport()
        {
            expenseAdd();
            ExpenseData reportDetails = new ExpenseData();
            DataTable dataTable = reportDetails.ExpenseTable;
            ExpenseReport Report = new ExpenseReport();
            readCompanyDetails();
            DataRow drow = dataTable.NewRow();
            drow["CompanyName"] = companyName;
            drow["CompanyDetails"] = companyDetails;
            string[] fDate = Regex.Split(fromDate.ToString(), " ");
            drow["DateFrom"] = fDate[0];
            string[] tDate = Regex.Split(toDate.ToString(), " ");
            drow["DateTo"] = tDate[0];
            dataTable.Rows.Add(drow);
            storeLists();
            for (int i = 0; i < expenseNameList.Count; i++)
            {
                DataRow dr = dataTable.NewRow();
                dr["ExpenseName"] = expenseNameList[i];
                string[] eDate = Regex.Split(expenseDateList[i], " ");
                dr["ExpenseDate"] = eDate[0];
                dr["ExpenseAmount"] = expenseAmountList[i];
                dataTable.Rows.Add(dr);
            }
            DataRow drr = dataTable.NewRow();
            drr["ExpenseNumber"] = expenseNameList.Count;
            drr["TotalExpense"] = expenseAmountList.Sum();
            dataTable.Rows.Add(drr);
            Report.Database.Tables["ExpenseTable"].SetDataSource((DataTable)dataTable);
            expenseReportViewer.ViewerCore.ReportSource = Report;
            Report.Refresh();
        }

        private void expenseAdd()
        {
            cc.OpenConnection();
            cc.ExecuteQuery("insert into ExpenseData(ExpenseDate,ExpenseName,ExpenseAmount) select distinct Edate,Ename,Epayment from ExpenseTransactionDetails where Edate Between '" + fromDate + "' And '" + toDate + "' Group By Edate,Ename,Epayment");
            cc.CloseConnection();
        }

        private void storeLists()
        {
            cc.OpenConnection();
            cc.DataReader("select * from ExpenseData");
            while (cc.reader.Read())
            {
                expenseNameList.Add(Convert.ToString(cc.reader["ExpenseName"]));
                expenseDateList.Add(Convert.ToString(cc.reader["ExpenseDate"]));
                expenseAmountList.Add(Convert.ToDouble(cc.reader["ExpenseAmount"]));
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void clearLists()
        {
            expenseAmountList.Clear();
            expenseDateList.Clear();
            expenseNameList.Clear();
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
    }
}
