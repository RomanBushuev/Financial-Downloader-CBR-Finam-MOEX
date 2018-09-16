using Core.Mir;
using Core.Mir.BaseTypes;
using Core.Mir.Enumerations;
using Core.Mir.Interfaces;
using DataBaseLink;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataProvider.Input.MirReader.DbObject;
using System.Linq;

namespace DataProvider.Input.MirReader
{
    public class Provider : IMarketProvider, IGetParams, ISetParams,IDisposable
    {
        private Dictionary<KeyValuePair<PortfolioPosition, Enum>, object> _cache =
            new Dictionary<KeyValuePair<PortfolioPosition, Enum>, object>();

        public const string SCALAR = "ScalarSource";
        public const string QUOTE = "QuoteSource";

        private DbLink _dbLink = null;
        private string _connection = string.Empty;
        private IMapping _mapping = null;
        private Dictionary<string, object> _providerParams =
            new Dictionary<string, object>()
                    {
                        {SCALAR, "CALCULATED"},
                        {QUOTE, "CALCULATED"},
                    };

        private List<ParamDescriptor> paramDescriptors =
            new List<ParamDescriptor>()
            {
                new ParamDescriptor()
                {
                    Ident = SCALAR,
                    Description = "",
                    ParamType = ParamType.String,
                    Value = "CALCULATED",
                },

                new ParamDescriptor()
                {
                    Ident = QUOTE,
                    Description = "",
                    ParamType = ParamType.String,
                    Value = "CALCULATED"
                }
            };

        public Provider(string connection)
            :this(connection, new Mapping())
        {

        }

        public Provider(string connection, Mapping mapping)
        {
            _connection = connection;
            _mapping = mapping;
            var tempConnection = DataBaseLink.Fabricate.CreateConnection(_connection, ConnectionType.Npgsql);
            _dbLink = new DbLink(tempConnection);

        }

        public List<ParamDescriptor> GetParams()
        {
            return paramDescriptors;
        }

        public void SetParams(Dictionary<string, object> objects)
        {
            foreach (var x in objects)
            {
                if (_providerParams.ContainsKey(x.Key))
                    _providerParams[x.Key] = x.Value;
                else
                    _providerParams.Add(x.Key, x.Value);

            }
        }

        public Dictionary<KeyValuePair<PortfolioPosition, Enum>, object> GetAllData()
        {
            throw new NotImplementedException();
        }

        public List<PortfolioPosition> GetAllPositions()
        {
            
            string query = 
                @"select fi.ident as ident, ft.ident as finType from fin_instrument fi
                    join fin_type ft on fi.ft_id = ft.ft_id";

            var result = _dbLink.GetConnection().Query<finInstrument>(query);
            List<PortfolioPosition> positions = new List<PortfolioPosition>(result.Count());
            foreach (var x in result)
            {
                FinType finType = (FinType)_mapping.Get<FinType>(x.finType, FinType.Default);
                BalancePosition position = new BalancePosition(x.ident, finType);
                positions.Add(position);

            }
            return positions;
            throw new NotImplementedException();
        }

        public List<PortfolioPosition> GetAllPositions(FinType finType)
        {
            var result = GetAllPositions();
            return result.Where(z => z.FinType == finType).ToList();
        }

        public List<PortfolioPosition> GetAllPositions(List<FinType> fintypes)
        {
            var result = GetAllPositions();
            return result.Where(z => fintypes.Contains(z.FinType)).ToList();
        }

        public IMapping GetIMapping()
        {
            return _mapping;
        }

