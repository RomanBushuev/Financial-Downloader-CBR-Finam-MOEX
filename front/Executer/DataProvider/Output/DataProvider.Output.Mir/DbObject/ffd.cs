using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProvider.Output.Mir.DbObject
{
    public class ffd
    {
        public ffd()
        {

        }
        public ffd(int fi_id, int ds_id, int fif_id, int fisd_id)
        {
            this.fi_id = fi_id;
            this.ds_id = ds_id;
            this.fif_id = fif_id;
            this.fisd_id = fisd_id;
        }
        public int fi_id { get; set; }
        public int ds_id { get; set; }
        public int fif_id { get; set; }
        public int fisd_id { get; set; }
    }
}
