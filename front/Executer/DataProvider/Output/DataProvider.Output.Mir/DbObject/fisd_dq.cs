using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProvider.Output.Mir.DbObject
{
    public class fisd_dq
    {
        public fisd_dq()
        {

        }
        public fisd_dq(int fisd_id, decimal val, DateTime dat)
        {
            this.val = val;
            this.fisd_id = fisd_id;
            this.dat = dat;
        }

        public decimal val { get; set; }
        public DateTime dat { get; set; }
        public int fisd_id { get; set; }
    }
}
