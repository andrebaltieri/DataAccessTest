using FluentNHibernate.Mapping;

namespace DataAccessTest.NHibernate
{
    public class CustomerMap : ClassMap<Customer>
    {
        public CustomerMap()
        {
            Id(x => x.Id)
                .GeneratedBy.Identity();

            Map(x => x.FirstName).Column("first_name");
            Map(x => x.LastName).Column("last_name");
            Map(x => x.Email).Column("email");
            Map(x => x.Country).Column("country");
        }
    }
}
