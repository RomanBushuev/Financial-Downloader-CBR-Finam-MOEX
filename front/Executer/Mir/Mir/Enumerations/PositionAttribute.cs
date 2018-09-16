using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Mir.Enumerations
{
    public enum PositionAttribute
    {
        Default = 1 << 0,
        Ident = 1 << 1,
        Type = 1 << 2,
        Amount = 1 << 3,
        ActualDate = 1 << 4,
    }
}
