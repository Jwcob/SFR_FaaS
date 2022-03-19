using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFR_FaaS
{
    public class Report
    {
        public Customer Customer { get; set; }
        public List<Transaction> Transactions { get; set; }
    }
}