        public ScalarDate GetScalarDate(PortfolioPosition position,
            ScalarAttribute attribute)
        {
            if (!_mapping.FindAI(attribute))
                return null;

            KeyValuePair<PortfolioPosition, Enum> key =
                new KeyValuePair<PortfolioPosition, Enum>(position, attribute);

            if (_cache.ContainsKey(key))
            {
                return (ScalarDate)_cache[key];
            }
            ScalarDate scalarDate = null;

            string fif_id = _mapping.GetAI(attribute);
            string query = string.Format(
                @"SELECT fi.ident as ident, fd.dat_from as dat_from, fd.val as val
                        from fin_instrument fi join
	                        ffd t on t.fi_id = fi.fi_id join
	                        data_source ds on t.ds_id = ds.ds_id join
	                        fisd_date fd on fd.fisd_id = t.fisd_id 
	                        where fi.ident = '{0}'
		                        and t.fif_id = {1}
		                        and ds.ident = '{2}'",
                    position.Ident,
                    fif_id,
                    _providerParams[SCALAR]);
            var result = _dbLink.GetConnection().Query<fisddate>(query);
            if (result != null && result.Count() != 0)
            {

                Dictionary<DateTime, DateTime> dict = new Dictionary<DateTime, DateTime>();
                foreach (var x in result)
                {
                    dict.Add(x.dat_from, x.val);
                }
                scalarDate = new ScalarDate(dict);
            }


            _cache.Add(key, scalarDate);
            return scalarDate;
        }

        public ScalarEnum GetScalarEnum(PortfolioPosition position, 
            ScalarAttribute attribute)
        {
            KeyValuePair<PortfolioPosition, Enum> key =
                new KeyValuePair<PortfolioPosition, Enum>(position, attribute);

            if(!_cache.ContainsKey(key))
            {
                ScalarStr scalarStr = null;
                string fif_id = _mapping.GetAI(attribute);
                string query = string.Format(
                    @"SELECT fi.ident, fd.dat_from,  fd.val, *
                        from fin_instrument fi join
	                        ffd t on t.fi_id = fi.fi_id join
	                        data_source ds on t.ds_id = ds.ds_id join
	                        fisd_item fd on fd.fisd_id = t.fisd_id 
	                        where fi.ident = '{0}'
		                        and t.fif_id = {1}
		                        and ds.ident = '{2}'",
                       position.Ident,
                       fif_id,
                       _providerParams[SCALAR]);
                var result = _dbLink.GetConnection().Query<fisdstring>(query);
                if (result != null && result.Count() != 0)
                {
                    Dictionary<DateTime, string> values = new Dictionary<DateTime, string>();
                    foreach (var x in result)
                    {
                        values.Add(x.dat_from, x.val);
                    }
                    scalarStr = new ScalarStr(values);
                }

                _cache.Add(key, scalarStr);
            }

            ScalarStr str = (ScalarStr)_cache[key];
            if (str == null)
                return null;

            //прочитаем все значения
            if(!_mapping.FindET(attribute))
            {
                return null;
            }

            Type type = _mapping.GetET(attribute);
            var bushuevs =
                Enum.GetValues(type).OfType<Enum>().OrderBy(z=>z).ToList();

            if (bushuevs.Count == 0)
                return null;

            Dictionary<DateTime, Enum> enumerations = new Dictionary<DateTime, Enum>();
            foreach(var x in str.Dictionary)
            {
                KeyValuePair<Type, string> keyEnum =
                    new KeyValuePair<Type, string>(type, x.Value);
                 
                if(_mapping.TKE.ContainsKey(keyEnum))
                {
                    Enum enumeration = _mapping.TKE[keyEnum];
                    enumerations.Add(x.Key, enumeration);
                }
                else
                {
                    Enum enumeration = bushuevs.First();
                    enumerations.Add(x.Key, enumeration);
                }
            }
            ScalarEnum scalarEnum = new ScalarEnum(enumerations);
            //конвертируем их в соответствии с типом, если получается 

            return scalarEnum;
        }

