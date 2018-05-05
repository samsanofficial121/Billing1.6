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
    /// Interaction logic for PurchasedVendor.xaml
    /// </summary>
    public partial class PurchasedVendor : Page
    {
        ConnectionClass cc = new ConnectionClass();
        private string VnameCombo;

        public PurchasedVendor(string selectedName)
        {
            InitializeComponent();
            VnameCombo = selectedName;
            LoadVendor();
            cc.OpenConnection();
            cc.CreateView("CREATE VIEW PurchasedVendor AS Select Stock.iname,Stock.rate,Stock.quantity,Stock.total,Stock.gtotal,TransactionDetails.Vname,TransactionDetails.PurchaseDate from Stock,TransactionDetails where Stock.Bno=TransactionDetails.Bno");
            //cc.DataGridDisplay("select iname,rate,quantity,total,gtotal,PurchaseDate from Stock where Vname='"+VnameCombo+"'");
            cc.DataGridDisplay("Select * from PurchasedVendor where Vname='" + VnameCombo + "'");
            dataGridPurchasedVendor.ItemsSource = cc.dt.AsDataView();
            dataGridPurchasedVendor.Visibility = System.Windows.Visibility.Visible;
            cc.DropView("DROP VIEW PurchasedVendor");
            cc.CloseConnection();
        }

        private void LoadVendor()
        {

            cc.OpenConnection();
            cc.DataReader("select Vid,Vname,Vphone,Vplace from TransactionDetails where Vname='" + VnameCombo+"'");
            while (cc.reader.Read())
            {
                textBlockVId.Text = cc.reader["Vid"].ToString();
                textBlockVName.Text = cc.reader["Vname"].ToString();
                textBlockVPhone.Text = cc.reader["Vphone"].ToString();
                textBlockVPlace.Text = cc.reader["Vplace"].ToString();
            }
            cc.CloseReader();
            cc.CloseConnection();
        }
    }
}
