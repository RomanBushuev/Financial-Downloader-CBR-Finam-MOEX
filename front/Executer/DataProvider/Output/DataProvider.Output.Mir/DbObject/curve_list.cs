using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProvider.Output.Mir.DbObject
{
    public class curve_list
    {
        public curve_list()
        {

        }

        public int cur_id { get; set; }
        public int fi_id { get; set; }
        public string ident { get; set; }
        public int ord_id { get; set; }
        public decimal term { get; set; }
        public DateTime dat { get; set; }
    }
}
