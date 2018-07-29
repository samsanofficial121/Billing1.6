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
using System.IO;
using System.Reflection;

namespace Billing
{
    /// <summary>
    /// Interaction logic for BarCodePage.xaml
    /// </summary>
    public partial class BarCodePage : Page
    {
        public static string companyName;
        public static int writeText, billNumber, sqtyCount;
        public static List<int> idList = new List<int>();
        public static List<string> nameList = new List<string>();
        public static List<int> sqtyList = new List<int>();
        ConnectionClass cc = new ConnectionClass();

        public BarCodePage()
        {
            InitializeComponent();
            billNumber = PurchasePage.bno;
            barCodeGenerate();
        }

        private void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            PurchasePage pp = new PurchasePage();
            this.NavigationService.Navigate(pp);
            clearList();
        }

        private void clearList()
        {
            idList.Clear();
            nameList.Clear();
            sqtyList.Clear();
        }

        private void barCodeGenerate()
        {
            BarCodeDataSet barcodeDetails = new BarCodeDataSet();
            BarCodeReport Report = new BarCodeReport();
            DataTable dTable = barcodeDetails.CompanyDetails;
            readCompanyDetails();
            DataRow dr = dTable.NewRow();
            dr["CompanyName"] = companyName;
            dTable.Rows.Add(dr);
            Report.Database.Tables["CompanyDetails"].SetDataSource((DataTable)dTable);
            storeLists();
            DataTable datatable = barcodeDetails.BarCodeTable;
            for (int i = 0; i < idList.Count; i++)
            {
                sqtyCount = sqtyList[i];
                for (int j = 0; j < sqtyCount; j++)
                {
                    DataRow drow = datatable.NewRow();
                    int id = idList[i];
                    string I_name = Convert.ToString(id);
                    drow["ItemId"] = "*";
                    drow["ItemId"] += I_name;
                    drow["ItemId"] += "*";
                    drow["ItemName"] = nameList[i];
                    datatable.Rows.Add(drow);
                }

            }
            Report.Database.Tables["BarCodeTable"].SetDataSource((DataTable)datatable);
            barcodeReportViewer.ViewerCore.ReportSource = Report;
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
            cc.CloseConnection();
        }

        public static void storeLists()
        {
            ConnectionClass cc = new ConnectionClass();
            cc.OpenConnection();
            cc.DataReader("select itemid,iname,squantity from Stock where Bno=" + billNumber + "");
            while (cc.reader.Read())
            {
                idList.Add(Convert.ToInt32(cc.reader["itemid"]));
                nameList.Add(Convert.ToString(cc.reader["iname"]));
                sqtyList.Add(Convert.ToInt32(cc.reader["squantity"]));
            }
            cc.CloseReader();
            cc.CloseConnection();
        }
    }
}
