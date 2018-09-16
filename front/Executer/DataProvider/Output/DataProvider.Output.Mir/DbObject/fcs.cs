using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProvider.Output.Mir.DbObject
{
    public class fcs
    {
        public fcs()
        {

        }
        public fcs(int fi_id, int ds_id, int ct_id, int cf_id)
        {
            this.fi_id = fi_id;
            this.ds_id = ds_id;
            this.ct_id = ct_id;
            this.cf_id = cf_id;
        }
        public int fi_id { get; set; }
        public int ds_id { get; set; }
        public int ct_id { get; set; }
        public int cf_id { get; set; }
    }
}
