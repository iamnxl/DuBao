using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class DataHistory
    {
        public string time { get; set; }
        public double capacity { get; set; }
        public double ghi { get; set; }
        public double paneltemp { get; set; }
        public double envtemp { get; set; }

    }
    class SortData : Comparer<DataHistory>
    {
        public override int Compare(DataHistory x, DataHistory y)
        {
            if (x.ghi > y.ghi)
            {
                return 1;
            }
            else if (x.ghi == y.ghi)
            {
                if (x.envtemp > y.envtemp)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }
    }
}