        public ScalarNum GetScalarNum(PortfolioPosition position,
            ScalarAttribute attribute)
        {
            if (!_mapping.FindAI(attribute))
                return null;

            KeyValuePair<PortfolioPosition, Enum> key =
                new KeyValuePair<PortfolioPosition, Enum>(position, attribute);
            if(_cache.ContainsKey(key))
            {
                return (ScalarNum)_cache[key];
            }

            string fif_id = _mapping.GetAI(attribute);
            ScalarNum scalarNum = null;
            string query =
                string.Format(
                    @"	SELECT fi.ident as ident, fd.dat_from as dat_from, fd.val as val
                        from fin_instrument fi join
	                        ffd t on t.fi_id = fi.fi_id join
	                        data_source ds on t.ds_id = ds.ds_id join
	                        fisd_num fd on fd.fisd_id = t.fisd_id 
	                        where fi.ident = '{0}'
		                        and t.fif_id = {1}
		                        and ds.ident = '{2}'",
                        position.Ident,
                        fif_id, 
                        _providerParams[SCALAR]);

            var result = _dbLink.GetConnection().Query<fisdnumber>(query);
            if(result != null && result.Count() != 0)
            {
                Dictionary<DateTime, decimal> values = new Dictionary<DateTime,decimal>();
                foreach(var x in result)
                {
                    values.Add(x.dat_from, x.val);
                }
                scalarNum = new ScalarNum(values);
            }

            _cache.Add(key, scalarNum);
            return scalarNum;
        }

        public ScalarStr GetScalarStr(PortfolioPosition position,
            ScalarAttribute attribute)
        {
            if (!_mapping.FindAI(attribute))
                return null;

            KeyValuePair<PortfolioPosition, Enum> key =
                new KeyValuePair<PortfolioPosition, Enum>(position, attribute);
            if(_cache.ContainsKey(key))
            {
                return (ScalarStr)_cache[key];
            }

            string fif_id = _mapping.GetAI(attribute);
            ScalarStr scalarStr = null;
            string query =
                string.Format(
                    @"	SELECT fi.ident as ident, fd.dat_from as dat_from,  fd.val as val
                        from fin_instrument fi join
	                        ffd t on t.fi_id = fi.fi_id join
	                        data_source ds on t.ds_id = ds.ds_id join
	                        fisd_str fd on fd.fisd_id = t.fisd_id 
	                        where fi.ident = '{0}'
		                        and t.fif_id = {1}
		                        and ds.ident = '{2}'",
                         position.Ident,
                         fif_id,
                         _providerParams[SCALAR]);

            var result = _dbLink.GetConnection().Query<fisdstring>(query);

            if(result == null || result.Count() == 0)
            {
                query =
                    string.Format(
                    @"	SELECT fi.ident as ident, fd.dat_from as dat_from,  fd.val as val
                        from fin_instrument fi join
	                        ffd t on t.fi_id = fi.fi_id join
	                        data_source ds on t.ds_id = ds.ds_id join
	                        fisd_item fd on fd.fisd_id = t.fisd_id 
	                        where fi.ident = '{0}'
		                        and t.fif_id = {1}
		                        and ds.ident = '{2}'",
                         position.Ident,
                         fif_id,
                         _providerParams[SCALAR]);
                result = _dbLink.GetConnection().Query<fisdstring>(query);
            }

            if(result != null && result.Count() != 0)
            {
                Dictionary<DateTime, string> values = new Dictionary<DateTime,string>();
                foreach(var x in result)
                {
                    values.Add(x.dat_from, x.val);
                }
                scalarStr = new ScalarStr(values);
            }

            _cache.Add(key, scalarStr);
            return scalarStr;
        }

        public TimeSeries GetTimeSeries(PortfolioPosition position, TimeSeriesAttribute attribute)
        {
            KeyValuePair<PortfolioPosition, Enum> key = new KeyValuePair<PortfolioPosition,Enum>();
            if(_cache.ContainsKey(key))
            {
                return (TimeSeries)_cache[key];
            }

            TimeSeries timeSeries = null;
            string query = string.Format(
                @"SELECT fi.ident,  fd.dat, fd.val
                    from fin_instrument fi join
	                    ffd t on t.fi_id = fi.fi_id join
	                    fisd_dq fd on fd.fisd_id = t.fisd_id join
	                    data_source ds on ds.ds_id = t.ds_id
	                    and fi.ident = '{0}'
	                    and ds.ident = '{1}'
	                    and t.fif_id = {2}",
                        position.Ident,
                        _providerParams[SCALAR],
                        _mapping.GetAI(attribute));

            var result = _dbLink.GetConnection().Query<fisddq>(query);
            if (result != null && result.Count() != 0)
            {
                Dictionary<DateTime, decimal> values = new Dictionary<DateTime, decimal>();
                foreach (var x in result)
                {
                    values.Add(x.dat, x.val);
                }
                timeSeries = new TimeSeries(values, attribute);
            }

            return timeSeries;
        }

