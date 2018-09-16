using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataProvider.Output.Mir.DbObject;
using DataProvider.Output.Mir.DbRepository;
using DataBaseLink;
using System.Data;

namespace Test.DataProvider.Output.Mir
{
    [TestClass]
    public class TestFinField
    {
        string _connection =
            "Host = localhost; Username =postgres; Password =roman; Database =MIR";

        [TestMethod]
        public void TestInsertFinField()
        {
            var tempConnection = DataBaseLink.Fabricate.CreateConnection(_connection, DataBaseLink.ConnectionType.Npgsql);
            DataBaseLink.DbLink dbLink = new DbLink(tempConnection);
            fin_field fin_field = new fin_field()
            {
                ident = "Start_Date",
                title = "Дата размещения",
                description = "Дата размещения",
            };

            if(FinField.FindId(dbLink, fin_field.ident) == null)
            {
                FinField.Insert(dbLink, fin_field);
            }
        }

        [TestMethod]
        public void TestRemoveFinField()
        {
            var tempConnection = DataBaseLink.Fabricate.CreateConnection(_connection, DataBaseLink.ConnectionType.Npgsql);
            DataBaseLink.DbLink dbLink = new DbLink(tempConnection);
            fin_field fin_field = new fin_field()
            {
                ident = "ROMAN",
                title = "ROMAN",
                description = "roman"
            };

            if (FinField.FindId(dbLink, fin_field.ident) == null)
            {
                FinField.Insert(dbLink, fin_field);
                var result = FinField.FindId(dbLink, fin_field.ident);
                FinField.Remove(dbLink, result.fif_id);
            }
            else
            {
                var result = FinField.FindId(dbLink, fin_field.ident);
                FinField.Remove(dbLink, result.fif_id);
            }
        }
    }
}
