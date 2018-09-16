using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Core.Mir;
using Core.Mir.BaseTypes;
using Core.Mir.Enumerations;

namespace Core.Mir.BaseTypes
{
    public class TimeSeries
    {
        private TimeSeriesAttribute _attribute = TimeSeriesAttribute.Close;

        SortedDictionary<DateTime, decimal> _timeSeries = new SortedDictionary<DateTime, decimal>();
        public TimeSeries(TimeSeriesAttribute attribute = TimeSeriesAttribute.Close)
        {
            _attribute = attribute;
        }
        public TimeSeries(IDictionary<DateTime, decimal> timeSeries,
            TimeSeriesAttribute attribute = TimeSeriesAttribute.Close)
        {
            _timeSeries = new SortedDictionary<DateTime, decimal>(timeSeries);
            _attribute = attribute;
        }

        public IDictionary<DateTime, decimal> Series { get { return _timeSeries; } }

        public bool Add(DateTime dateTime, decimal value)
        {
            if(_timeSeries.ContainsKey(dateTime))
            {
                _timeSeries[dateTime] = value;
                return true;
            }
            else
            {
                _timeSeries.Add(dateTime, value);
                return false;
            }
        }

        public bool Contains(DateTime dateTime)
        {
            if (_timeSeries.ContainsKey(dateTime))
                return true;
            return false;
        }

        public TimeSeriesAttribute Attribute
        {
            get { return _attribute; }
            set { _attribute = value; }
        }
    }
}
