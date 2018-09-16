using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProvider.Output.Mir.DbObject
{
    public class fisd_item
    {
        public fisd_item()
        {

        }

        public fisd_item(int fisd_id, DateTime dat_from, string val)
        {
            this.fisd_id = fisd_id;
            this.dat_from = dat_from;
            this.val = val;
        }

        public string val { get; set; }
        public DateTime dat_from { get; set; }
        public int fisd_id { get; set; }

    }
}