        public TimeSeries GetTimeSeries(PortfolioPosition position, 
            TimeSeriesAttribute attribute,
            DateTime from, 
            DateTime to)
        {
            TimeSeries timeSeries = GetTimeSeries(position, attribute);
            if (timeSeries == null)
                return null;

            var result = timeSeries.Series.Where(z => z.Key >= from && z.Key <= to);
            if (result.Count() == 0)
                return null;
            else
            {
                Dictionary<DateTime, decimal> values = new Dictionary<DateTime, decimal>();
                foreach(var x in result)
                {
                    values.Add(x.Key, x.Value);
                }
                return new TimeSeries(values, attribute);
            }
        }

        public bool Get<T>(PortfolioPosition position,
            ScalarAttribute attribute,
            DateTime dateTime)
        {
            ScalarStr str = GetScalarStr(position, attribute);
            if (str == null || !str.HasValue(dateTime))
                return false;
            return true;
        }

        public T Get<T>(PortfolioPosition position,
            ScalarAttribute attribute,
            DateTime dateTime, 
            T defaultValue) where T : struct, IConvertible
        {
            //прочитали скалярный атрибут scalarStr 
            //нашли в нем наше значение 
            //вернули
            //не нашли в нем наше значение вернули defaultValue

            ScalarStr str = GetScalarStr(position, attribute);
            if(str == null || !str.HasValue(dateTime))
                return defaultValue;

            string strIdent = str.Get(dateTime);
            KeyValuePair<Type, string> tke =
                new KeyValuePair<Type, string>(typeof(T), strIdent);

            if(_mapping.TKE.ContainsKey(tke))
            {
                Enum value = _mapping.TKE[tke];
                return (T)Enum.Parse(typeof(T), value.ToString(), true);
            }
            else
            {
                return defaultValue;
            }
        }


        public TimeSeries GetTimeSeries(Enum enumeration, TimeSeriesAttribute attribute)
        {
            if (!_mapping.FindAI(enumeration))
                return null;

            string ident = _mapping.GetAI(enumeration);
            BalancePosition position = new BalancePosition(ident, FinType.FxRate);

            KeyValuePair<PortfolioPosition, Enum> key =
                new KeyValuePair<PortfolioPosition, Enum>(position, attribute);

            if(_cache.ContainsKey(key))
            {
                return (TimeSeries)_cache[key];
            }

            TimeSeries timeSeries = GetTimeSeries(position, attribute);
            _cache.Add(key, timeSeries);
            return timeSeries;
        }

        public TimeSeries GetTimeSeries(Enum enumeration, TimeSeriesAttribute attribute, 
            DateTime from, DateTime to)
        {
            TimeSeries timeSeries = GetTimeSeries(enumeration, attribute);
            if (timeSeries == null || timeSeries.Series == null || timeSeries.Series.Count == 0)
                return null;
            var result = timeSeries.Series.Where(z => z.Key >= from && z.Key <= to);
            if (result.Count() == 0)
                return null;
            else
            {
                Dictionary<DateTime, decimal> values = new Dictionary<DateTime, decimal>();
                foreach (var x in result)
                {
                    values.Add(x.Key, x.Value);
                }
                return new TimeSeries(values, attribute);
            }

        }

        public void Dispose()
        {
            _dbLink.Close();
        }


        public void ClearCache()
        {
            _cache.Clear();
        }
    }
}
