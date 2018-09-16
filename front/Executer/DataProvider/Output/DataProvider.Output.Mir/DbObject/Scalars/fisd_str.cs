using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProvider.Output.Mir.DbObject
{
    public class fisd_str
    {
        public fisd_str()
        {

        }

        public fisd_str(int fisd_id, string val, DateTime dat_from)
        {
            this.val = val;
            this.fisd_id = fisd_id;
            this.dat_from = dat_from;
        }

        public string val { get; set; }
        public DateTime dat_from { get; set; }
        public int fisd_id { get; set; }
    }
}
