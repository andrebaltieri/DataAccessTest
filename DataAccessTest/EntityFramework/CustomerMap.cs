using System.Data.Entity.ModelConfiguration;

namespace DataAccessTest.EntityFramework
{
    public class CustomerMap : EntityTypeConfiguration<Customer>
    {
        public CustomerMap()
        {
            ToTable("Customer");

            HasKey(x => x.Id);
            Property(x => x.Id).HasColumnName("id");
            Property(x => x.FirstName).HasColumnName("first_name");
            Property(x => x.LastName).HasColumnName("last_name");
            Property(x => x.Email).HasColumnName("email");
            Property(x => x.Country).HasColumnName("country");
        }
    }
}
