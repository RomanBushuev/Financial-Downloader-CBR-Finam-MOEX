
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Mir.BaseTypes;

namespace Core.Mir.Interfaces
{
    public interface IPortfolioProvider
    {
        List<PortfolioPosition> Download(string portfolioTitle);

        List<PortfolioPosition> Download(IList<string> portfolioTitle);

        void Clear();
    }
}
