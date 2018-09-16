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
    public static class FinField
    {
        //find
        public static fin_field FindId(DbLink dbLink, string ident)
        {
            string query =
                string.Format(@"select * from fin_field t
                    where t.ident = upper('{0}');", ident);
            var result = dbLink.GetConnection().QueryFirstOrDefault<fin_field>(query);
            return result;
        }

        //insert 
        public static void Insert(DbLink dbLink, fin_field fin_filed)
        {
            string query =
                @"insert into fin_field(fif_id, ident, title, description)
                    values(nextval('mir_sequence'), @ident, @title, @description);";
            dbLink.GetConnection().Execute(query, fin_filed);        
        }

        //remove
        public static void Remove(DbLink dbLink, int fif_id)
        {
            string query =
                string.Format(@"delete from fin_field t where t.fif_id = {0}", fif_id);
            dbLink.GetConnection().Execute(query);
        }
    }
}
