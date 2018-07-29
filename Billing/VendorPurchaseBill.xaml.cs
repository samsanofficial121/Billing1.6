using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;

namespace Billing
{
    /// <summary>
    /// Interaction logic for VendorPurchaseBill.xaml
    /// </summary>
    public partial class VendorPurchaseBill : Window
    {
        ConnectionClass cc = new ConnectionClass();
        public static string billNumber;
        public VendorPurchaseBill()
        {
            InitializeComponent();
            billNumber = VendorPurchases.billNo;
            ShowGrid();
        }

        private void ShowGrid()
        {
            ShowDetails();
            if(MainWindow.userName=="admin")
            {
                if(MainWindow.isgst==1)
                {
                    cc.DataGridDisplay("select itemid,iname,rate,purchaseQty,total,sprice,gst_percent,gst_amount from Stock where Bno = " + billNumber + " and BillType='GST' ");
                }
                else
                {
                    cc.DataGridDisplay("select itemid,iname,rate,purchaseQty,total,sprice,gst_percent,gst_amount from Stock where Bno = " + billNumber + " and BillType='NON_GST' ");
                }
            }
            else
            {
                cc.DataGridDisplay("select itemid,iname,rate,purchaseQty,total,sprice,gst_percent,gst_amount from Stock where Bno = " + billNumber + " and BillType='GST' ");
            }
            dataGridVendorBill.ItemsSource = cc.dt.AsDataView();
            dataGridVendorBill.Visibility = System.Windows.Visibility.Visible;
        }

        private void ShowDetails()
        {
            cc.OpenConnection();
            if (MainWindow.userName == "admin")
            {
                if(MainWindow.isgst==1)
                {
                    cc.CreateView("CREATE VIEW PurchaseBill AS Select Distinct Stock.Bno,Stock.gtotal,FORMAT(TransactionDetails.PurchaseDate,'dd-MMM-yyyy') as PDate,TransactionDetails.CreditAmount,TransactionDetails.PaymentType,TransactionDetails.GstAmount from Stock,TransactionDetails where Stock.Bno=TransactionDetails.Bno and Stock.BillType='GST' and TransactionDetails.BillType='GST'");
                }
                else
                {
                    cc.CreateView("CREATE VIEW PurchaseBill AS Select Distinct Stock.Bno,Stock.gtotal,FORMAT(TransactionDetails.PurchaseDate,'dd-MMM-yyyy') as PDate,TransactionDetails.CreditAmount,TransactionDetails.PaymentType,TransactionDetails.GstAmount from Stock,TransactionDetails where Stock.Bno=TransactionDetails.Bno and Stock.BillType='NON_GST' and TransactionDetails.BillType='NON_GST'");
                }
            }
            else
            {
                cc.CreateView("CREATE VIEW PurchaseBill AS Select Distinct Stock.Bno,Stock.gtotal,FORMAT(TransactionDetails.PurchaseDate,'dd-MMM-yyyy') as PDate,TransactionDetails.CreditAmount,TransactionDetails.PaymentType,TransactionDetails.GstAmount from Stock,TransactionDetails where Stock.Bno=TransactionDetails.Bno and Stock.BillType='GST' and TransactionDetails.BillType='GST'");
            }
            cc.DataReader("select * from PurchaseBill where Bno = " + billNumber + "");
            while (cc.reader.Read())
            {
                txt_Bill_Number.Text = cc.reader["Bno"].ToString();
                txt_Sale_Date.Text = cc.reader["PDate"].ToString();
                txt_Grant_Total.Text = "₹ " + cc.reader["gtotal"].ToString();
                textBox_Credit.Text = cc.reader["CreditAmount"].ToString();
                txt_Payment_Type.Text = cc.reader["PaymentType"].ToString();
            }
            cc.CloseReader();
            cc.DropView("DROP VIEW PurchaseBill");
            cc.CloseConnection();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void dataGridVendorBill_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
