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
    public class TestFisdDq
    {
        string _connection =
            "Host = localhost; Username =postgres; Password =roman; Database =MIR";

        [TestMethod]
        public void TestInsertQuote()
        {
            //вставим несколько значений за разные даты 
            var tempConnection = DataBaseLink.Fabricate.CreateConnection(_connection, ConnectionType.Npgsql);
            DataBaseLink.DbLink dbLink = new DbLink(tempConnection);
            //создать финансовый инструмент 
            fin_instrument fin_instrument = new fin_instrument("roman", 13, "bushuev");
            if (FinInstrument.FindId(dbLink, fin_instrument.ident) == null)
                FinInstrument.Insert(dbLink, fin_instrument);

            var result = FinInstrument.FindId(dbLink, fin_instrument.ident);

            ffd ffd = new ffd()
            {
                ds_id = 17,
                fi_id = result.fi_id,
                fif_id = 3
            };

            if (FFD.Find(dbLink, result.fi_id, ffd.ds_id, ffd.fif_id) == null)
                FFD.Insert(dbLink, ffd);
            var ffdResult = FFD.Find(dbLink, result.fi_id, ffd.ds_id, ffd.fif_id);

            fisd_dq fisd_dq = new fisd_dq()
            {
                dat = DateTime.Today.AddDays(-1),
                fisd_id = ffdResult.fisd_id,
                val = 10.0m
            };

            if (FisdDq.FindId(dbLink, fisd_dq.fisd_id, fisd_dq.dat) == null)
                FisdDq.Insert(dbLink, fisd_dq);

            fisd_dq = new fisd_dq()
            {
                dat = DateTime.Today,
                fisd_id = ffdResult.fisd_id,
                val = 11.0m
            };

            if (FisdDq.FindId(dbLink, fisd_dq.fisd_id, fisd_dq.dat) == null)
                FisdDq.Insert(dbLink, fisd_dq);
        }

        [TestMethod]
        public void TestUpdateQuote()
        {
            var tempConnection = DataBaseLink.Fabricate.CreateConnection(_connection, ConnectionType.Npgsql);
            DataBaseLink.DbLink dbLink = new DbLink(tempConnection);

            fisd_dq fisd_dq = new fisd_dq()
            {
                fisd_id = 20281,
                dat = new DateTime(2018, 05, 16),
                val = 77.0m
            };

            if (FisdDq.FindId(dbLink, fisd_dq.fisd_id, fisd_dq.dat) != null)
            {
                FisdDq.Update(dbLink, fisd_dq);
            }
        }

        [TestMethod]
        public void TestRemoveQuote()
        {
            //вставим несколько значений за разные даты 
            var tempConnection = DataBaseLink.Fabricate.CreateConnection(_connection, ConnectionType.Npgsql);
            DataBaseLink.DbLink dbLink = new DbLink(tempConnection);
            //создать финансовый инструмент 
            fin_instrument fin_instrument = new fin_instrument("roman", 13, "bushuev");
            if (FinInstrument.FindId(dbLink, fin_instrument.ident) == null)
                FinInstrument.Insert(dbLink, fin_instrument);

            var result = FinInstrument.FindId(dbLink, fin_instrument.ident);

            ffd ffd = new ffd()
            {
                ds_id = 17,
                fi_id = result.fi_id,
                fif_id = 3
            };

            if (FFD.Find(dbLink, result.fi_id, ffd.ds_id, ffd.fif_id) == null)
                FFD.Insert(dbLink, ffd);
            var ffdResult = FFD.Find(dbLink, result.fi_id, ffd.ds_id, ffd.fif_id);

            fisd_dq fisd_dq = new fisd_dq()
            {
                dat = DateTime.Today.AddDays(-1),
                fisd_id = ffdResult.fisd_id,
                val = 10.0m
            };

            if (FisdDq.FindId(dbLink, fisd_dq.fisd_id, fisd_dq.dat) == null)
            {
                FisdDq.Insert(dbLink, fisd_dq);
                FisdDq.Remove(dbLink, fisd_dq.fisd_id, fisd_dq.dat);
            }
            else
            {
                FisdDq.Remove(dbLink, fisd_dq.fisd_id, fisd_dq.dat);
            }

            fisd_dq = new fisd_dq()
            {
                dat = DateTime.Today,
                fisd_id = ffdResult.fisd_id,
                val = 11.0m
            };

            if (FisdDq.FindId(dbLink, fisd_dq.fisd_id, fisd_dq.dat) == null)
            {
                FisdDq.Insert(dbLink, fisd_dq);
                FisdDq.Remove(dbLink, fisd_dq.fisd_id, fisd_dq.dat);
            }
            else
            {
                FisdDq.Remove(dbLink, fisd_dq.fisd_id, fisd_dq.dat);
            }
        }
    }
}
