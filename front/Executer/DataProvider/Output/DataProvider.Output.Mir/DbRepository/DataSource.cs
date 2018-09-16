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
    public static class DataSource
    {
        //find 
        public static data_source FindId(DbLink dbLink, string ident)
        {
            string query =
                string.Format(@"select * from data_source t
                    where t.ident = upper('{0}')", ident);
            var result = dbLink.GetConnection().QueryFirstOrDefault<data_source>(query);

            return result;
        }

        //insert 
        public static void Insert(DbLink dbLink, data_source dataSource)
        {
            string queryAmount = "select count(*) from data_source";
            int ord_id = dbLink.GetConnection().QueryFirstOrDefault<int>(queryAmount);
            dataSource.ord_id = ord_id +1;
            string query =
                string.Format(@"insert into data_source(ds_id, ident, description, ord_id)
                    values(nextval('mir_sequence'), @ident, @description, @ord_id)");

            dbLink.GetConnection().Execute(query, dataSource);
        }

        public static void Remove(DbLink dbLink, int ds_id)
        {
            string query =
                string.Format("delete from data_source t where t.ds_id = {0}", ds_id);
            dbLink.GetConnection().Execute(query);
        }

        public static void Remove(DbLink dbLink, string ident)
        {
            string query = 
                string.Format("delete from data_source t where t.ident = {0}",ident);
            dbLink.GetConnection().Execute(query);
        }
    }
}
