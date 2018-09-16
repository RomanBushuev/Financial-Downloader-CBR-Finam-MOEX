using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Mir.Interfaces;
using Core.Mir.BaseTypes;

namespace Core.Mir
{
    public class PortfolioData
    {
        private IPortfolioProvider _dbProvider = null;
        private bool isDownload = false;
        private SortedList<PortfolioPosition, PortfolioPosition> _positions =
            new SortedList<PortfolioPosition,PortfolioPosition>();
        private List<string> _portfolioTitles = new List<string>();

        public PortfolioData(IPortfolioProvider provider, string portfolioTitle)
        {
            _dbProvider = provider;
            _portfolioTitles.Add(portfolioTitle);
        }

        public PortfolioData(IPortfolioProvider provider, List<string> portfolioTitles)
        {
            _dbProvider = provider;
            _portfolioTitles = portfolioTitles;
        }

        public void Download()
        {
            List<PortfolioPosition> positions = _dbProvider.Download(_portfolioTitles);
            _positions = new SortedList<PortfolioPosition, PortfolioPosition>(positions.Count);
            foreach(var x in positions)
            {
                _positions.Add(x, x);
            }
        }

        public void Clear()
        {
            _positions.Clear();
            isDownload = false;
            _dbProvider.Clear();
        }
        
    }
}
