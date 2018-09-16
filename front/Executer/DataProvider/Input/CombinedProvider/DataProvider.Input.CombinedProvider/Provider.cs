using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Mir.BaseTypes;
using Core.Mir.Enumerations;
using Core.Mir.Interfaces;

namespace DataProvider.Input.CombinedProvider
{
    public class Provider : IMarketProvider, IDisposable
    {
        private Mapping _mapping = new Mapping();
        private List<IMarketProvider> _markets;
        public Provider(IEnumerable<IMarketProvider> markets)
        {
            _markets = markets.ToList();
            foreach(var market in _markets)
            {
                IMapping mapping = market.GetIMapping();
                foreach(var z in mapping.AI)
                {
                    if(!_mapping.FindAI(z.Key))
                        _mapping.AddAI(z.Key, z.Value);
                }

                foreach(var z in mapping.ET)
                {
                    if(!_mapping.FindET(z.Key))
                        _mapping.AddET(z.Key,z.Value);
                }

                foreach(var z in mapping.TKE)
                {
                    if(!_mapping.TKE.ContainsKey(z.Key))
                        _mapping.TKE.Add(z.Key, z.Value);
                }
            }
        }

        public Dictionary<KeyValuePair<PortfolioPosition, Enum>, object> GetAllData()
        {
            Dictionary<KeyValuePair<PortfolioPosition, Enum>, object> values =
                new Dictionary<KeyValuePair<PortfolioPosition, Enum>, object>();

            foreach(var x in _markets)
            {
                var result = x.GetAllData();
                foreach(var z in values)
                {
                    if(!values.ContainsKey(z.Key))
                        values.Add(z.Key, z.Value);
                }
            }

            return values;
        }

        public List<PortfolioPosition> GetAllPositions()
        {
            List<PortfolioPosition> positions = new List<PortfolioPosition>();
            foreach(var x in _markets)
            {
                positions.AddRange(x.GetAllPositions());
            }

            var t = positions.Distinct(new PortfolioPositionCompare()).ToList();
            return t;
        }

        public List<PortfolioPosition> GetAllPositions(FinType finType)
        {
            List<PortfolioPosition> positions = new List<PortfolioPosition>();
            foreach (var x in _markets)
            {
                positions.AddRange(x.GetAllPositions(finType));
            }

            var t = positions.Distinct(new PortfolioPositionCompare()).ToList();
            return t;
        }

        public List<PortfolioPosition> GetAllPositions(List<FinType> fintypes)
        {
            List<PortfolioPosition> positions = new List<PortfolioPosition>();
            foreach (var x in _markets)
            {
                positions.AddRange(x.GetAllPositions(fintypes));
            }

            var t = positions.Distinct(new PortfolioPositionCompare()).ToList();
            return t;
        }
        

        public IMapping GetIMapping()
        {
            return _mapping;
        }

        public ScalarDate GetScalarDate(PortfolioPosition position, ScalarAttribute attribute)
        {
            var markets = _markets.Where(z => z.GetIMapping().FindAI(attribute));
            foreach (var x in markets)
            {
                ScalarDate scalarDate = x.GetScalarDate(position, attribute);
                if (scalarDate != null)
                    return scalarDate;

            }
            return null;
        }

        public ScalarEnum GetScalarEnum(PortfolioPosition position, ScalarAttribute attribute)
        {
            var markets = _markets.Where(z => z.GetIMapping().FindAI(attribute));
            foreach(var x in markets)
            {
                ScalarEnum scalarEnum = x.GetScalarEnum(position, attribute);
                if (scalarEnum != null)
                    return scalarEnum;
            }
            return null;
        }

        public ScalarNum GetScalarNum(PortfolioPosition position, ScalarAttribute attribute)
        {
            var markets = _markets.Where(z => z.GetIMapping().FindAI(attribute));
            foreach(var x in markets)
            {
                ScalarNum scalarNum = x.GetScalarNum(position, attribute);
                if (scalarNum != null)
                    return scalarNum;
            }
            return null;
        }

        public ScalarStr GetScalarStr(PortfolioPosition position, ScalarAttribute attribute)
        {
            var markets = _markets.Where(z => z.GetIMapping().FindAI(attribute));
            foreach(var x in markets)
            {
                ScalarStr scalarStr = x.GetScalarStr(position, attribute);
                if (scalarStr != null)
                    return scalarStr;
            }
            return null;
        }

        public TimeSeries GetTimeSeries(PortfolioPosition position, TimeSeriesAttribute attribute)
        {
            var markets = _markets.Where(z => z.GetIMapping().FindAI(attribute));
            foreach(var x in markets)
            {
                TimeSeries timeSeries = x.GetTimeSeries(position, attribute);
                if (timeSeries != null)
                    return timeSeries;
            }
            return null;
        }

        public TimeSeries GetTimeSeries(PortfolioPosition position,
            TimeSeriesAttribute attribute, 
            DateTime from, 
            DateTime to)
        {
            var markets = _markets.Where(z => z.GetIMapping().FindAI(attribute));
            foreach(var x in markets)
            {
                TimeSeries timeSeries = x.GetTimeSeries(position, attribute, from, to);
                if (timeSeries != null)
                    return timeSeries;
            }
            return null;
        }

        public bool Get<T>(PortfolioPosition position,
            ScalarAttribute attribute,
            DateTime dateTime)
        {
            var markets = _markets.Where(z => z.GetIMapping().FindAI(attribute));
            foreach(var x in markets)
            {
                if(x.Get<T>(position,
                    attribute,
                    dateTime))
                {
                    return true;
                }
            }
            return false;
        }

        public T Get<T>(PortfolioPosition position, 
            ScalarAttribute attribute,
            DateTime dateTime, T defaultValue) where T : struct, IConvertible
        {
            var markets = _markets.Where(z => z.GetIMapping().FindAI(attribute));
            foreach(var x in markets)
            {
                if(x.Get<T>(position,
                    attribute,
                    dateTime))
                {
                    return x.Get<T>(position,
                        attribute,
                        dateTime,
                        defaultValue);
                }
            }

            return defaultValue;
        }


        public TimeSeries GetTimeSeries(Enum enumeration, TimeSeriesAttribute attribute)
        {
            var markets = _markets.Where(z => z.GetIMapping().FindAI(attribute));
            foreach(var x in markets)
            {
                var result = x.GetTimeSeries(enumeration, attribute);
                if (result == null)
                    continue;
                return result;
            }
            return null;
        }

        public TimeSeries GetTimeSeries(Enum enumeration, 
            TimeSeriesAttribute attribute, 
            DateTime from, 
            DateTime to)
        {
            var markets = _markets.Where(z => z.GetIMapping().FindAI(attribute));
            foreach(var x in markets)
            {
                var result = x.GetTimeSeries(enumeration,
                    attribute,
                    from,
                    to);

                if (result == null)
                    continue;
                return result;
            }
            return null;
        }

        public void Dispose()
        {
            foreach(var x in _markets)
            {
                if(x as IDisposable != null)
                {
                    ((IDisposable)x).Dispose();
                }
            }
            
        }





        public void ClearCache()
        {
            foreach(var x in _markets)
            {
                x.ClearCache();
            }
        }
    }
}
