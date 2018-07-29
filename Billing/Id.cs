using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing
{
    class Id
    {
        public List<String> list;

        public Id()
        {
            list = new List<string>();
        }

        public void StoreIId(int itemID)
        {
            list.Add(Convert.ToString(itemID));
        }
        public void clear()
        {
            list.Clear();
        }
    }
}
