using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
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
    /// Interaction logic for ChartPage.xaml
    /// </summary>
    public partial class ChartPage : Page
    {
        public static List<double> cmNetAmountList = new List<double>();
        public static List<double> cmCreditList = new List<double>();
        public static List<double> lmNetAmountList = new List<double>();
        public static List<double> lmCreditList = new List<double>();
        public static List<double> agoNetAmountList = new List<double>();
        public static List<double> agoCreditList = new List<double>();
        string currentMonth, lastMonth, agoLastMonth, fromCurrent, toCurrent, fromLast, toLast, fromAgoLast, toAgoLast;
        string currentMonthSales, currentMonthCredit, currentMonthExpense;
        string lastMonthSales, lastMonthCredit, lastMonthExpense;
        string agoLastMonthSales, agoLastMonthCredit, agoLastMonthExpense;
        ConnectionClass cc = new ConnectionClass();

        public ChartPage()
        {
            InitializeComponent();
            ClearLists();
            GetDateTime();
            CurrentMonthData();
            LastMonthData();
            AgoLastMonthData();
            LoadBarChartData();
        }

        private void GetDateTime()
        {
            var date = DateTime.Now;
            string formattedDate = date.ToString("dd-MMM-yy hh:mm:ss tt");
            var current = date.ToString("MMMM-yy", CultureInfo.InvariantCulture);
            currentMonth = current;
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddSeconds(-1);
            fromCurrent = Convert.ToString(firstDayOfMonth);
            toCurrent = Convert.ToString(lastDayOfMonth);
            var last = date.AddMonths(-1).ToString("MMMM-yy", CultureInfo.InvariantCulture);
            lastMonth = last;
            var firstDayOfLastMonth = firstDayOfMonth.AddMonths(-1);
            var lastDayOfLastMonth = firstDayOfLastMonth.AddMonths(1).AddSeconds(-1);
            fromLast = Convert.ToString(firstDayOfLastMonth);
            toLast = Convert.ToString(lastDayOfLastMonth);
            var agolast = date.AddMonths(-2).ToString("MMMM-yy", CultureInfo.InvariantCulture);
            agoLastMonth = agolast;
            var firstDayOfAgoLastMonth = firstDayOfLastMonth.AddMonths(-1);
            var lastDayOfAgoLastMonth = firstDayOfAgoLastMonth.AddMonths(1).AddSeconds(-1);
            fromAgoLast = Convert.ToString(firstDayOfAgoLastMonth);
            toAgoLast = Convert.ToString(lastDayOfAgoLastMonth);
        }

        private void ClearLists()
        {
            cmNetAmountList.Clear();
            cmCreditList.Clear();
            lmNetAmountList.Clear();
            lmCreditList.Clear();
            agoNetAmountList.Clear();
            agoCreditList.Clear();
        }

        private void CurrentMonthData()
        {
            cc.OpenConnection();
            if (MainWindow.userName == "admin")
            {
                cc.DataReader("select distinct BillNo,NetAmount,Credit from BillStock where SaleDate between '" + fromCurrent + "' and '" + toCurrent + "'");
            }
            else
            {
                cc.DataReader("select distinct BillNo,NetAmount,Credit from BillStock where SaleDate between '" + fromCurrent + "' and '" + toCurrent + "' and BillType='GST'");
            }
            while (cc.reader.Read())
            {
                cmNetAmountList.Add(Convert.ToDouble(cc.reader["NetAmount"]));
                cmCreditList.Add(Convert.ToDouble(cc.reader["Credit"]));
            }
            cc.CloseReader();
            currentMonthSales = cmNetAmountList.Sum().ToString();
            currentMonthCredit = cmCreditList.Sum().ToString();
            cc.DataReader("select month(Edate) as Mon,sum(Epayment) as Expense from ExpenseTransactionDetails where Edate between '" + fromCurrent + "' and '" + toCurrent + "' group by month(Edate)");
            while (cc.reader.Read())
            {
                currentMonthExpense = cc.reader["Expense"].ToString();
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void LastMonthData()
        {
            cc.OpenConnection();
            if (MainWindow.userName == "admin")
            {
                cc.DataReader("select distinct BillNo,NetAmount,Credit from BillStock where SaleDate between '" + fromLast + "' and '" + toLast + "'");
            }
            else
            {
                cc.DataReader("select distinct BillNo,NetAmount,Credit from BillStock where SaleDate between '" + fromLast + "' and '" + toLast + "' and BillType='GST'");
            }
            while (cc.reader.Read())
            {
                lmNetAmountList.Add(Convert.ToDouble(cc.reader["NetAmount"]));
                lmCreditList.Add(Convert.ToDouble(cc.reader["Credit"]));
            }
            cc.CloseReader();
            lastMonthSales = lmNetAmountList.Sum().ToString();
            lastMonthCredit = lmCreditList.Sum().ToString();
            cc.DataReader("select month(Edate) as Mon,sum(Epayment) as Expense from ExpenseTransactionDetails where Edate between '" + fromLast + "' and '" + toLast + "' group by month(Edate)");
            while (cc.reader.Read())
            {
                lastMonthExpense = cc.reader["Expense"].ToString();
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void AgoLastMonthData()
        {
            cc.OpenConnection();
            if (MainWindow.userName == "admin")
            {
                cc.DataReader("select distinct BillNo,NetAmount,Credit from BillStock where SaleDate between '" + fromAgoLast + "' and '" + toAgoLast + "'");
            }
            else
            {
                cc.DataReader("select distinct BillNo,NetAmount,Credit from BillStock where SaleDate between '" + fromAgoLast + "' and '" + toAgoLast + "' and BillType='GST'");
            }
            while (cc.reader.Read())
            {
                agoNetAmountList.Add(Convert.ToDouble(cc.reader["NetAmount"]));
                agoCreditList.Add(Convert.ToDouble(cc.reader["Credit"]));
            }
            cc.CloseReader();
            agoLastMonthSales = agoNetAmountList.Sum().ToString();
            agoLastMonthCredit = agoCreditList.Sum().ToString();
            cc.DataReader("select month(Edate) as Mon,sum(Epayment) as Expense from ExpenseTransactionDetails where Edate between '" + fromAgoLast + "' and '" + toAgoLast + "' group by month(Edate)");
            while (cc.reader.Read())
            {
                agoLastMonthExpense = cc.reader["Expense"].ToString();
            }
            cc.CloseReader();
            cc.CloseConnection();
        }

        private void LoadBarChartData()

        {

            ((ColumnSeries)mcChart.Series[0]).ItemsSource =

                new KeyValuePair<string, double>[]{

            new KeyValuePair<string, double>(agoLastMonth, Convert.ToDouble(agoLastMonthSales)),

            new KeyValuePair<string, double>(lastMonth, Convert.ToDouble(lastMonthSales)),

            new KeyValuePair<string, double>(currentMonth, Convert.ToDouble(currentMonthSales))};


            ((ColumnSeries)mcChart.Series[1]).ItemsSource =

                new KeyValuePair<string, double>[]{

            new KeyValuePair<string, double>(agoLastMonth, Convert.ToDouble(agoLastMonthCredit)),

            new KeyValuePair<string, double>(lastMonth, Convert.ToDouble(lastMonthCredit)),

            new KeyValuePair<string, double>(currentMonth, Convert.ToDouble(currentMonthCredit))};

            ((ColumnSeries)mcChart.Series[2]).ItemsSource =

                new KeyValuePair<string, double>[]{

            new KeyValuePair<string, double>(agoLastMonth, Convert.ToDouble(agoLastMonthExpense)),

            new KeyValuePair<string, double>(lastMonth, Convert.ToDouble(lastMonthExpense)),

            new KeyValuePair<string, double>(currentMonth, Convert.ToDouble(currentMonthExpense))};

        }
    }
}
