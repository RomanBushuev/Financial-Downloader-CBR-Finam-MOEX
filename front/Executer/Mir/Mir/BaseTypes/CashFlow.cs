using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Mir.Enumerations;

namespace Core.Mir.BaseTypes
{
    public class CashFlow
    {
        private CashFlowAttribute _attribute = CashFlowAttribute.Default;
        private SortedDictionary<DateTime, decimal?> _values =
            new SortedDictionary<DateTime, decimal?>();

        public CashFlow(CashFlowAttribute cfa = CashFlowAttribute.Default)
        {
            _attribute = cfa;
        }

        public CashFlow(CashFlowAttribute cfa, Dictionary<DateTime, decimal?> dict)
        {
            _attribute = cfa;
            _values = new SortedDictionary<DateTime,decimal?>(dict);
        }

        public void Add(DateTime dateTime, decimal? val)
        {
            _values.Add(dateTime, val);
        }
        public IDictionary<DateTime, decimal?> Values
        {
            get
            {
                return _values;
            }      
        }

        public CashFlowAttribute Attribute 
        {
            get 
            {
                return _attribute;
            }
            set
            {
                _attribute = value;
            }
        }
    }
}
