using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Mir.Interfaces;

namespace Core.Mir
{
    public class ScenarioData
    {
        private IScenarioProvider _provider = null;

        public ScenarioData(IScenarioProvider provider)
        {
            _provider = provider;
        }
    }
}
