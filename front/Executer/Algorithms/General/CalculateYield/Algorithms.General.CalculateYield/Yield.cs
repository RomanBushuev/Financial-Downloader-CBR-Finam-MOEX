using Core.Mir;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Mir;
using Core.Mir.BaseTypes;
using Core.Mir.Enumerations;
using Core.Mir.Interfaces;

namespace Algorithms.General.CalculateYield
{
    public class Yield : CalculationOneData
    {
        public override bool Run()
        {
            var portfolio = Environment.Market.GetAllPositions();
            Dictionary<PortfolioPosition, TimeSeries> values =
                new Dictionary<PortfolioPosition, TimeSeries>();

            foreach(var x in portfolio)
            {
                var t = Environment.Market.GetTimeSeries(x,
                    TimeSeriesAttribute.Close,
                    DateTime.MinValue,
                    ReportDate);

                if (t == null || t.Series.Count < 2 || !t.Series.ContainsKey(ReportDate))
                    continue;

                Currencies currency = Environment.Market.Get<Currencies>(x,
                    ScalarAttribute.Currency,
                    ReportDate,
                    Currencies.Default);

                if (currency != Currencies.Default && currency != Currencies.RUB)
                {
                    var currencySeries = Environment.Market.GetTimeSeries(currency,
                        TimeSeriesAttribute.Close,
                        ReportDate,
                        ReportDate);

                    if (currencySeries == null || currencySeries.Series.Count == 0)
                        continue;
                }

                values.Add(x, t);
            }

            Dictionary<PortfolioPosition, TimeSeries> calculatedYield =
                new Dictionary<PortfolioPosition, TimeSeries>(portfolio.Count);
            
            foreach(var x in values)
            {
                decimal nominal = x.Value.Series.Select(z => z.Value)
                    .Reverse()
                    .Skip(1)
                    .First();

                decimal currentPrice = x.Value.Series[ReportDate];

                Currencies currency = Environment.Market.Get<Currencies>(x.Key,
                    ScalarAttribute.Currency,
                    ReportDate,
                    Currencies.Default);

                decimal result = decimal.Zero;
                try
                {
                    if (currency != Currencies.RUB && currency!= Currencies.USD)
                    {
                        decimal scala = Environment.Market.GetTimeSeries(currency, TimeSeriesAttribute.Close).Series[ReportDate];
                        result = (currentPrice - nominal) / nominal;
                    }
                    else
                    {
                        result = (currentPrice - nominal) / nominal;
                    }
                }
                catch
                {

                }

                if (result == decimal.Zero)
                    continue;

                Dictionary<DateTime, decimal> timeSeries = new Dictionary<DateTime, decimal>()
                {
                    { ReportDate, result }
                };
                
                _resultSet.Add(x.Key, new TimeSeries(timeSeries, TimeSeriesAttribute.Yield));
            }

            return true;
        }
    }
}
