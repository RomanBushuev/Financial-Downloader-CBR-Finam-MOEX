using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core.Mir.BaseTypes;
using Core.Mir.Enumerations;
using Core.Mir.Interfaces;
using DataProvider.Output.Mir.DbObject;
using DataProvider.Output.Mir.DbRepository;
using DataBaseLink;
using System.Data;

namespace Test.DataProvider.Output.Mir
{
    [TestClass]
    public class TestDataSource
    {
        string _connection =
            "Host = localhost; Username =postgres; Password =roman; Database =MIR";

        [TestMethod]
        public void TestInsertDataSource()
        {
            var tempConnection = DataBaseLink.Fabricate.CreateConnection(_connection, ConnectionType.Npgsql);
            DataBaseLink.DbLink dbLink = new DbLink(tempConnection);
            data_source dataSource = new data_source();
            dataSource.ident = "CALCULATED";
            dataSource.description = "valid data";

            if (DataSource.FindId(dbLink, dataSource.ident) == null)
                DataSource.Insert(dbLink, dataSource);
        }

        [TestMethod]
        public void TestRemoveDataSource()
        {
            var tempConnection = DataBaseLink.Fabricate.CreateConnection(_connection, ConnectionType.Npgsql);
            DataBaseLink.DbLink dbLink = new DbLink(tempConnection);
            data_source dataSource = new data_source();
            dataSource.ident = "CALCULATED";
            dataSource.description = "valid data";

            if(DataSource.FindId(dbLink, dataSource.ident) == null)
            {
                DataSource.Insert(dbLink, dataSource);
                var t = DataSource.FindId(dbLink, dataSource.ident);
                DataSource.Remove(dbLink, t.ds_id);
            }
            else
            {
                var t = DataSource.FindId(dbLink, dataSource.ident);
                DataSource.Remove(dbLink, t.ds_id);
            }
        }
    }
}
