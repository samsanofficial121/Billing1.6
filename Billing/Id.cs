using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing
{
    class Id
    {
        //public static List<string> BillList = new List<string>();
        public string[] iti = new string[100];
        public string[] quanty = new string[100];
        public  int i = 0;
        public List<String> list;

        public Id()
        {
            list = new List<string>();
        }

        public void StoreBill()
        {

        }
        public void StoreIId(int itemID)
        {
            // iti[i] =Convert.ToString(itemID);
            // i++;
            list.Add(Convert.ToString(itemID));
        }
        public void clear()
        {
            i = 0;
           // for(int ind = 0; ind < iti.Length; ind++)
            {
            ///    iti[ind] = null;
            }
          //  iti = new string[100];
            list.Clear();

        }
    }
}
