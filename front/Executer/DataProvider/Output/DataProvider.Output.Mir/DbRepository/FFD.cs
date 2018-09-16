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
    public static class FFD
    {
        //find
        public static ffd Find(DbLink dbLink, int fi_id, int ds_id, int fif_id)
        {
            string query =
                string.Format(@"select * from ffd t
                    where t.fi_id = {0} and t.ds_id = {1} and fif_id = {2}",
                    fi_id, ds_id, fif_id);
            var result = dbLink.GetConnection().QueryFirstOrDefault<ffd>(query);
            return result;
        }
        //insert
        public static void Insert(DbLink dbLink, ffd ffd)
        {
            string query =
                @"insert into ffd(fisd_id, fi_id, ds_id, fif_id)
                    values(nextval('mir_sequence'), @fi_id, @ds_id, @fif_id);";
            dbLink.GetConnection().Execute(query, ffd);
        }

        //remove
        public static void Remove(DbLink dbLink, ffd ffd)
        {
            string query = string.Format("delete from ffd t where t.fi_id = {0} and t.ds_id = {1} and t.fif_id = {2}",
                ffd.fi_id,
                ffd.ds_id,
                ffd.fif_id);
            dbLink.GetConnection().Execute(query);
        }
    }
}
