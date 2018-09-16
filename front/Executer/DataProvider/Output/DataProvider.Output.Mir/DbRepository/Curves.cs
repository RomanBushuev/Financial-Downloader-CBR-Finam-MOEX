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
    public static class Curves
    {
        //find 
        public static curve FindId(DbLink dbLink, string ident)
        {
            string query =
                string.Format(@"select * from curves t
                    where t.ident = upper('{0}');", ident);
            var result = dbLink.GetConnection().QueryFirstOrDefault<curve>(query);
            
            return result;
        }

        //insert
        public static void Insert(DbLink dbLink, curve curve)
        {
            string query =
                @"insert into curves(cur_id, ident, description, title)
                    values(nextval('mir_sequence'), @ident, @description, @title);";
            dbLink.GetConnection().Execute(query, curve);
        }

        //remove
        public static void Remove(DbLink dbLink, int cur_id)
        {
            string query =
                string.Format(@"delete from curves t where t.cur_id ={0}",
                cur_id);
            dbLink.GetConnection().Execute(query);
        }
    }
}
