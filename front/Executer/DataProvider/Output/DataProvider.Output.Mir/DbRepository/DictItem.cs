using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataBaseLink;
using Core.Mir.BaseTypes;
using Core.Mir.Enumerations;
using DataProvider.Output.Mir.DbObject;
namespace DataProvider.Output.Mir.DbRepository
{
    public static class DictItem
    {
        //find
        public static dict_item FindId(DbLink dbLink, string ident, int fif_id)
        {
            string query =
                string.Format(@"select * from dict_item t
                    where (t.key_v = upper('{0}') or t.val = upper('{0}')) and t.fif_id = {1}",
                    ident, fif_id);
            var result = dbLink.GetConnection().QueryFirstOrDefault<dict_item>(query);
            return result;
        }

        //insert
        public static void Insert(DbLink dbLink, dict_item dict_item)
        {
            string value = string.Format("select count(*) from dict_item t where t.fif_id = {0}",
                dict_item.fif_id);

            int ord_id = dbLink.GetConnection().ExecuteScalar<int>(value);
            dict_item.ord_id = ord_id + 1;

            string query =
                @"insert into dict_item(fif_id, key_v, val, ord_id)
                    values(@fif_id, @key_v, @val, @ord_id)";
            dbLink.GetConnection().Execute(query, dict_item);
        }

        //remove
        public static void Remove(DbLink dbLink, int fif_id, string key_v)
        {
            string query =
                string.Format(@"delete from dict_item t where t.fif_id = {0} and t.key_v = upper('{1}')", 
                fif_id, key_v);
            dbLink.GetConnection().Execute(query);
        }
    }
}
