using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Mir;
using Core.Mir.BaseTypes;
using Core.Mir.Enumerations;
using Core.Mir.Interfaces;

namespace ConvertData
{
    public class Convert : CalculationOneData
    {
        public override bool Run()
        {
            _resultSet = new ResultSet();

            var result = Environment.Market.GetAllData();

            foreach(var x in result)
            {
                if(x.Value.GetType() == typeof(ScalarDate))
                {
                    _resultSet.Add(x.Key.Key, x.Key.Value, (ScalarDate)x.Value);
                    continue;
                }
                if(x.Value.GetType() == typeof(ScalarEnum))
                {
                    _resultSet.Add(x.Key.Key, x.Key.Value, (ScalarEnum)x.Value);
                    continue;
                }
                if(x.Value.GetType() == typeof(ScalarNum))
                {
                    _resultSet.Add(x.Key.Key, x.Key.Value, (ScalarNum)x.Value);
                    continue;
                }
                if(x.Value.GetType() == typeof(ScalarStr))
                {
                    _resultSet.Add(x.Key.Key, x.Key.Value, (ScalarStr)x.Value);
                    continue;
                }
                if(x.Value.GetType() == typeof(TimeSeries))
                {
                    _resultSet.Add(x.Key.Key, (TimeSeries)x.Value);
                    //_resultSet.Add(x.Key.Key, (TimeSeries)x.Value);
                    continue;
                }

                if(x.Value.GetType() == typeof(CashFlow))
                {
                    _resultSet.Add(x.Key.Key, x.Key.Value, (CashFlow)x.Value);
                    continue;
                }
            }

            return true;
        }
    }
}
