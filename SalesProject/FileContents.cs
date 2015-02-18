using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesProject
{
    public class FileContents
    {
        public string Manager { get; set; }
        public List<OrderRecord> Orders = new List<OrderRecord>();
    }

    public struct OrderRecord
    {
        public DateTime Date { get; set; }
        public string Client { get; set; }
        public string Product { get; set; }
        public int Sum { get; set; }
    }
}
