using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Mir.Interfaces;
using Core.Mir.Enumerations;
using Core.Mir.BaseTypes;
using QLNet;

namespace Core.Mir
{
    public class MarketData
    {
        IMarketProvider _dataProvider = null;
        public MarketData(IMarketProvider dataProvider)
        {
            _dataProvider = dataProvider; 
        }

        public ScalarDate GetScalarDate(PortfolioPosition position, ScalarAttribute attribute)
        {
            return _dataProvider.GetScalarDate(position, attribute);
        }

        public ScalarEnum GetScalarEnum(PortfolioPosition position, ScalarAttribute attribute)
        {
            return _dataProvider.GetScalarEnum(position, attribute);
        }

        public ScalarNum GetScalarNum(PortfolioPosition position, ScalarAttribute attribute)
        {
            return _dataProvider.GetScalarNum(position, attribute);
        }

        public ScalarStr GetScalarStr(PortfolioPosition position, ScalarAttribute attribute)
        {
            return _dataProvider.GetScalarStr(position, attribute);
        }

        public QLNet.Bond GetBond(PortfolioPosition position, DateTime calculatingDate)
        {
            throw new NotImplementedException();
        }

        public TimeSeries GetTimeSeries(PortfolioPosition position, TimeSeriesAttribute attribute)
        {
            return _dataProvider.GetTimeSeries(position, attribute);
        }

        public TimeSeries GetTimeSeries(PortfolioPosition position, TimeSeriesAttribute attribute, DateTime from, DateTime to)
        {
            return _dataProvider.GetTimeSeries(position, attribute, from, to);
        }

        public TimeSeries GetTimeSeries(Enum enumeration, TimeSeriesAttribute attribute)
        {
            return _dataProvider.GetTimeSeries(enumeration, attribute);
        }

        public TimeSeries GetTimeSeries(Enum enumeration, TimeSeriesAttribute attribute, DateTime from, DateTime to)
        {
            return _dataProvider.GetTimeSeries(enumeration, attribute, from, to);
        }
        public List<PortfolioPosition> GetAllPositions()
        {
            return _dataProvider.GetAllPositions();
        }


        public List<PortfolioPosition> GetAllPositions(FinType finType)
        {
            return _dataProvider.GetAllPositions(finType);
        }

        public List<PortfolioPosition> GetAllPositions(List<FinType> finTypes)
        {
            return _dataProvider.GetAllPositions(finTypes);
        }
        public Dictionary<KeyValuePair<PortfolioPosition, Enum>, object> GetAllData()
        {
            return _dataProvider.GetAllData();
        }

        public T Get<T>(PortfolioPosition position,
            ScalarAttribute attribute,
            DateTime dateTime,
            T defaultValue) where T : struct, IConvertible
        {
            return _dataProvider.Get<T>(position, attribute, dateTime, defaultValue);
        }
    }
}
