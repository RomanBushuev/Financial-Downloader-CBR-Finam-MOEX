using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Mir.Interfaces;

namespace Core.Mir
{
    public class Environment
    {
        private MarketData _marketData = null;
        private PortfolioData _portfolioData = null;
        private ScenarioData _scenarioData = null;

        public MarketData Market { get { return _marketData; } set { _marketData = value; } }
        public PortfolioData Portfolio { get { return _portfolioData; } set { _portfolioData = value; } }
        public ScenarioData Scenario { get { return _scenarioData; } set { _scenarioData = value; } }
    }
}
