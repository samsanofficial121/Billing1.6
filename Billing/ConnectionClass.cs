using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Billing
{
    class ConnectionClass
    {
        SqlConnection connection;
        public SqlDataReader reader;
        public SqlDataAdapter da;
        public DataTable dt;
        public DataSet ds;

        public ConnectionClass()
        {
            connection = new SqlConnection();
            connection.ConnectionString = ConfigurationManager.ConnectionStrings["Connection"].ToString();
        }

        public void InitialiseConnection()
        {
            connection = new SqlConnection();
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
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = Query;
            command.ExecuteNonQuery();
        }

        public void CreateView(string Query)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = Query;
            string returnValue = (string)command.ExecuteScalar();
        }

        public void DropView(string Query)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = Query;
            string returnValue = (string)command.ExecuteScalar();
        }

        public void DataReader(string Query)
        {
            SqlCommand command = new SqlCommand();
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
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = Query;
            da = new SqlDataAdapter(command);
            dt = new DataTable();
            da.Fill(dt);
            
        }

        public int BillPreview(string Query, string bn, string tbn, int blxt)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = Query;
            command.Parameters.AddWithValue(bn, tbn);
            blxt = (int)command.ExecuteScalar();
            return blxt;
        }

        public int BillPreview1(string Query, int blxt)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = Query;
            blxt = (int)command.ExecuteScalar();
            return blxt;
        }

        public void TransactionDetails(string Query, string Id, int id, string Name, string name, string Bno, int bno, string NetAmnt, string netamnt, string Payment, string payment,string Phone, string phone, string Place, string place, string Date, string formattedDate, string Credit, string credit, string Debit, string debit, string CashPaid, string cashPaid, string PaymentType, string paymentType, string GstAmount, string gstAmount, string BillType, string billtype)
        {
            SqlCommand command = new SqlCommand();
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
            command.Parameters.AddWithValue(GstAmount, gstAmount);
            command.Parameters.AddWithValue(BillType, billtype);
            command.ExecuteNonQuery();
        }

        public void CustomerTransactions(string Query, string BillNo, int billNo, string CustName, string custName, string CustPhone, string custPhone, string NetAmnt, double netAmnt, string CashPaid, double cashPaid, string Date, string date, string Credit, string credit, string Debit, string debit, string Billtype, string billtype, string Discount, string discount)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = Query;
            command.Parameters.AddWithValue(BillNo, billNo);
            command.Parameters.AddWithValue(CustName, custName);
            command.Parameters.AddWithValue(CustPhone, custPhone);
            command.Parameters.AddWithValue(NetAmnt, netAmnt);
            command.Parameters.AddWithValue(CashPaid, cashPaid);
            command.Parameters.AddWithValue(Date, date);
            command.Parameters.AddWithValue(Credit, credit);
            command.Parameters.AddWithValue(Debit, debit);
            command.Parameters.AddWithValue(Billtype, billtype);
            command.Parameters.AddWithValue(Discount, discount);
            command.ExecuteNonQuery();
        }

        public int UserAuthentication(string Query, string Username, string username, string Password, string password, int blxt)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = Query;
            command.Parameters.AddWithValue(Username, username);
            command.Parameters.AddWithValue(Password, password);
            blxt = (int)command.ExecuteScalar();
            return blxt;
        }
    }
}
