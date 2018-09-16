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
    public class TestFisdNum
    {
        string _connection =
            "Host = localhost; Username =postgres; Password =roman; Database =MIR";

        [TestMethod]
        public void TestInsertFisdNum()
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

            fisd_num fisd_num = new fisd_num()
            {
                fisd_id = ffdResult.fisd_id,
                dat_from = new DateTime(2018, 03, 24),
                val = 320.0m
            };


            if(FisdNum.FindId(dbLink, ffdResult.fisd_id, fisd_num.dat_from) == null)
            {
                FisdNum.Insert(dbLink, fisd_num);
                var one = FisdNum.FindId(dbLink, fisd_num.fisd_id, fisd_num.dat_from);
            }
            else
            {
                FisdNum.Insert(dbLink, fisd_num);
                var one = FisdNum.FindId(dbLink, fisd_num.fisd_id, fisd_num.dat_from);
            }
        }

        [TestMethod]
        public void TestRemoveFisdNum()
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

            fisd_num fisd_num = new fisd_num()
            {
                fisd_id = ffdResult.fisd_id,
                dat_from = new DateTime(2018, 03, 24),
                val = 320.0m
            };


            if (FisdNum.FindId(dbLink, ffdResult.fisd_id, fisd_num.dat_from) == null)
            {
                FisdNum.Insert(dbLink, fisd_num);
                var one = FisdNum.FindId(dbLink, fisd_num.fisd_id, fisd_num.dat_from);
                FisdNum.Remove(dbLink, fisd_num.fisd_id, fisd_num.dat_from);
            }
            else
            {
                FisdNum.Insert(dbLink, fisd_num);
                var one = FisdNum.FindId(dbLink, fisd_num.fisd_id, fisd_num.dat_from);
                FisdNum.Remove(dbLink, fisd_num.fisd_id, one.dat_from);
            }
        }
    }
}
