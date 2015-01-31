using System.Data.Entity;

namespace DataAccessTest.EntityFramework
{
    public class DataContext : DbContext
    {
        public DataContext()
            : base("ConnectionString")
        {

        }

        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new CustomerMap());
        }
    }
}
