using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Reflection;
using System.Data;
using System.Text.RegularExpressions;

namespace Billing
{
    /// <summary>
    /// Interaction logic for CustomerReportViewer.xaml
    /// </summary>
    public partial class CustomerReportViewer : Page
    {
        ConnectionClass cc = new ConnectionClass();
        public static List<string> customerNameList = new List<string>();
        public static List<string> customerPhoneList = new List<string>();
        public static List<double> creditList = new List<double>();
        public static string companyName, companyDetails;

        public CustomerReportViewer()
        {
            InitializeComponent();
            clearLists();
            clearData();
            generateReport();
        }

        private void clearData()
        {
            cc.OpenConnection();
            cc.ExecuteQuery("Delete from CustomerReport");
            cc.CloseConnection();
        }

        private void generateReport()
        {
            CustomerReportData reportDetails = new CustomerReportData();
            CustomerReport report = new CustomerReport();
            DataTable dataTable = reportDetails.CompanyDetails;
            readCompanyDetails();
            DataRow drr = dataTable.NewRow();
            drr["CompanyName"] = companyName;
            drr["CompanyDetails"] = companyDetails;
            dataTable.Rows.Add(drr);
            report.Database.Tables["CompanyDetails"].SetDataSource((DataTable)dataTable);
            DataTable daTable = reportDetails.CustomerReportTable;
            storeLists();
            for (int i = 0; i < customerNameList.Count; i++)
            {
                DataRow drow = daTable.NewRow();
                string[] custName = Regex.Split(customerNameList[i], "  ");
                drow["CustomerName"] = custName[0];
                drow["CustomerPhone"] = customerPhoneList[i];
                drow["Credit"] = creditList[i];
                daTable.Rows.Add(drow);
            }
            DataRow dr = daTable.NewRow();
            dr["TotalCredit"] = creditList.Sum();
            daTable.Rows.Add(dr);
            report.Database.Tables["CustomerReportTable"].SetDataSource((DataTable)daTable);
            customerReportViewer.ViewerCore.ReportSource = report;
            report.Refresh();
        }

        private void storeLists()
        {
            storeData();
            cc.OpenConnection();
            cc.DataReader("select * from CustomerReport where Credit > 0");
            while (cc.reader.Read())
            {
                customerNameList.Add(Convert.ToString(cc.reader["CustomerName"]));
                customerPhoneList.Add(Convert.ToString(cc.reader["CustomerPhone"]));
                creditList.Add(Convert.ToDouble(cc.reader["Credit"]));
            }
        }

        private void storeData()
        {
            if(MainWindow.userName=="admin")
            {
                cc.OpenConnection();
                cc.ExecuteQuery("insert into CustomerReport(CustomerName,CustomerPhone,Credit) select CustomerName,CustomerPhone,sum(NetAmount) - sum(AmountPaid) as Credit from CustomerTransactions Group By CustomerName,CustomerPhone");
                cc.CloseConnection();
            }
            else
            {
                cc.OpenConnection();
                cc.ExecuteQuery("insert into CustomerReport(CustomerName,CustomerPhone,Credit) select CustomerName,CustomerPhone,sum(NetAmount) - sum(AmountPaid) as Credit from CustomerTransactions where BillType='GST' Group By CustomerName,CustomerPhone");
                cc.CloseConnection();
            }
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

        private void clearLists()
        {
            customerNameList.Clear();
            customerPhoneList.Clear();
            creditList.Clear();
        }

        private void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            CustomerTab ct = new CustomerTab();
            this.NavigationService.Navigate(ct);
        }
    }
}