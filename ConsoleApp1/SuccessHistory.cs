using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace ConsoleApp1
{
    class SuccessHistory
    {
        public string success { get; set; }
        public List<DataHistory> data { get; set; }
        public string message { get; set; }
        public void sort()
        {
            data.Sort(new SortData());
        }
    }
}
