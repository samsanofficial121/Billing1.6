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

namespace Billing
{
    /// <summary>
    /// Interaction logic for BarCodePage.xaml
    /// </summary>
    public partial class BarCodePage : Page
    {
        public static string readText;
        public static int writeText;
        public static List<int> idList = new List<int>();
        public static List<string> nameList = new List<string>();
        public static List<int> sqtyList = new List<int>();
        public static int sqtyCount;

        public BarCodePage()
        {
            InitializeComponent();
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
            DataTable datatable = barcodeDetails.BarCodeTable;
            BarCodeReport Report = new BarCodeReport();
            storeLists();
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

        public static void storeLists()
        {
            ConnectionClass cc = new ConnectionClass();
            cc.OpenConnection();
            cc.DataReader("select Item_ID,Item_Name,SQuantity from Purchase");
            while (cc.reader.Read())
            {
                 idList.Add(Convert.ToInt32(cc.reader["Item_ID"]));
                 nameList.Add(Convert.ToString(cc.reader["Item_Name"]));
                 sqtyList.Add(Convert.ToInt32(cc.reader["SQuantity"]));
             }
            cc.CloseReader();
            cc.CloseConnection();
        }
    }
}
