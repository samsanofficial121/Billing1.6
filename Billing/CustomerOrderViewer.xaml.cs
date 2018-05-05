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

namespace Billing
{
    /// <summary>
    /// Interaction logic for CustomerOrderViewer.xaml
    /// </summary>
    public partial class CustomerOrderViewer : Page
    {
        public static List<string> itemNameList = new List<string>();
        public static List<int> quantityList = new List<int>();
        public static List<string> customerNameList = new List<string>();
        public static List<string> customerPhoneList = new List<string>();
        public static List<double> advanceAmountList = new List<double>();
        public static List<string> orderDateList = new List<string>();
        ConnectionClass cc = new ConnectionClass();

        public CustomerOrderViewer()
        {
            InitializeComponent();
            clearList();
            generateReport();
        }

        private void generateReport()
        {
            CustomerOrderDataSet reportDetails = new CustomerOrderDataSet();
            DataTable dataTable = reportDetails._CustomerOrderDataSet;
            CustomerOrderReport Report = new CustomerOrderReport();
            addData();
            for (int i = 0; i < itemNameList.Count; i++)
            {
                DataRow drow = dataTable.NewRow();
                drow["ItemName"] = itemNameList[i];
                drow["Quantity"] = quantityList[i];
                drow["CustomerName"] = customerNameList[i];
                drow["CustomerPhone"] = customerPhoneList[i];
                drow["Advance"] = advanceAmountList[i];
                drow["OrderDate"] = orderDateList[i];
                dataTable.Rows.Add(drow);
            }
            Report.Database.Tables["CustomerOrderDataSet"].SetDataSource((DataTable)dataTable);
            CustomerOrderReportViewer.ViewerCore.ReportSource = Report;
        }

        private void addData()
        {
            cc.OpenConnection();
            cc.DataReader("select * from OrderedItems");
            while (cc.reader.Read())
            {
                itemNameList.Add(Convert.ToString(cc.reader["OItemName"]));
                quantityList.Add(Convert.ToInt32(cc.reader["OQuantity"]));
                customerNameList.Add(Convert.ToString(cc.reader["OCustomerName"]));
                customerPhoneList.Add(Convert.ToString(cc.reader["OCustomerPhone"]));
                advanceAmountList.Add(Convert.ToDouble(cc.reader["OAdvance"]));
                orderDateList.Add(Convert.ToString(cc.reader["OrderDate"]));
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
            itemNameList.Clear();
            quantityList.Clear();
            customerNameList.Clear();
            customerPhoneList.Clear();
            advanceAmountList.Clear();
            orderDateList.Clear();
        }
    }
}
