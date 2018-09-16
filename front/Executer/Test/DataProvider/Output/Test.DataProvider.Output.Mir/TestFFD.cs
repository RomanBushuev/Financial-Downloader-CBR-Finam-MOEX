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
    public class TestFFD
    {
        string _connection =
            "Host = localhost; Username =postgres; Password =roman; Database =MIR";


        [TestMethod]
        public void TestInsertFFD()
        {
            var tempConnection = DataBaseLink.Fabricate.CreateConnection(_connection, ConnectionType.Npgsql);
            DataBaseLink.DbLink dbLink = new DbLink(tempConnection);
            //инструмент
            fin_instrument fin_instrument = new fin_instrument("roman", 13, "bushuev");
            if (FinInstrument.FindId(dbLink, fin_instrument.ident) == null)
                FinInstrument.Insert(dbLink, fin_instrument);

            var result = FinInstrument.FindId(dbLink, fin_instrument.ident);
            //fisd_id
            ffd ffd = new ffd()
            {
                ds_id = 17,
                fi_id = result.fi_id,
                fif_id = 11,
            };

            if(FFD.Find(dbLink, ffd.fi_id, ffd.ds_id, ffd.fif_id) == null)
                FFD.Insert(dbLink, ffd);
        }

        [TestMethod]
        public void TestRemoveFFD()
        {
            var tempConnection = DataBaseLink.Fabricate.CreateConnection(_connection, ConnectionType.Npgsql);
            DataBaseLink.DbLink dbLink = new DbLink(tempConnection);
            //инструмент
            fin_instrument fin_instrument = new fin_instrument("roman", 13, "bushuev");
            if (FinInstrument.FindId(dbLink, fin_instrument.ident) == null)
                FinInstrument.Insert(dbLink, fin_instrument);

            var result = FinInstrument.FindId(dbLink, fin_instrument.ident);
            //fisd_id
            ffd ffd = new ffd()
            {
                ds_id = 17,
                fi_id = result.fi_id,
                fif_id = 11,
            };

            if (FFD.Find(dbLink, ffd.fi_id, ffd.ds_id, ffd.fif_id) == null)
            {
                FFD.Insert(dbLink, ffd);
                var ffdResult = FFD.Find(dbLink, result.fi_id, ffd.ds_id, ffd.fif_id);
                FFD.Remove(dbLink, ffdResult);
            }
            else
            {
                var ffdResult = FFD.Find(dbLink, result.fi_id, ffd.ds_id, ffd.fif_id);
                FFD.Remove(dbLink, ffdResult);
            }

        }

        [TestMethod]
        public void TestRemoveFFDByInstrument()
        {
            var tempConnection = DataBaseLink.Fabricate.CreateConnection(_connection, ConnectionType.Npgsql);
            DataBaseLink.DbLink dbLink = new DbLink(tempConnection);
            //инструмент
            fin_instrument fin_instrument = new fin_instrument("roman", 13, "bushuev");
            if (FinInstrument.FindId(dbLink, fin_instrument.ident) == null)
                FinInstrument.Insert(dbLink, fin_instrument);

            var result = FinInstrument.FindId(dbLink, fin_instrument.ident);
            //fisd_id
            ffd ffd = new ffd()
            {
                ds_id = 17,
                fi_id = result.fi_id,
                fif_id = 11,
            };

            FinInstrument.Remove(dbLink, result.fi_id);
        }
    }
}
