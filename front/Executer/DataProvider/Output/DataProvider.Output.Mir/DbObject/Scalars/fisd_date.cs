using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProvider.Output.Mir.DbObject
{
    public class fisd_date
    {
        public fisd_date()
        {

        }
           
        public fisd_date(DateTime val, DateTime dateFrom)
        {
            this.val = val;
            this.dat_from = dateFrom;
        }

        public fisd_date(int fisd_id, DateTime val, DateTime dateFrom)
        {
            this.fisd_id = fisd_id;
            this.val = val;
            this.dat_from = dateFrom;
        }

        public int fisd_id { get; set; }
        public DateTime val { get; set; }
        public DateTime dat_from { get; set; }

    }
}
