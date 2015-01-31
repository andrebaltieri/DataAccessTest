using DataAccessTest.EntityFramework;
using System;
using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.Entity.Core.Objects;
using Dapper;
using NHibernate;
using NHibernate.Linq;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;

namespace DataAccessTest
{
    class Program
    {
        static void Main(string[] args)
        {
            AdoDataAccess();
            Console.WriteLine();
            DapperDataAccess();
            Console.WriteLine();
            EfDataAccess();
            Console.WriteLine();
            NhDataAccess();

            Console.WriteLine();
            Console.WriteLine("Teste finalizado");
            Console.ReadKey();
        }

        public static void AdoDataAccess()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            List<Customer> customers = new List<Customer>();

            var startDate = DateTime.Now;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT [id],[first_name],[last_name],[email],[country] FROM [dbo].[Customer]", conn);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    customers.Add(new Customer
                    {
                        Id = (int)dr[0],
                        FirstName = (string)dr[1],
                        LastName = (string)dr[2],
                        Email = (string)dr[3],
                        Country = (string)dr[4]
                    });
                }

                conn.Close();
            }
            var endDate = DateTime.Now;

            Console.WriteLine("ADO Puro");
            Console.WriteLine("Objetos Gerados: {0}", customers.Count);
            Console.WriteLine("Início: {0}", startDate);
            Console.WriteLine("Término: {0}", endDate);
            Console.WriteLine("Tempo Total: {0}", (endDate - startDate));

            customers = null;
        }

        public static void DapperDataAccess()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            IEnumerable<Customer> customers = new List<Customer>();

            var startDate = DateTime.Now;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                customers = conn.Query<Customer>("SELECT [id] as Id,[first_name] as FirstName, [last_name] as LastName, [email] as Email, [country] as Country FROM [dbo].[Customer]");
                conn.Close();
            }
            var endDate = DateTime.Now;

            Console.WriteLine("Dapper");
            Console.WriteLine("Objetos Gerados: {0}", customers.ToList().Count);
            Console.WriteLine("Início: {0}", startDate);
            Console.WriteLine("Término: {0}", endDate);
            Console.WriteLine("Tempo Total: {0}", (endDate - startDate));

            customers = null;
        }

        public static void EfDataAccess()
        {
            DataContext db = new DataContext();

            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            List<Customer> customers = new List<Customer>();

            var startDate = DateTime.Now;
            //db.Database.Log = Console.Write;
            customers = db.Customers.ToList();
            var endDate = DateTime.Now;

            Console.WriteLine("Entity Framework");
            Console.WriteLine("Objetos Gerados: {0}", customers.Count);
            Console.WriteLine("Início: {0}", startDate);
            Console.WriteLine("Término: {0}", endDate);
            Console.WriteLine("Tempo Total: {0}", (endDate - startDate));

            customers = null;
            db.Dispose();
        }

        public static void NhDataAccess()
        {
            var sessionFactory = CreateSessionFactory();
            
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            List<Customer> customers = new List<Customer>();

            var startDate = DateTime.Now;
            using (var session = sessionFactory.OpenSession())
            {
                customers = session.Query<Customer>().ToList();
            }
            var endDate = DateTime.Now;

            Console.WriteLine("NHibernate");
            Console.WriteLine("Objetos Gerados: {0}", customers.Count);
            Console.WriteLine("Início: {0}", startDate);
            Console.WriteLine("Término: {0}", endDate);
            Console.WriteLine("Tempo Total: {0}", (endDate - startDate));

            customers = null;
            sessionFactory.Dispose();
        }

        private static ISessionFactory CreateSessionFactory()
        {
            return Fluently.Configure()
                .Database(
                    MsSqlConfiguration.MsSql2008
                    .ConnectionString(c => c.FromConnectionStringWithKey("ConnectionString"))
                )
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Program>())
                .BuildSessionFactory();
        }
    }
}
