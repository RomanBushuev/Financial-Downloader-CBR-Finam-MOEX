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
    public class TestFisdItem
    {
        string _connection =
            "Host = localhost; Username =postgres; Password =roman; Database =MIR";

        [TestMethod]
        public void TestInsertFisdItem()
        {
            var tempConnection = DataBaseLink.Fabricate.CreateConnection(_connection,
                ConnectionType.Npgsql);
            DbLink dbLink = new DbLink(tempConnection);

            //создать финансовый инструмент
            fin_instrument fin_instrument = new fin_instrument("roman", 13, "bushuev");
            if (FinInstrument.FindId(dbLink, fin_instrument.ident) == null)
                FinInstrument.Insert(dbLink, fin_instrument);

            var resultFI = FinInstrument.FindId(dbLink, fin_instrument.ident);

            //ffd
            ffd ffd = new ffd()
            {
                ds_id = 17,
                fi_id = resultFI.fi_id,
                fif_id = 8,
            };

            if (FFD.Find(dbLink, resultFI.fi_id, ffd.ds_id, ffd.fif_id) == null)
                FFD.Insert(dbLink, ffd);
            var ffdResult = FFD.Find(dbLink, resultFI.fi_id, ffd.ds_id, ffd.fif_id);

            //dict_item
            dict_item dict_item = new dict_item()
            {
                fif_id = 8,
                key_v = "RUB",
            };

            var resultDictItem = DictItem.FindId(dbLink, dict_item.key_v, dict_item.fif_id);

            //fisd_item
            fisd_item fisd_item = new fisd_item()
            {
                dat_from = new DateTime(2017, 08, 05),
                fisd_id = ffdResult.fisd_id,
                val = resultDictItem.key_v
            };

            if(FisdItem.FindId(dbLink, fisd_item.fisd_id, fisd_item.dat_from) == null)
            {
                FisdItem.Insert(dbLink, fisd_item);
                var one = FisdItem.FindId(dbLink, fisd_item.fisd_id, fisd_item.dat_from);
            }
            else
            {
                FisdItem.Insert(dbLink, fisd_item);
                var one = FisdItem.FindId(dbLink, fisd_item.fisd_id, fisd_item.dat_from);
            }
        }

        [TestMethod]
        public void TestRemoveFisdItem()
        {
            var tempConnection = DataBaseLink.Fabricate.CreateConnection(_connection, ConnectionType.Npgsql);
            DbLink dbLink = new DbLink(tempConnection);

            //создать финансовый инструмент
            fin_instrument fin_instrument = new fin_instrument("roman", 13, "bushuev");
            if (FinInstrument.FindId(dbLink, fin_instrument.ident) == null)
                FinInstrument.Insert(dbLink, fin_instrument);

            var resultFI = FinInstrument.FindId(dbLink, fin_instrument.ident);

            //ffd
            ffd ffd = new ffd()
            {
                ds_id = 17,
                fi_id = resultFI.fi_id,
                fif_id = 8,
            };

            if (FFD.Find(dbLink, resultFI.fi_id, ffd.ds_id, ffd.fif_id) == null)
                FFD.Insert(dbLink, ffd);
            var ffdResult = FFD.Find(dbLink, resultFI.fi_id, ffd.ds_id, ffd.fif_id);

            //dict_item
            dict_item dict_item = new dict_item()
            {
                fif_id = 8,
                key_v = "RUB",
            };

            var resultDictItem = DictItem.FindId(dbLink, dict_item.key_v, dict_item.fif_id);

            //fisd_item
            fisd_item fisd_item = new fisd_item()
            {
                dat_from = new DateTime(2017, 08, 05),
                fisd_id = ffdResult.fisd_id,
                val = resultDictItem.key_v
            };

            if (FisdItem.FindId(dbLink, fisd_item.fisd_id, fisd_item.dat_from) == null)
            {
                FisdItem.Insert(dbLink, fisd_item);
                var one = FisdItem.FindId(dbLink, fisd_item.fisd_id, fisd_item.dat_from);
                FisdItem.Remove(dbLink, one.fisd_id, one.dat_from);
            }
            else
            {
                var one = FisdItem.FindId(dbLink, fisd_item.fisd_id, fisd_item.dat_from);
                FisdItem.Remove(dbLink, one.fisd_id, one.dat_from);
            }
        }
    }
}
