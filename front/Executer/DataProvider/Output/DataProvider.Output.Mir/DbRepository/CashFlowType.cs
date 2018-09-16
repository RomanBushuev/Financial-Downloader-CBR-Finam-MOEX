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
    public class CashFlowType
    {
        public static cashflow_types FindId(DbLink dbLink, string ident)
        {
            string query =
                string.Format(@"select * from cashflow_types t
                    where t.ident = upper('{0}');", ident);
            var result = dbLink.GetConnection().QueryFirstOrDefault<cashflow_types>(query);
            return result;
        }
    }
}
