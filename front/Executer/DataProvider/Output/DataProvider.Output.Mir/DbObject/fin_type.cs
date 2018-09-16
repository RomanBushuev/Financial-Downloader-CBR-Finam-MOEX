using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProvider.Output.Mir.DbObject
{
    public class fin_type
    {
        public fin_type(string ident, string title)
        {
            this.ident = ident;
            this.title = title;
        }

        public fin_type(int ft_id, string ident, string title)
        {
            this.ft_id = ft_id;
            this.ident = ident;
            this.title = title;
        }

        public int ft_id { get; set; }
        public string ident { get; set; }
        public string title { get; set; }
    }
}
