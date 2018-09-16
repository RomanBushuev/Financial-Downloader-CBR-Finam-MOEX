using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataBaseLink;
using System.Configuration;
using Dapper;
using Npgsql;

namespace TestDbProvider
{
    [TestClass]
    public class TestDbConnection
    {
        [TestMethod]
        public void TestConnection()
        {
            var connection = ConfigurationManager.AppSettings.Get("postgre");
            var postgre = Fabricate.CreateConnection(connection, ConnectionType.Npgsql);
            DbLink dbLink = new DbLink(postgre);
            var t = dbLink.GetConnection();
            
        }
    }
}
