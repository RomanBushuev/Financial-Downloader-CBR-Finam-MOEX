using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Mir.Enumerations
{
    [Flags]
    public enum Currencies
    {
        Default = 1 << 0,
        CHF = 1 << 1,
        RUB = 1 << 2,
        USD = 1 << 3,
        EUR = 1 << 4,
        GBP = 1 << 5
    }
}
