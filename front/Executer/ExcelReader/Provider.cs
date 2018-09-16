using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Mir.BaseTypes;
using Core.Mir.Interfaces;

namespace DataProvider.Input.ExcelReader
{
    public class Provider : IMarketProvider, IGetParams, ISetParams
    {
        private Mapping _mapping = new Mapping();
        private string _connection = string.Empty;
        public Provider(string connection)
        {
            _connection = connection;
        }

        public void Download()
        {

        }

        public IMapping GetIMapping()
        {
            return _mapping;
        }

        public ScalarDate GetScalarDate(PortfolioPosition position, Core.Mir.Enumerations.ScalarAttribute attribute)
        {
            return null;
        }

        public ScalarEnum GetScalarEnum(PortfolioPosition position, Core.Mir.Enumerations.ScalarAttribute attribute)
        {
            return null;
        }

        public ScalarNum GetScalarNum(PortfolioPosition position, Core.Mir.Enumerations.ScalarAttribute attribute)
        {
            return null;
        }

        public ScalarStr GetScalarStr(PortfolioPosition position, Core.Mir.Enumerations.ScalarAttribute attribute)
        {
            return null;
        }

        public TimeSeries GetTimeSeries(PortfolioPosition position, Core.Mir.Enumerations.TimeSeriesAttribute attribute)
        {
            return null;
        }

        public TimeSeries GetTimeSeries(PortfolioPosition position, Core.Mir.Enumerations.TimeSeriesAttribute attribute, DateTime from, DateTime to)
        {
            return null;
        }

        public List<Core.Mir.ParamDescriptor> GetParams()
        {
            return new List<Core.Mir.ParamDescriptor>();
        }

        public void SetParams(Dictionary<string, object> objects)
        {
            return;
        }

        public Dictionary<KeyValuePair<PortfolioPosition, Enum>, object> GetAllData()
        {
            return new Dictionary<KeyValuePair<PortfolioPosition, Enum>, object>();
        }

        public List<PortfolioPosition> GetAllPositions()
        {
            return new List<PortfolioPosition>(); 
        }

        public List<PortfolioPosition> GetAllPositions(Core.Mir.Enumerations.FinType finType)
        {
            return new List<PortfolioPosition>();
        }


        public List<PortfolioPosition> GetAllPositions(List<Core.Mir.Enumerations.FinType> fintypes)
        {
            return new List<PortfolioPosition>();
        }


        public bool Get<T>(PortfolioPosition position, Core.Mir.Enumerations.ScalarAttribute attribute, DateTime dateTime)
        {
            return false;
        }

        public T Get<T>(PortfolioPosition position, Core.Mir.Enumerations.ScalarAttribute attribute, DateTime dateTime, T defaultValue) where T : struct, IConvertible
        {
            return defaultValue;
        }


        public TimeSeries GetTimeSeries(Enum enumeration, Core.Mir.Enumerations.TimeSeriesAttribute attribute)
        {
            return null;
        }

        public TimeSeries GetTimeSeries(Enum enumeration, Core.Mir.Enumerations.TimeSeriesAttribute attribute, DateTime from, DateTime to)
        {
            return null;
        }


        public void ClearCache()
        {
            
        }
    }
}
