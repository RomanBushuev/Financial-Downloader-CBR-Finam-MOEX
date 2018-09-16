using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProvider.Output.Mir.DbObject
{
    public class curve
    {
        public curve()
        {

        }

        public curve(int cur_id, string ident, string description, string title)
        {
            this.cur_id = cur_id;
            this.ident = ident;
            this.description = description;
            this.title = title;
        }
        public curve(string ident, string description, string title)
        {
            this.ident = ident;
            this.description = description;
            this.title = title;
        }
        public int cur_id { get; set; }
        public string ident { get; set; }
        public string description { get; set; }
        public string title { get; set; }
    }
}
