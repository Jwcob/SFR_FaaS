using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;


namespace SFR_FaaS
{
    public class TransactionDataContext : DbContext
    {

        public TransactionDataContext() : base(Environment.GetEnvironmentVariable("mycs")) { }
        public DbSet<Transaction> Transactions
        {
            get;
            set;
        }
    }
}
