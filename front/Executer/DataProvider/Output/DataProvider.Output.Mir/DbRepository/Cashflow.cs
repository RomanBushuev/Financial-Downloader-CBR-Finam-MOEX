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
    public class Cashflow
    {
        public static cashflow FindId(DbLink dbLink, int cf_id, DateTime dateTime, DateTime validDatetime)
        {
            cashflow cf= new cashflow();
            cf.cf_id = cf_id;
            cf.dat = dateTime;
            cf.valid_dat = validDatetime;

            string query =
                string.Format(@"select * from cashflow t
                    where t.cf_id = @cf_id and t.dat = @dat and t.valid_dat <= @valid_dat");

            var result = dbLink.GetConnection().QueryFirstOrDefault<cashflow>(query, cf);
            return result;
        }

        public static void Insert(DbLink dbLink, cashflow cashflow)
        {
            string query = @"insert into cashflow(cf_id, val, dat, valid_dat)
                            values(@cf_id, @val, @dat, @valid_dat)";
            dbLink.GetConnection().Execute(query, cashflow);
        }
    }
}
