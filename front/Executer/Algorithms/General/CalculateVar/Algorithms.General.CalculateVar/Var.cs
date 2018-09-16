using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Mir;
using Core.Mir.BaseTypes;
using Core.Mir.Enumerations;
using Core.Mir.Interfaces;

namespace Algorithms.General.CalculateVar
{
    public class Var : CalculationOneData
    {
        public const string WINDOW = "Окно";
        public const string QUANTILE = "Квантиль";

        private string RESULT_TABLE = "Результат общий";
        private string RESULT_EQUITY = "Результат по акциям";
        private string RESULT_BONDS = "Результат по облигациям";
        private string RESULT_FUND = "Результат по фондам";
        private string RESULT_CERTIFICATE = "Результат по сертификатам";
        private string RESULT_DR = "Результат по депозитарным распискам";
        private string IDENT_COLUMN = "IDENT";
        private string VOLAT_COLUMN = "VAR_RUB";

        private int _window;
        private decimal _quantile;

        public Var()
        {
            #region Результат общий
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
            #endregion 
            #region Результат по акциям
            _resultSet.AddDataTable(RESULT_EQUITY, new List<ParamDescriptor>()
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
            #endregion
            #region Результат по облигациям
            _resultSet.AddDataTable(RESULT_BONDS, new List<ParamDescriptor>()
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
            #endregion
            #region Результат по фондам
            _resultSet.AddDataTable(RESULT_FUND, new List<ParamDescriptor>()
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
            #endregion
            #region Результат по сертификатам
            _resultSet.AddDataTable(RESULT_CERTIFICATE, new List<ParamDescriptor>()
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
            #endregion
            #region Результат по депозитарным распискам
            _resultSet.AddDataTable(RESULT_DR, new List<ParamDescriptor>()
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
            #endregion
            GetParams();
        }

        public override List<ParamDescriptor> GetParams()
        {
            _paramDescriptors.Clear();
            _paramDescriptors.Add(new ParamDescriptor()
            {
                Ident = WINDOW,
                Description = "Окно в рамках которого рассчитывается волатильность",
                ParamType = ParamType.Int,
                Value = 10
            });

            _paramDescriptors.Add(new ParamDescriptor()
            {
                Ident = QUANTILE,
                Description = "Минимальное кол-во значений",
                ParamType = ParamType.Decimal,
                Value = 0.9
            });

            return _paramDescriptors;
        } 

        public int Quantile(List<decimal> array)
        {
            int index = (int)Math.Ceiling(array.Count * _quantile) - 1;
            if (index < 0)
                return 0;
            if (index >= array.Count)
                return array.Count - 1;
            return index;
        }

        public override bool Run()
        {
            _window = Param<int>(WINDOW);
            _quantile = Param<decimal>(QUANTILE);

            List<FinType> finTypes = new List<FinType>()
            {
                FinType.Equity,
                FinType.Bond,
                FinType.Fund,
                FinType.Certificate,
                FinType.DepositaryReceipt
            };

            var portfolio = Environment.Market.GetAllPositions(finTypes);
            Dictionary<PortfolioPosition, TimeSeries> values = new Dictionary<PortfolioPosition, TimeSeries>();

            foreach (var x in portfolio)
            {
                var t = Environment.Market.GetTimeSeries(x,
                    TimeSeriesAttribute.Close,
                    DateTime.MinValue,
                    ReportDate);

                if (t == null || t.Series.Count <= _window || !t.Series.ContainsKey(ReportDate))
                    continue;
                Currencies currency = Environment.Market.Get<Currencies>(x,
                    ScalarAttribute.Currency,
                    ReportDate,
                    Currencies.Default);

                if (currency == Currencies.Default)
                    continue;

                values.Add(x, t);
            }

            Dictionary<DateTime, decimal> portfolioVar = new Dictionary<DateTime, decimal>();

            #region  Расчет всех бумаг 
            foreach (var x in values.Keys)
            {
                //смотрим валюту, если default, то выкидываем, нет берем курс
                Currencies currency = Environment.Market.Get<Currencies>(x,
                    ScalarAttribute.Currency,
                    ReportDate,
                    Currencies.Default);

                if (currency == Currencies.Default)
                    continue;

                if(currency != Currencies.RUB)
                {
                    TimeSeries timeseries = Environment.Market.GetTimeSeries(currency, TimeSeriesAttribute.Close, ReportDate, ReportDate);
                    if (timeseries == null)
                        continue;
                }

                var t = values[x].Series.Select(z => z)
                    .Reverse()
                    .Take(_window)
                    .ToArray();

                TimeSeries timeSeries = null;

                if (currency != Currencies.RUB)
                {
                    timeSeries = Environment.Market.GetTimeSeries(currency,
                        TimeSeriesAttribute.Close,
                        t.Select(z => z.Key).Min(),
                        t.Select(z => z.Key).Max());
                }
                else
                {
                    Dictionary<DateTime, decimal> decimals = new Dictionary<DateTime, decimal>();
                    for(DateTime romanz = t.Select(z => z.Key).Min();
                        romanz <= t.Select(z => z.Key).Max();)
                    {
                        if (!t.Select(roz => roz.Key).Contains(romanz))
                        {
                            romanz = romanz.AddDays(1);
                            continue;
                        }

                        decimals.Add(romanz, decimal.One);
                        romanz = romanz.AddDays(1);
                    }

                    timeSeries = new TimeSeries(decimals);
                }
                List<decimal> valuesByCurrencies = new List<decimal>(t.Count());
                decimal result = decimal.Zero;
                foreach(var z in t)
                {
                    if(timeSeries.Contains(z.Key))
                    {
                        result = timeSeries.Series[z.Key] * z.Value;
                        valuesByCurrencies.Add(result);

                        #region портфель
                        if (portfolioVar.ContainsKey(z.Key))
                        {
                            result = timeSeries.Series[z.Key] * z.Value;
                            portfolioVar[z.Key] += result;
                        }
                        else
                        {
                            result = timeSeries.Series[z.Key] * z.Value;
                            portfolioVar.Add(z.Key, result);
                        }
                        #endregion
                    }
                }

                decimal var = valuesByCurrencies.OrderBy(z => z).ElementAt(Quantile(valuesByCurrencies));
                Dictionary<DateTime, decimal> varTenDay = new Dictionary<DateTime, decimal>()
                {
                    { ReportDate, var }
                };

                if (!_resultSet.TimeSeries.ContainsKey(x))
                    _resultSet.Add(x, new TimeSeries(varTenDay, TimeSeriesAttribute.Var));
                //в таблицу
                _resultSet.AddRow(RESULT_TABLE, x.Ident, var);
            }

            #endregion

            string portfolioName = "Портфель";
            decimal varP = portfolioVar.Select(z => z.Value)
                .OrderBy(z => z)
                .ElementAt(Quantile(portfolioVar.Select(z => z.Value).ToList()));
            _resultSet.AddRow(RESULT_TABLE, portfolioName, varP);

            #region Расчет по различным портфелям
            Dictionary<FinType, string> dictionary = new Dictionary<FinType, string>()
            {
                { FinType.Equity, RESULT_EQUITY},
                { FinType.Bond, RESULT_BONDS},
                { FinType.Fund, RESULT_FUND},
                { FinType.Certificate, RESULT_CERTIFICATE},
                { FinType.DepositaryReceipt, RESULT_DR},
            };

            Dictionary<FinType, string> titles = new Dictionary<FinType,string>()
            {
                { FinType.Equity, "Портфель Акции"},
                { FinType.Bond, "Портфель Облигации"},
                { FinType.Fund, "Портфель Фонд"},
                { FinType.Certificate, "Портфель Сертификат"},
                { FinType.DepositaryReceipt, "Портфель Депозитарная расписка"},
            };

            foreach(var x in finTypes)
            {
                string portfolioTitle = dictionary[x];
                portfolioVar = new Dictionary<DateTime, decimal>();

                #region Расчет для отдельного портфеля
                foreach (var z in values.Where(t=>t.Key.FinType == x))
                {
                    Currencies currency = Environment.Market.Get<Currencies>(z.Key,
                        ScalarAttribute.Currency,
                        ReportDate,
                        Currencies.Default);

                    if (currency == Currencies.Default)
                        continue;

                    if (currency != Currencies.RUB)
                    {
                        TimeSeries timeseries = Environment.Market.GetTimeSeries(currency, TimeSeriesAttribute.Close, ReportDate, ReportDate);
                        if (timeseries == null)
                            continue;
                    }

                    var k = values[z.Key].Series.Select(t => t)
                        .Reverse()
                        .Take(_window)
                        .ToArray();

                    TimeSeries timeSeries = null;

                    if (currency != Currencies.RUB)
                    {
                        timeSeries = Environment.Market.GetTimeSeries(currency,
                            TimeSeriesAttribute.Close,
                            k.Select(zz => zz.Key).Min(),
                            k.Select(zz => zz.Key).Max());
                    }
                    else
                    {
                        Dictionary<DateTime, decimal> decimals = new Dictionary<DateTime, decimal>();
                        for (DateTime romanz = k.Select(zz => zz.Key).Min();
                            romanz <= k.Select(zz => zz.Key).Max(); )
                        {
                            if (!k.Select(roz => roz.Key).Contains(romanz))
                            {
                                romanz = romanz.AddDays(1);
                                continue;
                            }

                            decimals.Add(romanz, decimal.One);
                            romanz = romanz.AddDays(1);
                        }

                        timeSeries = new TimeSeries(decimals);
                    }

                    List<decimal> valuesByCurrencies = new List<decimal>(k.Count());

                    decimal result = decimal.Zero;
                    foreach (var r in k)
                    {
                        if (timeSeries.Contains(r.Key))
                        {
                            #region портфель
                            if (portfolioVar.ContainsKey(r.Key))
                            {
                                result = timeSeries.Series[r.Key] * r.Value;
                                portfolioVar[r.Key] += result;
                            }
                            else
                            {
                                result = timeSeries.Series[r.Key] * r.Value;
                                portfolioVar.Add(r.Key, result);
                            }
                            #endregion
                        }
                    }
                }
                #endregion

                //перенести данный финюинструмент в другую структуру
                foreach(var roman in _resultSet.TimeSeries.Where(rz=>rz.Key.FinType == x))
                {
                    _resultSet.AddRow(dictionary[x], roman.Key.Ident, roman.Value.Series.First().Value);
                }

                decimal varResult = portfolioVar.Select(z => z.Value)
                .OrderBy(z => z)
                .ElementAt(Quantile(portfolioVar.Select(z => z.Value).ToList()));
                _resultSet.AddRow(dictionary[x], titles[x], varResult);
                //расчет по всему портфелю 
            }
            #endregion

            return true;
        }
    }
}
