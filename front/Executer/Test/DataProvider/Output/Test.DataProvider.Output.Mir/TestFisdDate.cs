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
    public class TestFisdDate
    {
        string _connection =
            "Host = localhost; Username =postgres; Password =roman; Database =MIR";


        [TestMethod]
        public void TestInsertFisdDate()
        {
            var tempConnection = DataBaseLink.Fabricate.CreateConnection(_connection, ConnectionType.Npgsql);
            DataBaseLink.DbLink dbLink = new DbLink(tempConnection);
            //создать финансовый инструмент 
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

            //создать fisd_date
            fisd_date fisdDate = new fisd_date()
            {
                fisd_id = ffdResult.fisd_id,
                dat_from = new DateTime(2018, 03, 24),
                val = new DateTime(2018, 03, 24)
            };

            if(FisdDate.FindId(dbLink, fisdDate.fisd_id, fisdDate.dat_from) == null)
            {
                FisdDate.Insert(dbLink, fisdDate);
                var one = FisdDate.FindId(dbLink, fisdDate.fisd_id, fisdDate.dat_from);
            }
            else
            {
                FisdDate.Insert(dbLink, fisdDate);
                var one = FisdDate.FindId(dbLink, fisdDate.fisd_id, fisdDate.dat_from);
            }
        }

        [TestMethod]
        public void TestRemoveFisdDate()
        {
            var tempConnection = DataBaseLink.Fabricate.CreateConnection(_connection, ConnectionType.Npgsql);
            DataBaseLink.DbLink dbLink = new DbLink(tempConnection);
            //создать финансовый инструмент 
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

            //создать fisd_date
            fisd_date fisdDate = new fisd_date()
            {
                fisd_id = ffdResult.fisd_id,
                dat_from = new DateTime(2018, 03, 25),
                val = new DateTime(2018, 03, 25)
            };

            if (FisdDate.FindId(dbLink, fisdDate.fisd_id, fisdDate.dat_from) == null)
            {
                FisdDate.Insert(dbLink, fisdDate);
                var one = FisdDate.FindId(dbLink, fisdDate.fisd_id, fisdDate.dat_from);
                FisdDate.Remove(dbLink, fisdDate.fisd_id, one.dat_from);
            }
            else
            {
                var one = FisdDate.FindId(dbLink, fisdDate.fisd_id, fisdDate.dat_from);
                FisdDate.Remove(dbLink, fisdDate.fisd_id, one.dat_from);
            }
        }
    }
}
