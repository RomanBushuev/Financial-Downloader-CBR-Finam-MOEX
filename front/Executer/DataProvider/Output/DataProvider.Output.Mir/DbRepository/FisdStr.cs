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
    public static class FisdStr
    {
        //find 
        public static fisd_str FindId(DbLink dbLink, int fisd_id, DateTime dateTime)
        {
            string query =
                string.Format(@"select * from fisd_str t 
                    where t.fisd_id = {0} and t.dat_from <= to_date('{1}', 'dd.mm.yyyy')",
                         fisd_id, dateTime.ToString("dd.MM.yyyy"));
            var result = dbLink.GetConnection().QueryFirstOrDefault<fisd_str>(query);
            return result;
        }

        //insert 
        public static void Insert(DbLink dbLink, fisd_str fisd_str)
        {
            //при первой вставке дату указываем = 01.01.1900
            if (FindId(dbLink, fisd_str.fisd_id, fisd_str.dat_from) == null)
            {
                fisd_str.dat_from = new DateTime(1900, 01, 01);
            }
            else
            {
                var result = FindId(dbLink, fisd_str.fisd_id, fisd_str.dat_from);
                if (fisd_str.val == result.val)
                    return;
            }

            string query =
                @"insert into fisd_str(val, dat_from, fisd_id)
                    values(@val, @dat_from, @fisd_id)";
            dbLink.GetConnection().Execute(query, fisd_str);
        }

        //remove 
        public static void Remove(DbLink dbLink, int fisd_id, DateTime dateTime)
        {
            string query =
                string.Format(@"delete from fisd_str t where t.fisd_id = {0} and t.dat_from = to_date('{0}', 'dd.mm.yyyy')",
                fisd_id, dateTime.ToString("dd.MM.yyyy"));
            dbLink.GetConnection().Execute(query);
        }
    }
}
