using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProvider.Output.Mir.DbObject
{
    public class data_source
    {
        public data_source()
        {

        }

        public data_source(string ident, string description = "")
        {
            this.ident = ident;
            this.description = description;
        }

        public int ds_id { get; set; }
        public string ident { get; set; }
        public string description { get; set; }
        public int ord_id { get; set; }
    }
}
