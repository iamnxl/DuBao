using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace ConsoleApp1
{
    class Success
    {
        public string success { get; set; }
        public List<DataModel> data { get; set; }
        public string message { get; set; }
    }
}
