using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Mir.BaseTypes;
using Core.Mir.Enumerations;
using Core.Mir.Interfaces;
using Core.Mir;

namespace CalculateVolat
{
    public class Volat : CalculationOneData
    {
        public const string WINDOW = "Окно";
        public const string MINIMAL_AMOUT = "Минимальное_кол-во_данных";
        private string RESULT_TABLE = "результат";
        private string IDENT_COLUMN = "IDENT";
        private string VOLAT_COLUMN = "VOLAT";
        private int _window;
        private int _minimalAmount;
        
        public Volat()
        {
            _resultSet.AddDataTable(RESULT_TABLE, new List<ParamDescriptor>()
                {
                    new ParamDescriptor()
                    {
                        Ident = IDENT_COLUMN,
                        Description = IDENT_COLUMN,
                        ParamType = ParamType.String,
                        Value = string.Empty
                    },
                    new ParamDescriptor()
                    {
                        Ident = VOLAT_COLUMN,
                        Description = VOLAT_COLUMN,
                        ParamType = ParamType.Decimal,
                        Value = decimal.Zero
                    }
                });
            GetParams();
        }

        public override List<ParamDescriptor> GetParams()
        {
            _paramDescriptors.Clear();
            _paramDescriptors.Add(new ParamDescriptor()
                {
                    Ident  = WINDOW,
                    Description = "Окно в рамках которого рассчитывается волатильность",
                    ParamType = ParamType.Int,
                    Value = 5
                });

            _paramDescriptors.Add(new ParamDescriptor()
                {
                    Ident = MINIMAL_AMOUT,
                    Description = "Минимальное кол-во значений",
                    ParamType = ParamType.Int,
                    Value = 30
                });

            return _paramDescriptors;
        }

        public decimal FindMaxVolat(decimal[] array)
        {
            decimal volat = decimal.Zero;
            for(int i = 0; i< array.Length/_window;++i)
            {
                int skipElements = i * _window;
                var temp = array.Skip(skipElements).Take(_window);
                decimal pmax = temp.Max();
                decimal pmin = temp.Min();
                decimal tempVolat = (pmax - pmin) / pmax;
                if (tempVolat > volat)
                    volat = tempVolat;
            }
            return volat;
        }

        public override bool Run()
        {
            _window = Param<int>(WINDOW);
            _minimalAmount = Param<int>(MINIMAL_AMOUT);

            var portfolio = Environment.Market.GetAllPositions();
            Dictionary<PortfolioPosition, TimeSeries> values = new Dictionary<PortfolioPosition, TimeSeries>();
            
            foreach(var x in portfolio)
            {
                var t = Environment.Market.GetTimeSeries(x, 
                    TimeSeriesAttribute.Close,
                    DateTime.MinValue,
                    ReportDate);

                if (t == null || t.Series.Count <= _minimalAmount)
                    continue;
                values.Add(x, t);
            }

            foreach(var x in values)
            {
                var t = x.Value.Series.Select(z => z.Value)
                    .Reverse()
                    .Take((x.Value.Series.Count / _window) * _window)
                    .Take(_minimalAmount)
                    .ToArray();

                var zz = FindMaxVolat(t);
                Dictionary<DateTime, decimal> dictionary = 
                    new Dictionary<DateTime,decimal>()
                    {
                        {ReportDate, zz}
                    };
                TimeSeries timeSeries = new TimeSeries(dictionary, TimeSeriesAttribute.Volat);
                _resultSet.Add(x.Key, timeSeries);
                _resultSet.AddRow(RESULT_TABLE, x.Key.Ident, zz);
            }

            return true;
        }
    }
}
