using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Data.OleDb;
using System.Configuration;

namespace Billing
{
    class ConnectionClass
    {
        OleDbConnection connection;
        public OleDbDataReader reader;
        public OleDbDataAdapter da;
        public DataTable dt;
        public DataSet ds;

        public ConnectionClass()
        {
            connection = new OleDbConnection();
            connection.ConnectionString = ConfigurationManager.ConnectionStrings["Connection"].ToString();
        }

    public void InitialiseConnection()
        {
            connection = new OleDbConnection();
            connection.ConnectionString = ConfigurationManager.ConnectionStrings["Connection"].ToString();
        }

        public  void OpenConnection()
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();
        }

        public void CloseConnection()
        {
            connection.Close();
        }

        public void ExecuteQuery(string Query)
        {
            OleDbCommand command = new OleDbCommand();
            command.Connection = connection;
            command.CommandText = Query;
            command.ExecuteNonQuery();
        }

        public void CreateView(string Query)
        {
            OleDbCommand command = new OleDbCommand();
            command.Connection = connection;
            command.CommandText = Query;
            string returnValue = (string)command.ExecuteScalar();
        }

        public void DropView(string Query)
        {
            OleDbCommand command = new OleDbCommand();
            command.Connection = connection;
            command.CommandText = Query;
            string returnValue = (string)command.ExecuteScalar();
        }

        public void DataReader(string Query)
        {
            OleDbCommand command = new OleDbCommand();
            command.Connection = connection;
            command.CommandText = Query;
            reader = command.ExecuteReader();
        }

        public void CloseReader()
        {
            reader.Close();
        }

        public void DataGridDisplay(string Query)
        {
            OleDbCommand command = new OleDbCommand();
            command.Connection = connection;
            command.CommandText = Query;
            da = new OleDbDataAdapter(command);
            dt = new DataTable();
            da.Fill(dt);
            
        }

        public void UpdateBillTable(string Query, string Qty, int qt)
        {
            OleDbCommand command = new OleDbCommand();
            command.Connection = connection;
            command.CommandText = Query;
            command.Parameters.AddWithValue(Qty, qt);
            command.ExecuteNonQuery();
        }

        public void InsertBill(string Query, string Qty, int qt)
        {
            OleDbCommand command = new OleDbCommand();
            command.Connection = connection;
            command.CommandText = Query;
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue(Qty, qt);
            reader = command.ExecuteReader();
        }
        
        public void SaveBill(string Query, string BillNumber, string billNumber, string NetGst, double netGst, string NetAmount, string netAmount, string SaleDate, string saleDate, string CustomerName, string customerName, string CustomerPhone, string customerPhone, string Credit, string credit, string Debit, string debit, string SReturn, string sreturn, string BillAmnt, string billamnt, string CashPaid, string cashpaid, string Particulars, string particulars)
        {
            OleDbCommand command = new OleDbCommand();
            command.Connection = connection;
            command.CommandText = Query;
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue(BillNumber, billNumber);
            command.Parameters.AddWithValue(NetGst, netGst);
            command.Parameters.AddWithValue(NetAmount, netAmount);
            command.Parameters.AddWithValue(SaleDate, saleDate);
            command.Parameters.AddWithValue(CustomerName, customerName);
            command.Parameters.AddWithValue(CustomerPhone, customerPhone);
            command.Parameters.AddWithValue(Credit, credit);
            command.Parameters.AddWithValue(Debit, debit);
            command.Parameters.AddWithValue(SReturn, sreturn);
            command.Parameters.AddWithValue(BillAmnt, billamnt);
            command.Parameters.AddWithValue(CashPaid, cashpaid);
            command.Parameters.AddWithValue(Particulars, particulars);
            reader = command.ExecuteReader();
        }

        public void UpdateBill(string Query, string TxtBillNumber, int txtBillNumber, string BillNumber, string billNumber, string NetGst, double netGst, string NetAmount, string netAmount, string SaleDate, string saleDate, string CustomerName, string customerName, string CustomerPhone, string customerPhone, string Credit, string credit, string Debit, string debit)
        {
            OleDbCommand command = new OleDbCommand();
            command.Connection = connection;
            command.CommandText = Query;
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue(TxtBillNumber, txtBillNumber);
            command.Parameters.AddWithValue(BillNumber, billNumber);
            command.Parameters.AddWithValue(NetGst, netGst);
            command.Parameters.AddWithValue(NetAmount, netAmount);
            command.Parameters.AddWithValue(SaleDate, saleDate);
            command.Parameters.AddWithValue(CustomerName, customerName);
            command.Parameters.AddWithValue(CustomerPhone, customerPhone);
            command.Parameters.AddWithValue(Credit, credit);
            command.Parameters.AddWithValue(Debit, debit);
            command.ExecuteNonQuery();
        }

        public int BillPreview(string Query, string bn, string tbn, int blxt)
        {
            OleDbCommand command = new OleDbCommand();
            command.Connection = connection;
            command.CommandText = Query;
            command.Parameters.AddWithValue(bn, tbn);
            blxt = (int)command.ExecuteScalar();
            return blxt;
        }

