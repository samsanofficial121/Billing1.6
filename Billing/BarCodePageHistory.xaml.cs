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
    /// Interaction logic for BarCodePageHistory.xaml
    /// </summary>
    public partial class BarCodePageHistory : Page
    {
        public static List<int> idList = new List<int>();
        public static List<string> nameList = new List<string>();
        public static List<int> sqtyList = new List<int>();
        public static int sqtyCount, billNumber;

        public BarCodePageHistory()
        {
            InitializeComponent();
            billNumber = PurchaseHistory.bno;
            barCodeGenerate();
        }

        private void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            PurchaseHistory ph = new PurchaseHistory();
            this.NavigationService.Navigate(ph);
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
            barCodeHistoryViewer.ViewerCore.ReportSource = Report;
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
