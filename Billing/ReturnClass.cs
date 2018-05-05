using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Billing
{
    class ReturnClass
    {
        public class ReturnGridTable
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
            public DataAccess()
            {
                cc.InitialiseConnection();
                /*connection = new OleDbConnection();
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["Connection"].ToString();*/
            }
            public ObservableCollection<ReturnGridTable> GetReturnBill()
            {
                ObservableCollection<ReturnGridTable> ReturnBill = new ObservableCollection<ReturnGridTable>();
                cc.OpenConnection();
                cc.DataReader("select * from ReturnGridTable");
                while (cc.reader.Read())
                {
                    ReturnBill.Add(new ReturnGridTable()
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
                cc.CloseConnection();
                return ReturnBill;
            }

            public void InsertReturnBill(ReturnGridTable objbill)
            {
                cc.OpenConnection();
                cc.DataReader("select gst_amount from Stock where itemid= " + objbill.Id + " ");
                while (cc.reader.Read())
                {
                    gstamount = cc.reader["gst_amount"].ToString();
                }
                cc.CloseReader();
                cc.CloseConnection();
                cc.OpenConnection();
                cc.DoBillInsert("update ReturnGridTable set Id=@Id,Name=@Name,Rate=@Rate,Quantity=@Quantity,Amount=@Rate*@Quantity+@AddedGST*@Quantity,AddedGST=@AddedGST*@Quantity where Id=@Id", "@Id", objbill.Id, "@Name", objbill.Name, "@Rate", objbill.Rate, "@Quantity", objbill.Quantity, "@Amount", gstamount, "@GSTPercent", objbill.GSTPercent, "@AddedGST", objbill.AddedGST);
                cc.CloseConnection();


            }
        }
    }
}
