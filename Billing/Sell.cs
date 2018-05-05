using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;
using System.Data.OleDb;
using System.Collections.ObjectModel;

namespace Billing
{
    class Sell
    {
        public int[] Id = new int[100];
        public string[] Name = new string[100];
        public int[] Rate = new int[100];
        public int[] Quantity = new int[100];
        public int[] Amount = new int[100];

        public class DataAccess
        {
            OleDbConnection connection;
            OleDbCommand command;
            public DataAccess()
            {
                connection = new OleDbConnection();
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["Connection"].ToString();
            }

            /*public void GetData()
            {
                SellPage sp = new SellPage();
                OleDbCommand command = new OleDbCommand();
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                command.Connection = connection;
                int id = Convert.ToInt32(sp.iid);
                string qr = "select * from BillTable where itemid= " + id + " ";
                command.CommandText = qr;
                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    for (int j = 0; j < 100; j++)
                    {
                        Sell sell = new Sell();
                        sell.Id[j] = Convert.ToInt32(reader["Id"]);
                        sell.Name[j] = Convert.ToString(reader["Name"]);
                        sell.Rate[j] = Convert.ToInt32(reader["Rate"]);
                        sell.Quantity[j] = Convert.ToInt32(reader["Quantity"]);
                        sell.Amount[j] = Convert.ToInt32(reader["Amount"]);

                    }
                }
            }*/

        }
    }
}
