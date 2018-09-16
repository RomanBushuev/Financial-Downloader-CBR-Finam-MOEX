using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Mir.Enumerations
{
    [Flags]
    public enum ParamType
    {
        Default = 1 << 0,
        Decimal = 1 << 1, 
        String = 1 << 2,
        Int = 1 << 3,
        DateTime = 1 << 4
    }
}
