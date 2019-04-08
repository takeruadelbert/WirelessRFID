using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WirelessRFID.Class
{
    class EPC_data : IComparable
    {
        public string epc;
        public int count;
        public int devNo;
        public byte antNo;
        int IComparable.CompareTo(object obj)
        {
            EPC_data temp = (EPC_data)obj;
            {
                return string.Compare(this.epc, temp.epc);
            }
        }
    }
}
