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
    public static class FinInstrument
    {
        //find
        public static fin_instrument FindId(DbLink dbLink, string ident)
        {
            string query =
               string.Format(@"select * from fin_instrument t
                    where t.ident = upper('{0}');", ident);
            var result = dbLink.GetConnection().QueryFirstOrDefault<fin_instrument>(query);

            return result;
        }
        //insert 
        public static void Insert(DbLink dbLink, fin_instrument finInstrument)
        {
            string query =
                @"insert into fin_instrument(fi_id,ident,ft_id,title)
                    values(nextval('mir_sequence'), @ident, @ft_id, @title);";
            dbLink.GetConnection().Execute(query, finInstrument);
        }
        //remove
        public static void Remove(DbLink dbLink, int fin_id)
        {
            string query = 
                string.Format(@"delete from fin_instrument t where t.fi_id = {0}",
                fin_id);
            dbLink.GetConnection().Execute(query);
        }

        public static void Remove(DbLink dbLink, string ident)
        {
            string query = "delete from fin_instrument t where t.ident = @ident";
            dbLink.GetConnection().Execute(query, ident);
        }
    }
}
