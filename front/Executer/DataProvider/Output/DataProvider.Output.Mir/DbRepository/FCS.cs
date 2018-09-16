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
    public class FCS
    {
        //find
        public static fcs Find(DbLink dbLink, int fi_id, int ds_id, int ct_id)
        {
            string query =
                string.Format(@"select * from fcs t
                    where t.fi_id = {0} and t.ds_id = {1} and ct_id = {2}",
                    fi_id, ds_id, ct_id);
            var result = dbLink.GetConnection().QueryFirstOrDefault<fcs>(query);
            return result;
        }

        //insert
        public static void Insert(DbLink dbLink, fcs ffd)
        {
            string query =
                @"insert into fcs(cf_id, fi_id, ds_id, ct_id)
                    values(nextval('mir_sequence'), @fi_id, @ds_id, @ct_id);";
            dbLink.GetConnection().Execute(query, ffd);
        }
    }
}
