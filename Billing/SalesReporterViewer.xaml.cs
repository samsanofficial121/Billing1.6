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
using System.Data;
using System.Globalization;

namespace Billing
{
    /// <summary>
    /// Interaction logic for SalesReporterViewer.xaml
    /// </summary>
    public partial class SalesReporterViewer : Page
    {
        ConnectionClass cc = new ConnectionClass();
        public static List<int> bnoList = new List<int>();
        public static List<int> billNoList = new List<int>();
        public static List<int> bnoFromList = new List<int>();
        public static List<int> bnoToList = new List<int>();
        public static List<string> saleDateList = new List<string>();
        public static List<double> totalPriceList = new List<double>();
        public static List<double> netGstList = new List<double>();
        public static List<double> salesReturnList = new List<double>();
        public static List<double> grandTotalList = new List<double>();
        public static List<double> totalExpenseList = new List<double>();
        public static List<string> expenseDateList = new List<string>();
        public static List<double> creditList = new List<double>();
        public static List<double> cashPaidList = new List<double>();
        public static List<string> particularsList = new List<string>();
        public DateTime? fromDate = null, toDate = null;
        public string typeOfReport;

        public SalesReporterViewer(string typrpt,DateTime? frmDat,DateTime? toDat)
        {
            InitializeComponent();
            clearSalesData();
            typeOfReport = typrpt;
            fromDate = frmDat;
            toDate = toDat;
            clearList();
            reportAction();
        }

        private void reportAction()
        {
            if (typeOfReport == "Total Report")
            {
                if (fromDate != null && toDate != null)
                {
                    clearList();
                    generateReport();
                }
                else
                {
                    MessageBox.Show("Select date from and to");
                }
            }
            else if(typeOfReport == "Daily Report")
            {
                clearList();
                if (fromDate != null && toDate != null)
                {
                    clearList();
                    clearSalesData();
                    dailyReport();
                }
                else
                {
                    MessageBox.Show("Select date from and to");
                }
            }
            else if(typeOfReport == "Monthly Report")
            {
                clearList();
                if (fromDate != null && toDate != null)
                {
                    clearList();
                    clearSalesData();
                    monthlyReport();
                }
                else
                {
                    MessageBox.Show("Select date from and to");
                }
            }
        }


        private void generateReport()
        {
            SalesReporterData reportDetails = new SalesReporterData();
            DataTable dataTable = reportDetails.SalesReporterTable;

            SalesReporter Report = new SalesReporter();
            storeList();
            for (int i = 0; i < bnoList.Count; i++)
            {
                DataRow drow = dataTable.NewRow();
                drow["DateFrom"] = fromDate;
                drow["DateTo"] = toDate;
                drow["BillNo"] = bnoList[i];
                drow["SaleDate"] = saleDateList[i];
                drow["Particulars"] = particularsList[i];
                drow["TotalPrice"] = totalPriceList[i];
                drow["Gst"] = netGstList[i];
                drow["SalesReturn"] = salesReturnList[i];
                drow["Credit"] = creditList[i];
                drow["CashPaid"] = cashPaidList[i];
                drow["GrandTotal"] = grandTotalList[i];
                dataTable.Rows.Add(drow);
            }
            DataRow droww = dataTable.NewRow();
            droww["NetAmount"] = totalPriceList.Sum();
            droww["TotalGst"] = netGstList.Sum();
            droww["TotalSalesReturn"] = salesReturnList.Sum();
            droww["TotalCredit"] = creditList.Sum();
            droww["TotalCashPaid"] = cashPaidList.Sum();
            droww["TotalGrandTotal"] = grandTotalList.Sum();
            droww["TotalInvoice"] = bnoList.Count;
            dataTable.Rows.Add(droww);
            Report.Database.Tables["SalesReporterTable"].SetDataSource((DataTable)dataTable);
            expenseData();
            DataTable dTable = reportDetails.ExpenseData;
            DataRow dr = dTable.NewRow();
            dr["TotalExpense"] = totalExpenseList.Sum();
            dTable.Rows.Add(dr);
            Report.Database.Tables["ExpenseData"].SetDataSource((DataTable)dTable);
            sales_Reporter_Viewer.ViewerCore.ReportSource = Report;
            Report.Refresh();
        }

