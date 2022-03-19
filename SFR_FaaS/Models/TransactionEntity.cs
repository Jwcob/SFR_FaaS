using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFR_FaaS
{
    public class Transaction
    {
        public int Id
        {
            get;
            set;
        }
        public string Amount
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }
        public int Creditor
        {
            get;
            set;
        }
        public int Debtor
        {
            get;
            set;
        }
        public DateTime ExecutionDate
        {
            get;
            set;
        }
    }
}
