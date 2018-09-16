using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataProvider.Output.Mir.DbObject;
using DataProvider.Output.Mir.DbRepository;
using DataBaseLink;
using System.Data;

namespace Test.DataProvider.Output.Mir
{
    [TestClass]
    public class TestFinType
    {
        string _connection =
               "Host = localhost; Username =postgres; Password =roman; Database =MIR";
        
        [TestMethod]
        public void TestInsertFinType()
        {
            var tempConnection = DataBaseLink.Fabricate.CreateConnection(_connection, DataBaseLink.ConnectionType.Npgsql);
            DataBaseLink.DbLink dbLink = new DbLink(tempConnection);
            fin_type fin_type = new fin_type("FX_RATE", "Валютный курс");
            if (FinType.FindId(dbLink, fin_type.ident) == null)
                FinType.Insert(dbLink, fin_type);
        }

        [TestMethod]
        public void TestRemoveFinType()
        {
            var tempConnection = DataBaseLink.Fabricate.CreateConnection(_connection, DataBaseLink.ConnectionType.Npgsql);
            DataBaseLink.DbLink dbLink = new DbLink(tempConnection);
            fin_type fin_type = new fin_type("BUSHEUV", "BUSHUEV");
            if (FinType.FindId(dbLink, fin_type.ident) == null)
            {
                FinType.Insert(dbLink, fin_type);
                var t = FinType.FindId(dbLink, fin_type.ident);
                Console.WriteLine(t.ft_id);
                FinType.Remove(dbLink, t.ft_id);
            }
            else
            {
                var t = FinType.FindId(dbLink, fin_type.ident);
                Console.WriteLine(t.ft_id);
                FinType.Remove(dbLink, t.ft_id);
            }

        }
    }
}
