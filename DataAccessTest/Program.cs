using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Dapper;
using DataAccessTest.EntityFramework;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Linq;

namespace DataAccessTest
{
    class Program
    {
        static void Main(string[] args)
        {
            AdoDataAccess();
            Console.WriteLine();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            DapperDataAccess();
            Console.WriteLine();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            EfDataAccess();
            Console.WriteLine();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            EfFastDataAccess();
            Console.WriteLine();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            EfMoreFastDataAccess();
            Console.WriteLine();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            NhDataAccess();

            Console.WriteLine();
            Console.WriteLine("Teste finalizado");
            Console.ReadKey();
        }

        public static void AdoDataAccess()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            List<Customer> customers = new List<Customer>();

            var stopwatch = Stopwatch.StartNew();
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
            stopwatch.Stop();

            Console.WriteLine("ADO Puro");
            Console.WriteLine("Objetos Gerados: {0}", customers.Count);
            Console.WriteLine("Tempo Total: {0}", stopwatch.Elapsed);

            customers = null;
        }

        public static void DapperDataAccess()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            IEnumerable<Customer> customers = new List<Customer>();

            var stopwatch = Stopwatch.StartNew();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                customers = conn.Query<Customer>("SELECT [id] as Id,[first_name] as FirstName, [last_name] as LastName, [email] as Email, [country] as Country FROM [dbo].[Customer]");
                conn.Close();
            }
            stopwatch.Stop();

            Console.WriteLine("Dapper");
            Console.WriteLine("Objetos Gerados: {0}", customers.ToList().Count);
            Console.WriteLine("Tempo Total: {0}", stopwatch.Elapsed);

            customers = null;
        }

        public static void EfDataAccess()
        {
            List<Customer> customers;
            Stopwatch stopwatch;
            var db = new DataContext();
            using (db)
            {
                stopwatch = Stopwatch.StartNew();
                customers = db.Customers.ToList();
                stopwatch.Stop();
            }
            db = null;

            Console.WriteLine("Entity Framework");
            Console.WriteLine("Objetos Gerados: {0}", customers.Count);
            Console.WriteLine("Tempo Total: {0}", stopwatch.Elapsed);

            customers = null;
        }
        public static void EfFastDataAccess()
        {
            List<Customer> customers;
            Stopwatch stopwatch;
            var db = new DataContext();
            using (db)
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                db.Configuration.EnsureTransactionsForFunctionsAndCommands = false;
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;
                db.Configuration.UseDatabaseNullSemantics = false;
                stopwatch = Stopwatch.StartNew();
                customers = db.Customers.AsNoTracking().ToList();
                stopwatch.Stop();
            }
            db = null;

            Console.WriteLine("Entity Framework Rápido");
            Console.WriteLine("Objetos Gerados: {0}", customers.Count);
            Console.WriteLine("Tempo Total: {0}", stopwatch.Elapsed);

            customers = null;
        }
        public static void EfMoreFastDataAccess()
        {
            List<Customer> customers;
            Stopwatch stopwatch;
            var db = new DataContext();
            using (db)
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                db.Configuration.EnsureTransactionsForFunctionsAndCommands = false;
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;
                db.Configuration.UseDatabaseNullSemantics = false;
                db.Customers.AsNoTracking().FirstOrDefault();
                stopwatch = Stopwatch.StartNew();
                customers = db.Customers.AsNoTracking().ToList();
                stopwatch.Stop();
            }
            db = null;

            Console.WriteLine("Entity Framework Mais Rápido");
            Console.WriteLine("Objetos Gerados: {0}", customers.Count);
            Console.WriteLine("Tempo Total: {0}", stopwatch.Elapsed);

            customers = null;
        }

        public static void NhDataAccess()
        {
            var sessionFactory = CreateSessionFactory();

            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            List<Customer> customers = new List<Customer>();

            var stopwatch = Stopwatch.StartNew();
            using (var session = sessionFactory.OpenSession())
            {
                customers = session.Query<Customer>().ToList();
            }
            stopwatch.Stop();

            Console.WriteLine("NHibernate");
            Console.WriteLine("Objetos Gerados: {0}", customers.Count);
            Console.WriteLine("Tempo Total: {0}", stopwatch.Elapsed);

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
