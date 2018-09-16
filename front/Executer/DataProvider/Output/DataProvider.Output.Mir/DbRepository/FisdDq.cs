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
    public static class FisdDq
    {
        public static fisd_dq FindId(DbLink dbLink, int fisd_id, DateTime dat)
        {
            string query =
                string.Format(@"select * from fisd_dq t
                    where t.fisd_id = {0} and t.dat = to_date('{1}', 'dd.mm.yyyy')",
                    fisd_id, dat.ToString("dd.MM.yyyy"));
            var result = dbLink.GetConnection().QueryFirstOrDefault<fisd_dq>(query);
            return result;
        }

        //insert 
        public static void Insert(DbLink dbLink, fisd_dq fisd_dq)
        {
            string query =
                @"insert into fisd_dq(val, dat, fisd_id)
                    values(@val, @dat, @fisd_id)";
            dbLink.GetConnection().Execute(query, fisd_dq);
        }

        //remove
        public static void Remove(DbLink dbLink, int fisd_id, DateTime dat)
        {
            string query =
                string.Format(@"delete from fisd_dq t where t.fisd_id = {0} 
                    and t.dat = to_date('{1}', 'dd.MM.yyyy')", fisd_id, dat.ToString("dd.MM.yyyy"));
            dbLink.GetConnection().Execute(query);
        }

        public static void Update(DbLink dbLink, fisd_dq fisd_dq)
        {
            string query =
                string.Format(
                    @"update fisd_dq fd 
                    set val = @val 
                    where fd.dat = @dat and fd.fisd_id = @fisd_id");
            dbLink.GetConnection().Execute(query, fisd_dq);
        }
    }
}
