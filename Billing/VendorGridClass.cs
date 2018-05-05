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
    class VendorGridClass : VendorList
    {
        public class VendorTable
        {
            public int Vid { get; set; }
            public string Vname { get; set; }
            public string Vphone { get; set; }
            public string Vplace { get; set; }
        }
        public class DataAccess
        {
            ConnectionClass cc = new ConnectionClass();
            public DataAccess()
            {
                cc.InitialiseConnection();
            }
            public ObservableCollection<VendorTable> GetVendor()
            {
                ObservableCollection<VendorTable> Vendor = new ObservableCollection<VendorTable>();
                cc.OpenConnection();
                cc.DataReader("select * from VendorDetails");
                while (cc.reader.Read())
                {
                    Vendor.Add(new VendorTable()
                    {
                        Vid = Convert.ToInt32(cc.reader["Vid"]),
                        Vname = cc.reader["Vname"].ToString(),
                        Vphone = cc.reader["Vphone"].ToString(),
                        Vplace = cc.reader["Vplace"].ToString(),
                    });
                }
                cc.CloseReader();
                cc.CloseConnection();
                return Vendor;
            }

            public void InsertVendor(VendorTable objvendor)
            {
                cc.OpenConnection();
                cc.DoInsertVendor("Insert into VendorDetails Values(@Vid,@Vname,@Vphone,@Vplace)", "@Vid", objvendor.Vid, "@Vname", objvendor.Vname, "@Vphone", objvendor.Vphone, "@Vplace", objvendor.Vplace);
                cc.CloseConnection();
            }

        }
    }
}
