using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dapper;
using Npgsql;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using DataBaseLink;

namespace UnitTestProject1
{
    [TestClass]
    public class TestConnections
    {
        [TestMethod]
        public void UsingWithoutDapper()
        {
            string connection =
                "Host = localhost; Username =postgres; Password =roman; Database =MIR";

            using(NpgsqlConnection npgsConnection = new NpgsqlConnection(connection))
            {
                npgsConnection.Open();
                string command = " select * from data_source";
                using (NpgsqlCommand npgsCommand = new NpgsqlCommand(command, npgsConnection))
                {
                    using(NpgsqlDataReader dataReader = npgsCommand.ExecuteReader())
                    {
                        while(dataReader.Read())
                        {
                            Console.WriteLine(dataReader["ident"].ToString());
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void UsingWithDapper()
        {
            string connection =
                "Host = localhost; Username =postgres; Password =roman; Database =MIR";
            using (IDbConnection db = new NpgsqlConnection(connection))
            {
                db.Open();
                DataSource datasource = new DataSource()
                {
                    ds_id = 777,
                    description = "roman",
                    ident = "bushuev",
                    ord_id = 3
                };
                DataSource result = db.Query<DataSource>("select * from data_source").FirstOrDefault();
                Console.WriteLine(result.ident);
                //npgsConnection.Open();
                //string command = " select * from data_source";
                //using (NpgsqlCommand npgsCommand = new NpgsqlCommand(command, npgsConnection))
                //{
                //    using (NpgsqlDataReader dataReader = npgsCommand.ExecuteReader())
                //    {
                //        while (dataReader.Read())
                //        {
                //            Console.WriteLine(dataReader["ident"].ToString());
                //        }
                //    }
                //}
            }
        }

        [TestMethod]
        public void UsingWithDapperAndFabricate()
        {
            string connection = "Host = localhost; Username =postgres; Password =roman; Database =MIR";
            var dbConnection = Fabricate.CreateConnection(connection, ConnectionType.Npgsql);
            DbLink dbLink = new DbLink(dbConnection);
            var t = dbLink.GetConnection().Query<DataSource>("select * from data_source").First();
            Console.WriteLine(t.ident);
        }

        public class DataSource
        {
              public int ds_id{get;set;}
              public string ident {get;set;}
              public string description {get;set;}
              public int ord_id {get;set;}
        }
    }
}
