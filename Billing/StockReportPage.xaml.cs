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
using System.Data.OleDb;
using System.Configuration;

namespace Billing
{
    /// <summary>
    /// Interaction logic for SalesReportPage.xaml
    /// </summary>
    public partial class StockReportPage : Page
    {
        public static List<int> sList = new List<int>();
        public static List<int> iList = new List<int>();
        public static List<string> nList = new List<string>();
        public static List<double> rList = new List<double>();
        public static List<int> qList = new List<int>();
        public static List<double> gstList = new List<double>();
        public static List<string> gstpList = new List<string>();
        public static List<double> spList = new List<double>();
        ConnectionClass cc = new ConnectionClass();

        public StockReportPage()
        {
            InitializeComponent();
            clearList();
            generateReport();
        }

        private void generateReport()
        {
            StockReportData reportDetails = new StockReportData();
            DataTable dataTable = reportDetails.StockData;

            StockReport Report = new StockReport();
            addData();
            for (int i = 0; i < sList.Count; i++)
            {
                DataRow drow = dataTable.NewRow();
                drow["SlNo"] = sList[i];
                drow["ItemNo"] = iList[i];
                drow["ItemName"] = nList[i];
                drow["Rate"] = rList[i];
                drow["Quantity"] = qList[i];
                drow["GstRate"] = gstList[i];
                drow["SalePrice"] = spList[i];
                drow["Gst%"] = gstpList[i];
                dataTable.Rows.Add(drow);
            }
            Report.Database.Tables["StockData"].SetDataSource((DataTable)dataTable);
            stockReportViewer.ViewerCore.ReportSource = Report;

        }

        private void addData()
        {
            cc.OpenConnection();
            cc.DataReader("select * from Stock ORDER BY Sno ASC");
            while (cc.reader.Read())
            {
                sList.Add(Convert.ToInt32(cc.reader["Sno"]));
                iList.Add(Convert.ToInt32(cc.reader["itemid"]));
                nList.Add(Convert.ToString(cc.reader["iname"]));
                rList.Add(Convert.ToDouble(cc.reader["rate"]));
                qList.Add(Convert.ToInt32(cc.reader["quantity"]));
                gstList.Add(Convert.ToDouble(cc.reader["gst_amount"]));
                spList.Add(Convert.ToDouble(cc.reader["sprice"]));
                gstpList.Add(Convert.ToString(cc.reader["gst_percent"]));
            }
            cc.CloseReader();
        }

        private void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            ReportsPage rp = new ReportsPage();
            this.NavigationService.Navigate(rp);
            clearList();
        }

        private void clearList()
        {
            sList.Clear();
            iList.Clear();
            nList.Clear();
            rList.Clear();
            qList.Clear();
            gstList.Clear();
            spList.Clear();
            gstpList.Clear();
        }
    }
}