        public void BillInsert(string Query, string Id, int id, string Name, string name, string Rate, int rate, string Qty, int qt, string Amount, int amount)
        {
            OleDbCommand command = new OleDbCommand();
            command.Connection = connection;
            command.CommandText = Query;
            command.Parameters.AddWithValue(Id, id);
            command.Parameters.AddWithValue(Name, name);
            command.Parameters.AddWithValue(Rate, rate);
            command.Parameters.AddWithValue(Qty, qt);
            command.Parameters.AddWithValue(Amount, amount);
            command.ExecuteNonQuery();
        }

        public void DoPurchase(string Query, string Bno, int bno, string NetAmnt, int netamnt)
        {
            OleDbCommand command = new OleDbCommand();
            command.Connection = connection;
            command.CommandText = Query;
            command.Parameters.AddWithValue(Bno, bno);
            command.Parameters.AddWithValue(NetAmnt, netamnt);
            command.ExecuteNonQuery();
        }

        public void TransactionDetails(string Query, string Id, int id, string Name, string name, string Bno, int bno, string NetAmnt, int netamnt, string Payment, int payment,string Phone, string phone, string Place, string place, string Date, string formattedDate, string Credit, string credit, string Debit, string debit, string CashPaid, int cashPaid, string PaymentType, string paymentType)
        {
            OleDbCommand command = new OleDbCommand();
            command.Connection = connection;
            command.CommandText = Query;
            command.Parameters.AddWithValue(Id, id);
            command.Parameters.AddWithValue(Name, name);
            command.Parameters.AddWithValue(Bno, bno);
            command.Parameters.AddWithValue(NetAmnt, netamnt);
            command.Parameters.AddWithValue(Payment, payment);
            command.Parameters.AddWithValue(Phone, phone);
            command.Parameters.AddWithValue(Place, place);
            command.Parameters.AddWithValue(Date, formattedDate);
            command.Parameters.AddWithValue(Credit, credit);
            command.Parameters.AddWithValue(Debit, debit);
            command.Parameters.AddWithValue(CashPaid, cashPaid);
            command.Parameters.AddWithValue(PaymentType, paymentType);
            command.ExecuteNonQuery();
        }

        public void CustomerTransactions(string Query, string BillNo, int billNo, string CustName, string custName, string CustPhone, string custPhone, string NetAmnt, double netAmnt, string CashPaid, double cashPaid)
        {
            OleDbCommand command = new OleDbCommand();
            command.Connection = connection;
            command.CommandText = Query;
            command.Parameters.AddWithValue(BillNo, billNo);
            command.Parameters.AddWithValue(CustName, custName);
            command.Parameters.AddWithValue(CustPhone, custPhone);
            command.Parameters.AddWithValue(NetAmnt, netAmnt);
            command.Parameters.AddWithValue(CashPaid, cashPaid);
            command.ExecuteNonQuery();
        }

        public void DoInsertVendor(string Query, string VendorId, int vendorId, string VendorName, string vendorName, string VendorPhone, string vendorPhone, string VendorPlace, string vendorPlace)
        {
            OleDbCommand command = new OleDbCommand();
            command.Connection = connection;
            command.CommandText = Query;
            command.Parameters.AddWithValue(VendorId, vendorId);
            command.Parameters.AddWithValue(VendorName, vendorName);
            command.Parameters.AddWithValue(VendorPhone, vendorPhone);
            command.Parameters.AddWithValue(VendorPlace, vendorPlace);
            command.ExecuteNonQuery();
        }

        public void DoBillInsert(string Query, string Id, int id, string Name, string name, string Rate, int rate, string Qty, int qt, string Amount, string amount, string GstPercent, string gstPercent, string AddedGst, double addedGst)
        {
            OleDbCommand command = new OleDbCommand();
            command.Connection = connection;
            command.CommandText = Query;
            command.Parameters.AddWithValue(Id, id);
            command.Parameters.AddWithValue(Name, name);
            command.Parameters.AddWithValue(Rate, rate);
            command.Parameters.AddWithValue(Qty, qt);
            command.Parameters.AddWithValue(Amount, amount);
            command.Parameters.AddWithValue(GstPercent, gstPercent);
            command.Parameters.AddWithValue(AddedGst, addedGst);
            command.ExecuteNonQuery();
        }
        public void UpdateGST(string Query, string GST, string gst)
        {
            OleDbCommand command = new OleDbCommand();
            command.Connection = connection;
            command.CommandText = Query;
            command.Parameters.AddWithValue(GST, gst);
            command.ExecuteNonQuery();
        }

        public int UserAuthentication(string Query, string Username, string username, string Password, string password, int blxt)
        {
            OleDbCommand command = new OleDbCommand();
            command.Connection = connection;
            command.CommandText = Query;
            command.Parameters.AddWithValue(Username, username);
            command.Parameters.AddWithValue(Password, password);
            blxt = (int)command.ExecuteScalar();
            return blxt;
        }
    }
}
