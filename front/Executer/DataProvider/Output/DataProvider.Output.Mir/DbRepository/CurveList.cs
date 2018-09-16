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
    public static class CurveList
    {
        //find 
        public static List<curve_list> FindId(DbLink dbLink, int cur_id)
        {
            string query =
                string.Format(@"select * from curve_list t
                    where t.cur_id = {0}", cur_id);
            var result = dbLink.GetConnection().Query<curve_list>(query);
            return result.ToList();
        }

        //insert 
        public static void Insert(DbLink dbLink, curve_list curveList)
        {
            string query =
                @"insert into curve_list(cur_id, fi_id, ident, ord_id, term, dat)
                    values(@cur_id, @fi_id, @ident, @ord_id, @term, @dat)";

            dbLink.GetConnection().Execute(query, curveList);
        }

        //remove
        public static void Remove(DbLink dbLink, int cur_id, int fi_id, DateTime dat)
        {
            string query =
                string.Format(@"delete from curve_list t where t.cur_id = {0} and fi_id = {1} and dat = {2};",
                cur_id, fi_id, dat.ToString("dd.MM.yyyy"));
            dbLink.GetConnection().Execute(query);
        }
    }
}
