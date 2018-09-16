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
    public class TestFinInstrument
    {
        string _connection =
                "Host = localhost; Username =postgres; Password =roman; Database =MIR";
        [TestMethod]
        public void TestInsertFinInstrument()
        {
            var tempConnection = DataBaseLink.Fabricate.CreateConnection(_connection, ConnectionType.Npgsql);
            DataBaseLink.DbLink dbLink = new DbLink(tempConnection);
            fin_instrument fin_instrument = new fin_instrument("roman", 13, "bushuev");
            if (FinInstrument.FindId(dbLink, fin_instrument.ident) == null)
                FinInstrument.Insert(dbLink, fin_instrument);
        }

        [TestMethod]
        public void TestRemoveFinInstrument()
        {
            var tempConnection = DataBaseLink.Fabricate.CreateConnection(_connection, ConnectionType.Npgsql);
            DataBaseLink.DbLink dbLink = new DbLink(tempConnection);
            fin_instrument fin_instrument = new fin_instrument("roman", 13, "bushuev");
            if (FinInstrument.FindId(dbLink, fin_instrument.ident) == null)
            {
                FinInstrument.Insert(dbLink, fin_instrument);
                var t = FinInstrument.FindId(dbLink, fin_instrument.ident);
                Console.WriteLine(t.fi_id);
                FinInstrument.Remove(dbLink, t.fi_id);
            }
            else
            {
                var t = FinInstrument.FindId(dbLink, fin_instrument.ident);
                Console.WriteLine(t.fi_id);
                FinInstrument.Remove(dbLink, t.fi_id);
            }
        }
    }
}
