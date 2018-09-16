using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Core.Mir.BaseTypes;
using Core.Mir.Enumerations;
using Core.Mir.BaseTypes;

namespace Core.Mir.Interfaces
{
    public interface IMarketProvider
    {
        Dictionary<KeyValuePair<PortfolioPosition, Enum>, object> GetAllData();
        List<PortfolioPosition> GetAllPositions();
        List<PortfolioPosition> GetAllPositions(FinType finType);
        List<PortfolioPosition> GetAllPositions(List<FinType> fintypes);
        IMapping GetIMapping();
        ScalarDate GetScalarDate(PortfolioPosition position, ScalarAttribute attribute);
        ScalarEnum GetScalarEnum(PortfolioPosition position, ScalarAttribute attribute);
        ScalarNum GetScalarNum(PortfolioPosition position, ScalarAttribute attribute);
        ScalarStr GetScalarStr(PortfolioPosition position, ScalarAttribute attribute);
        TimeSeries GetTimeSeries(PortfolioPosition position, TimeSeriesAttribute attribute);
        TimeSeries GetTimeSeries(PortfolioPosition position, TimeSeriesAttribute attribute, DateTime from, DateTime to);
        TimeSeries GetTimeSeries(Enum enumeration, TimeSeriesAttribute attribute);
        TimeSeries GetTimeSeries(Enum enumeration, TimeSeriesAttribute attribute, DateTime from, DateTime to);

        bool Get<T>(PortfolioPosition position,
            ScalarAttribute attribute,
            DateTime dateTime);

        T Get<T>(PortfolioPosition position, 
            ScalarAttribute attribute,
            DateTime dateTime,
            T defaultValue) where T : struct, IConvertible;

        void ClearCache();
    }
}