        private void expenseData()
        {
            cc.OpenConnection();
            cc.DataReader("select Epayment from ExpenseTransactionDetails where Edate Between #" + fromDate + "# And #" + toDate + "#");
            while (cc.reader.Read())
            {
                totalExpenseList.Add(Convert.ToDouble(cc.reader["Epayment"]));
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void storeList()
        {
            cc.OpenConnection();
            cc.DataReader("select BillNo,SaleDate,NetAmount,netGst,SalesReturn,Credit,CashPaid,Particulars,SalesReturn+NetAmount as GrandTotal from (select distinct BillNo,SaleDate,NetAmount,netGst,SalesReturn,Credit,CashPaid,Particulars from BillStock) where SaleDate Between #" + fromDate + "# And #" + toDate + "# Group By BillNo,SaleDate,NetAmount,netGst,SalesReturn,Credit,CashPaid,Particulars");
            while (cc.reader.Read())
            {
                bnoList.Add(Convert.ToInt32(cc.reader["BillNo"]));
                saleDateList.Add(Convert.ToString(cc.reader["SaleDate"]));
                totalPriceList.Add(Convert.ToDouble(cc.reader["GrandTotal"]));
                netGstList.Add(Convert.ToDouble(cc.reader["netGst"]));
                salesReturnList.Add(Convert.ToDouble(cc.reader["SalesReturn"]));
                creditList.Add(Convert.ToDouble(cc.reader["Credit"]));
                cashPaidList.Add(Convert.ToDouble(cc.reader["CashPaid"]));
                particularsList.Add(Convert.ToString(cc.reader["Particulars"]));
                grandTotalList.Add(Convert.ToDouble(cc.reader["NetAmount"]));
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void clearList()
        {
            bnoList.Clear();
            bnoFromList.Clear();
            bnoToList.Clear();
            saleDateList.Clear();
            totalPriceList.Clear();
            netGstList.Clear();
            salesReturnList.Clear();
            grandTotalList.Clear();
            totalExpenseList.Clear();
            expenseDateList.Clear();
            creditList.Clear();
            cashPaidList.Clear();
            particularsList.Clear();
        }

        private void clearSalesData()
        {
            cc.OpenConnection();
            cc.ExecuteQuery("delete * from SalesData");
            cc.CloseConnection();
        }

        private void dailyReport()
        {
            DailySalesData reportDetails = new DailySalesData();
            DataTable dataTable = reportDetails.DailySaleData;

            DailySalesReport Report = new DailySalesReport();
            addList();
            for (int i = 0; i < saleDateList.Count; i++)
            {
                DataRow drow = dataTable.NewRow();
                drow["DateFrom"] = fromDate;
                drow["DateTo"] = toDate;
                drow["BillNoFrom"] = bnoFromList[i];
                drow["BillNoTo"] = bnoToList[i];
                drow["SaleDate"] = saleDateList[i];
                drow["GrossValue"] = totalPriceList[i];
                drow["Gst"] = netGstList[i];
                drow["SalesReturn"] = salesReturnList[i];
                drow["GrandTotal"] = grandTotalList[i];
                drow["Credit"] = creditList[i];
                drow["CashPaid"] = cashPaidList[i];
                drow["TotalExpense"] = totalExpenseList[i];
                dataTable.Rows.Add(drow);
            }
            DataRow droww = dataTable.NewRow();
            droww["TotalGross"] = totalPriceList.Sum();
            droww["TotalGst"] = netGstList.Sum();
            droww["TotalSalesReturn"] = salesReturnList.Sum();
            droww["TotalGrand"] = grandTotalList.Sum();
            droww["TotalCredit"] = creditList.Sum();
            droww["TotalCashPaid"] = cashPaidList.Sum();
            droww["NetExpense"] = totalExpenseList.Sum();
            dataTable.Rows.Add(droww);
            Report.Database.Tables["DailySaleData"].SetDataSource((DataTable)dataTable);
            sales_Reporter_Viewer.ViewerCore.ReportSource = Report;
            Report.Refresh();
        }

        private void monthlyReport()
        {
            MonthlyReportData reportDetails = new MonthlyReportData();
            DataTable dataTable = reportDetails._MonthlyReportData;

            MonthlyReport Report = new MonthlyReport();
            monthlyList();
            for (int i = 0; i < saleDateList.Count; i++)
            {
                DataRow drow = dataTable.NewRow();
                drow["DateFrom"] = fromDate;
                drow["DateTo"] = toDate;
                drow["BillNoFrom"] = bnoFromList[i];
                drow["BillNoTo"] = bnoToList[i];
                drow["SaleDate"] = saleDateList[i];
                drow["GrossValue"] = totalPriceList[i];
                drow["Gst"] = netGstList[i];
                drow["SalesReturn"] = salesReturnList[i];
                drow["Credit"] = creditList[i];
                drow["CashPaid"] = cashPaidList[i];
                drow["GrandTotal"] = grandTotalList[i];
                drow["TotalExpense"] = totalExpenseList[i];
                dataTable.Rows.Add(drow);
            }
            DataRow droww = dataTable.NewRow();
            droww["TotalGross"] = totalPriceList.Sum();
            droww["TotalGst"] = netGstList.Sum();
            droww["TotalSalesReturn"] = salesReturnList.Sum();
            droww["TotalCredit"] = creditList.Sum();
            droww["TotalCashPaid"] = cashPaidList.Sum();
            droww["TotalGrand"] = grandTotalList.Sum();
            droww["NetExpense"] = totalExpenseList.Sum();
            dataTable.Rows.Add(droww);
            Report.Database.Tables["MonthlyReportData"].SetDataSource((DataTable)dataTable);
            sales_Reporter_Viewer.ViewerCore.ReportSource = Report;
            Report.Refresh();
        }

        private void addList()
        {
            salesDataTable();
            cc.OpenConnection();
            cc.DataReader("select Min(BillNo) as BnoFrom,Max(BillNo) as BnoTo,SaleDate,sum(NetAmount) as GrossValue,sum(netGst) as Gst,sum(SalesReturn) as TotalSalesReturn,sum(GrandTotal) as GTotal, sum(Credit) as NetCredit, sum(CashPaid) as NetCashPaid, sum(ExpenseAmount) as DailyExpense from SalesData Group By SaleDate");
            while (cc.reader.Read())
            {
                bnoFromList.Add(Convert.ToInt32(cc.reader["BnoFrom"]));
                bnoToList.Add(Convert.ToInt32(cc.reader["BnoTo"]));
                saleDateList.Add(Convert.ToString(cc.reader["SaleDate"]));
                totalPriceList.Add(Convert.ToDouble(cc.reader["GTotal"]));
                netGstList.Add(Convert.ToDouble(cc.reader["Gst"]));
                salesReturnList.Add(Convert.ToDouble(cc.reader["TotalSalesReturn"]));
                grandTotalList.Add(Convert.ToDouble(cc.reader["GrossValue"]));
                creditList.Add(Convert.ToDouble(cc.reader["NetCredit"]));
                cashPaidList.Add(Convert.ToDouble(cc.reader["NetCashPaid"]));
                totalExpenseList.Add(Convert.ToDouble(cc.reader["DailyExpense"]));
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void monthlyList()
        {
            salesDataTable();
            cc.OpenConnection();
            cc.DataReader("select Min(BillNo) as BnoFrom,Max(BillNo) as BnoTo,month(SaleDate) as Mon,sum(NetAmount) as GrossValue,sum(netGst) as Gst,sum(SalesReturn) as TotalSalesReturn,sum(GrandTotal) as GTotal,sum(Credit) as TotalCredit,sum(CashPaid) as TotalCashPaid, sum(ExpenseAmount) as MonthlyExpense from SalesData Group By month(SaleDate)");
            while (cc.reader.Read())
            {
                bnoFromList.Add(Convert.ToInt32(cc.reader["BnoFrom"]));
                bnoToList.Add(Convert.ToInt32(cc.reader["BnoTo"]));
                months();
                totalPriceList.Add(Convert.ToDouble(cc.reader["GTotal"]));
                netGstList.Add(Convert.ToDouble(cc.reader["Gst"]));
                salesReturnList.Add(Convert.ToDouble(cc.reader["TotalSalesReturn"]));
                creditList.Add(Convert.ToDouble(cc.reader["TotalCredit"]));
                cashPaidList.Add(Convert.ToDouble(cc.reader["TotalCashPaid"]));
                grandTotalList.Add(Convert.ToDouble(cc.reader["GrossValue"]));
                totalExpenseList.Add(Convert.ToDouble(cc.reader["MonthlyExpense"]));
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void months()
        {
            var date = DateTime.Now;
            string year = date.ToString("yyyy");
            if (Convert.ToInt32(cc.reader["Mon"]) == 1)
            {
                saleDateList.Add("January," + year);
            }
            else if(Convert.ToInt32(cc.reader["Mon"]) == 2)
            {
                saleDateList.Add("February," + year);
            }
            else if (Convert.ToInt32(cc.reader["Mon"]) == 3)
            {
                saleDateList.Add("March," + year);
            }
            else if (Convert.ToInt32(cc.reader["Mon"]) == 4)
            {
                saleDateList.Add("April," + year);
            }
            else if (Convert.ToInt32(cc.reader["Mon"]) == 5)
            {
                saleDateList.Add("May," + year);
            }
            else if (Convert.ToInt32(cc.reader["Mon"]) == 6)
            {
                saleDateList.Add("June," + year);
            }
            else if (Convert.ToInt32(cc.reader["Mon"]) == 7)
            {
                saleDateList.Add("July," + year);
            }
            else if (Convert.ToInt32(cc.reader["Mon"]) == 8)
            {
                saleDateList.Add("August," + year);
            }
            else if (Convert.ToInt32(cc.reader["Mon"]) == 9)
            {
                saleDateList.Add("September," + year);
            }
            else if (Convert.ToInt32(cc.reader["Mon"]) == 10)
            {
                saleDateList.Add("October," + year);
            }
            else if (Convert.ToInt32(cc.reader["Mon"]) == 11)
            {
                saleDateList.Add("November," + year);
            }
            else if (Convert.ToInt32(cc.reader["Mon"]) == 12)
            {
                saleDateList.Add("December," + year);
            }
        }

        private void salesDataTable()
        {
            cc.OpenConnection();
            cc.ExecuteQuery("insert into SalesData(BillNo,SaleDate,NetAmount,netGst,SalesReturn,GrandTotal,Credit,CashPaid) select distinct BillNo,SaleDate,NetAmount,netGst,SalesReturn,SalesReturn+NetAmount,Credit,CashPaid as GrandTotal from BillStock where SaleDate Between #" + fromDate + "# And #" + toDate + "# Group By BillNo,SaleDate,NetAmount,netGst,SalesReturn,Credit,CashPaid");
            cc.CloseConnection();
            expenseAdd();
        }

        private void expenseAdd()
        {
            cc.OpenConnection();
            cc.ExecuteQuery("insert into SalesData(SaleDate,ExpenseName,ExpenseAmount) select distinct Edate,Ename,Epayment from ExpenseTransactionDetails where Edate Between #" + fromDate + "# And #" + toDate + "# Group By Edate,Ename,Epayment");
            cc.CloseConnection();
        }
    }
}
