using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataProvider.Output.Mir.DbObject;
using DataProvider.Output.Mir.DbRepository;
using DataBaseLink;
using System.Data;

namespace Test.DataProvider.Output.Mir
{
    [TestClass]
    public class TestDictItem
    {
        string _connection =
            "Host = localhost; Username =postgres; Password =roman; Database =MIR";


        [TestMethod]
        public void TestInsertDictItem()
        {
            var tempConnection = DataBaseLink.Fabricate.CreateConnection(_connection, DataBaseLink.ConnectionType.Npgsql);
            DataBaseLink.DbLink dbLink = new DbLink(tempConnection);
            dict_item dict_item = new dict_item()
            {
                fif_id = 8,
                key_v = "RUB_roman",
                val = "RUB_roman",
            };

            if(DictItem.FindId(dbLink, dict_item.key_v, dict_item.fif_id) == null)
            {
                DictItem.Insert(dbLink, dict_item);
            }
        }
        
        [TestMethod]
        public void TestRemoveDictItem()
        {
            var tempConnection = DataBaseLink.Fabricate.CreateConnection(_connection, DataBaseLink.ConnectionType.Npgsql);
            DataBaseLink.DbLink dbLink = new DbLink(tempConnection);
            dict_item dict_item = new dict_item()
            {
                fif_id = 8,
                key_v = "RUB_roman",
                val = "RUB_roman",
            };

            if (DictItem.FindId(dbLink, dict_item.key_v, dict_item.fif_id) == null)
            {
                DictItem.Insert(dbLink, dict_item);
                var result = DictItem.FindId(dbLink, dict_item.key_v, dict_item.fif_id);
                DictItem.Remove(dbLink, result.fif_id, result.key_v);
            }
            else
            {
                var result = DictItem.FindId(dbLink, dict_item.key_v, dict_item.fif_id);
                DictItem.Remove(dbLink, result.fif_id, result.key_v);
            }
        }
    }
}
