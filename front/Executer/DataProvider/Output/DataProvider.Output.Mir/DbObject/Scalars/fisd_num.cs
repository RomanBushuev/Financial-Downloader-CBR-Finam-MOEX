using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProvider.Output.Mir.DbObject
{
    public class fisd_num
    {
        public fisd_num()
        {

        }

        public fisd_num(decimal val,DateTime dat_from, int fisd_id)
        {
            this.fisd_id = fisd_id;
            this.val = val;
            this.dat_from = dat_from;
        }

        public decimal val { get; set; }
        public DateTime dat_from { get; set; }
        public int fisd_id { get; set; }
    }
}
