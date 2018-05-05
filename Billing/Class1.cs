using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;
using System.Data.OleDb;
using System.Collections.ObjectModel;
using System.Windows;

namespace Billing
{
    class Class1 : SellPage
    {
        public class BillTable
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Rate { get; set; }
            public int Quantity { get; set; }
            public double Amount { get; set; }
            public string GSTPercent { get; set; }
            public double AddedGST { get; set; }
        }
        public class DataAccess
        {
            ConnectionClass cc = new ConnectionClass();
            /*OleDbConnection connection;
            OleDbCommand command;*/
            string stkleftc, gstamount;
            public static string blno;
            public DataAccess()
            {
                cc.InitialiseConnection();
                /*connection = new OleDbConnection();
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["Connection"].ToString();*/
            }
            public ObservableCollection<BillTable> GetBill()
            {
                ObservableCollection<BillTable> Bill = new ObservableCollection<BillTable>();
                cc.OpenConnection();
                cc.DataReader("select * from BillTable");
                /*connection.Open();
                command = new OleDbCommand();
                command.Connection = connection;
                command.CommandText = "select * from BillTable";
                OleDbDataReader reader = command.ExecuteReader();*/
                while (cc.reader.Read())
                {
                    Bill.Add(new BillTable()
                    {
                        Id = Convert.ToInt32(cc.reader["Id"]),
                        Name = cc.reader["Name"].ToString(),
                        Rate = Convert.ToInt32(cc.reader["Rate"]),
                        Quantity = Convert.ToInt32(cc.reader["Quantity"]),
                        Amount = Convert.ToDouble(cc.reader["Amount"]),
                        GSTPercent = cc.reader["GSTPercent"].ToString(),
                        AddedGST = Convert.ToDouble(cc.reader["AddedGST"]),
                    });
                }
                cc.CloseReader();
                //reader.Close();
                cc.CloseConnection();
                //connection.Close();
                return Bill;
            }

            public void InsertBill(BillTable objbill)
            {
                //OleDbCommand command = new OleDbCommand();
                cc.OpenConnection();
                /*if (connection.State != ConnectionState.Open)
                    connection.Open();*/
                cc.DataReader("select quantity,gst_amount from Stock where itemid= " + objbill.Id + " ");
                /*command.Connection = connection;
                string qr = "select quantity,gst_amount from Stock where itemid= " + objbill.Id + " ";
                command.CommandText = qr;
                OleDbDataReader reader = command.ExecuteReader();*/
                while (cc.reader.Read())
                {
                    stkleftc = cc.reader["quantity"].ToString();
                    gstamount = cc.reader["gst_amount"].ToString();
                }
                cc.CloseReader();
                //reader.Close();
                cc.CloseConnection();
                //connection.Close();
                int stk = Convert.ToInt32(stkleftc);
                int qty = stk - objbill.Quantity;
                if (stk <= 0)
                {
                    MessageBox.Show("Out Of Stock!.");
                }
                else if (qty < 0)
                {
                    MessageBox.Show("Sorry...! Only " + stkleftc + " Items Left ");
                }
                else
                {
                    //connection.Open();
                    cc.OpenConnection();
                    cc.DoBillInsert("update BillTable set Id=@Id,Name=@Name,Rate=@Rate,Quantity=@Quantity,Amount=@Rate*@Quantity+@AddedGST*@Quantity,AddedGST=@AddedGST*@Quantity where Id=@Id", "@Id", objbill.Id, "@Name", objbill.Name, "@Rate", objbill.Rate, "@Quantity", objbill.Quantity, "@Amount", gstamount, "@GSTPercent", objbill.GSTPercent, "@AddedGST", objbill.AddedGST);
                    /*command = new OleDbCommand();
                    command.Connection = connection;
                    command.CommandText = "update BillTable set Id=@Id,Name=@Name,Rate=@Rate,Quantity=@Quantity,Amount=@Rate*@Quantity+@AddedGST*@Quantity,AddedGST=@AddedGST*@Quantity where Id=@Id";
                    command.Parameters.AddWithValue("@Id", objbill.Id);
                    command.Parameters.AddWithValue("@Name", objbill.Name);
                    command.Parameters.AddWithValue("@Rate", objbill.Rate);
                    command.Parameters.AddWithValue("@Quantity", objbill.Quantity);
                    command.Parameters.AddWithValue("@Amount", gstamount);
                    command.Parameters.AddWithValue("@GSTPercent", objbill.GSTPercent);
                    command.Parameters.AddWithValue("@AddedGST", objbill.AddedGST);
                    command.ExecuteNonQuery();
                    connection.Close();*/
                    cc.CloseConnection();
                }

            }
        }
    }
}
