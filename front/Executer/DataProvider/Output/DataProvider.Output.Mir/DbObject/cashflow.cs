using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProvider.Output.Mir.DbObject
{
    public class cashflow
    {
        public int cf_id { get; set; }
        public decimal? val { get; set; }
        public DateTime dat { get; set; }
        public DateTime valid_dat { get; set; }
    }
}
