using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFR_FaaS
{
    public class CustomerDataContext : DbContext
    {
        public CustomerDataContext() : base(Environment.GetEnvironmentVariable("mycs")) { }
        public DbSet<Customer> Customers
        {
            get;
            set;
        }
    }
}
