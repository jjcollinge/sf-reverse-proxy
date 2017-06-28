using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReverseProxyService
{
    public class ProxyItem
    {
        public string Key { get; set; }
        public string Extension { get; set; }
        public int TrafficPercentage { get; set; }
    }
}
