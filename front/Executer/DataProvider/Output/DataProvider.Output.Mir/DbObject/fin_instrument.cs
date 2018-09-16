using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Mir.BaseTypes;

namespace DataProvider.Output.Mir.DbObject
{
    public class fin_instrument
    {
        public fin_instrument()
        {

        }

        public fin_instrument(string ident, int ft_id, string title)
        {
            this.ident = ident;
            this.ft_id = ft_id;
            this.title = title;
        }

        public fin_instrument(int fi_id, string ident, int ft_id, string title)
        {
            this.fi_id = fi_id;
            this.ident = ident;
            this.ft_id = ft_id;
            this.title = title;
        }

        public int fi_id { get; set; }
        public string ident { get; set; }
        public int ft_id { get; set; }
        public string title { get; set; }
    }
}
