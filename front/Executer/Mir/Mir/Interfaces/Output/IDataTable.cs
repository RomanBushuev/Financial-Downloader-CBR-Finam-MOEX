using Core.Mir.BaseTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Mir.Interfaces
{
    public interface IDataTable
    {
        ResultSet returnResultSet();
    }
}
