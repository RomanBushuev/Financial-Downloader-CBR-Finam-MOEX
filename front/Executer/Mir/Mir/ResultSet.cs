using Core.Mir.BaseTypes;
using Core.Mir.Enumerations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Mir
{
    public class ResultSet
    {
        private List<DataTable> _dataTables;
        private Dictionary<KeyValuePair<PortfolioPosition, Enum>, ScalarDate> _dates;
        private Dictionary<KeyValuePair<PortfolioPosition, Enum>, ScalarEnum> _enumerations;
        private Dictionary<KeyValuePair<PortfolioPosition, Enum>, ScalarNum> _numbers;
        private Dictionary<KeyValuePair<PortfolioPosition, Enum>, ScalarStr> _strings;
        private Dictionary<PortfolioPosition, TimeSeries> _timeseries;
        private Dictionary<KeyValuePair<PortfolioPosition, Enum>, CashFlow> _cashFlows;

        public ResultSet()
        {
            _dataTables = new List<DataTable>();
            _dates = new Dictionary<KeyValuePair<PortfolioPosition, Enum>, ScalarDate>();
            _enumerations = new Dictionary<KeyValuePair<PortfolioPosition, Enum>, ScalarEnum>();
            _numbers = new Dictionary<KeyValuePair<PortfolioPosition, Enum>, ScalarNum>();
            _strings = new Dictionary<KeyValuePair<PortfolioPosition, Enum>, ScalarStr>();
            _timeseries = new Dictionary<PortfolioPosition, TimeSeries>();
            _cashFlows = new Dictionary<KeyValuePair<PortfolioPosition, Enum>, CashFlow>();
        }

        public void AddDataTable(string tableName, List<ParamDescriptor> columnTitles)
        {
            List<DataColumn> columns = new List<DataColumn>(columnTitles.Count);

            foreach (var x in columnTitles)
            {
                DataColumn dataColumn = null;

                if (x.ParamType == ParamType.DateTime)
                    dataColumn = new DataColumn(x.Ident, typeof(DateTime));
                if (x.ParamType == ParamType.Decimal)
                    dataColumn = new DataColumn(x.Ident, typeof(decimal));
                if (x.ParamType == ParamType.Int)
                    dataColumn = new DataColumn(x.Ident, typeof(int));
                if (x.ParamType == ParamType.String)
                    dataColumn = new DataColumn(x.Ident, typeof(string));

                columns.Add(dataColumn);
            }
            DataTable dataTable = new DataTable(tableName);
            dataTable.Columns.AddRange(columns.ToArray());

            _dataTables.Add(dataTable);
        }

        public void AddRow(string tableName, params object[] values)
        {
            DataTable dataTable = null;
            foreach (var t in _dataTables)
            {
                if (t.TableName == tableName)
                {
                    dataTable = t;
                    break;
                }
            }

            DataRow dataRow = dataTable.NewRow();

            for (int i = 0; i < values.Length; ++i)
            {
                dataRow[dataTable.Columns[i]] = values[i];
            }

            dataTable.Rows.Add(dataRow);
        }

        public void Add(PortfolioPosition position, Enum enumeration, ScalarDate scalar)
        {
            KeyValuePair<PortfolioPosition, Enum> key = new KeyValuePair<PortfolioPosition, Enum>(position, enumeration);
            if (_dates.ContainsKey(key))
                _dates[key] = scalar;
            else
                _dates.Add(key, scalar);
        }

        public void Add(PortfolioPosition position, Enum enumeration, ScalarEnum scalar)
        {
            KeyValuePair<PortfolioPosition, Enum> key = new KeyValuePair<PortfolioPosition, Enum>(position, enumeration);
            if (_enumerations.ContainsKey(key))
                _enumerations[key] = scalar;
            else
                _enumerations.Add(key, scalar);
        }

        public void Add(PortfolioPosition position, Enum enumeration, ScalarNum scalar)
        {
            KeyValuePair<PortfolioPosition, Enum> key = new KeyValuePair<PortfolioPosition, Enum>(position, enumeration);
            if (_numbers.ContainsKey(key))
                _numbers[key] = scalar;
            else
                _numbers.Add(key, scalar);
        }

        public void Add(PortfolioPosition position, Enum enumeration, ScalarStr scalar)
        {
            KeyValuePair<PortfolioPosition, Enum> key = new KeyValuePair<PortfolioPosition, Enum>(position, enumeration);
            if(_strings.ContainsKey(key))
                _strings[key] = scalar;
            else
                _strings.Add(key, scalar);
        }

        public void Add(PortfolioPosition position, TimeSeries timeSeries)
        {
            _timeseries.Add(position, timeSeries);
        }

        public void Add(PortfolioPosition position, Enum enumeration, CashFlow cashFlow)
        {
            KeyValuePair<PortfolioPosition, Enum> key = new KeyValuePair<PortfolioPosition, Enum>(position, enumeration);
            _cashFlows.Add(key, cashFlow);
        }

        public List<DataTable> DataTable { get { return _dataTables; } set { _dataTables = value; } }
        public Dictionary<KeyValuePair<PortfolioPosition, Enum>, ScalarDate> Dates { get { return _dates; } }
        public Dictionary<KeyValuePair<PortfolioPosition, Enum>, ScalarEnum> Enumerations { get { return _enumerations; } }
        public Dictionary<KeyValuePair<PortfolioPosition, Enum>, ScalarNum> Numbers { get { return _numbers; } }
        public Dictionary<KeyValuePair<PortfolioPosition, Enum>, ScalarStr> Strings { get { return _strings; } }
        public Dictionary<PortfolioPosition, TimeSeries> TimeSeries { get { return _timeseries; } }

        public Dictionary<KeyValuePair<PortfolioPosition, Enum>, CashFlow> CashFlows { get { return _cashFlows;  } }
    }
}
