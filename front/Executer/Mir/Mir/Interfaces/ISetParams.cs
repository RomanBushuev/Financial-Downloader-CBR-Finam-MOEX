using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Mir.Interfaces
{
    public interface ISetParams
    {
        void SetParams(Dictionary<string, object> objects);
    }
}
