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
    public class TestFisdStr
    {
        string _connection =
            "Host = localhost; Username =postgres; Password =roman; Database =MIR";

        [TestMethod]
        public void TestInsertFisdStr()
        {
            var tempConnection = DataBaseLink.Fabricate.CreateConnection(_connection, ConnectionType.Npgsql);
            DataBaseLink.DbLink dbLink = new DbLink(tempConnection);

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
            if (FFD.Find(dbLink, result.fi_id, ffd.ds_id, ffd.fif_id) == null)
                FFD.Insert(dbLink, ffd);
            var ffdResult = FFD.Find(dbLink, result.fi_id, ffd.ds_id, ffd.fif_id);

            fisd_str fisd_str = new fisd_str()
            {
                fisd_id = ffdResult.fisd_id,
                dat_from = new DateTime(2018, 03, 24),
                val = "roman"
            };

            if(FisdStr.FindId(dbLink, fisd_str.fisd_id, fisd_str.dat_from) == null)
            {
                FisdStr.Insert(dbLink, fisd_str);
                var one = FisdStr.FindId(dbLink, fisd_str.fisd_id, fisd_str.dat_from);
            }
            else
            {
                FisdStr.Insert(dbLink, fisd_str);
                var one = FisdStr.FindId(dbLink, fisd_str.fisd_id, fisd_str.dat_from);
            }
        }

        [TestMethod]
        public void TestRemoveFisdStr()
        {
            var tempConnection = DataBaseLink.Fabricate.CreateConnection(_connection, ConnectionType.Npgsql);
            DataBaseLink.DbLink dbLink = new DbLink(tempConnection);

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
            if (FFD.Find(dbLink, result.fi_id, ffd.ds_id, ffd.fif_id) == null)
                FFD.Insert(dbLink, ffd);
            var ffdResult = FFD.Find(dbLink, result.fi_id, ffd.ds_id, ffd.fif_id);

            fisd_str fisd_str = new fisd_str()
            {
                fisd_id = ffdResult.fisd_id,
                dat_from = new DateTime(2018, 03, 24),
                val = "roman"
            };

            if (FisdStr.FindId(dbLink, fisd_str.fisd_id, fisd_str.dat_from) == null)
            {
                FisdStr.Insert(dbLink, fisd_str);
                var one = FisdStr.FindId(dbLink, fisd_str.fisd_id, fisd_str.dat_from);
                FisdStr.Remove(dbLink, one.fisd_id, one.dat_from);
            }
            else
            {
                FisdStr.Insert(dbLink, fisd_str);
                var one = FisdStr.FindId(dbLink, fisd_str.fisd_id, fisd_str.dat_from);
                FisdStr.Remove(dbLink, one.fisd_id, one.dat_from);
            }
        }
    }
}
