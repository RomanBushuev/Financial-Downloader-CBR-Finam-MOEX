using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataProvider.Output.Mir.DbObject;
using DataProvider.Output.Mir.DbRepository;
using DataBaseLink;
using System.Data;

namespace Test.DataProvider.Output.Mir
{
    [TestClass]
    public class TestCurves
    {
        string _connection =
            "Host = localhost; Username =postgres; Password =roman; Database =MIR";

        [TestMethod]
        public void TestInsertCurve()
        {
            var tempConnection = DataBaseLink.Fabricate.CreateConnection(_connection, DataBaseLink.ConnectionType.Npgsql);
            DataBaseLink.DbLink dbLink = new DbLink(tempConnection);
            curve curve = new curve() { description = "debt_zcc_rub", ident = "debt_zcc_rub", title = "debt_zcc_rub" };
            if(Curves.FindId(dbLink, curve.ident) == null)
                Curves.Insert(dbLink, curve);
        }

        [TestMethod]
        public void TestRemoveCurve()
        {
            var tempConnection = DataBaseLink.Fabricate.CreateConnection(_connection, DataBaseLink.ConnectionType.Npgsql);
            DataBaseLink.DbLink dbLink = new DbLink(tempConnection);
            curve curve = new curve() { description = "debt_zcc_rub", ident = "debt_zcc_rub", title = "debt_zcc_rub" };
            if (Curves.FindId(dbLink, curve.ident) == null)
            {
                Curves.Insert(dbLink, curve);
                var result = Curves.FindId(dbLink, curve.ident);
                Curves.Remove(dbLink, result.cur_id);
            }
            else
            {
                var result = Curves.FindId(dbLink, curve.ident);
                Curves.Remove(dbLink, result.cur_id);
            }
        }
    }
}
