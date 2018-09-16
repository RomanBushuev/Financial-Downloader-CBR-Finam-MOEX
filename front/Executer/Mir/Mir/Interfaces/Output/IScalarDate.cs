using Core.Mir.BaseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Mir.Interfaces
{
    public interface IScalarDate
    {
        Dictionary<PortfolioPosition, List<ScalarDate>> GetScalarDate();
        Dictionary<PortfolioPosition, List<ScalarNum>> GetScalarNum();
        Dictionary<PortfolioPosition, List<ScalarEnum>> GetScalarEnum();
        Dictionary<PortfolioPosition, List<ScalarStr>> GetScalarStr();
    }
}
