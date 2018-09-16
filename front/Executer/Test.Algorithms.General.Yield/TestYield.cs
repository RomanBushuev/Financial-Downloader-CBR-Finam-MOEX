using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core.Mir.Interfaces;
using System.Collections.Generic;
using Core.Mir;
using Core.Mir.BaseTypes;
using Core.Mir.Enumerations;
using Algorithms.General.CalculateYield;
using YSQ.core.Quotes;

namespace Test.Algorithms.General.TestYield
{
    [TestClass]
    public class TestYield
    {
        public IMarketProvider GetMoex()
        {
            string connection = "Host = localhost; Username =postgres; Password =roman; Database =MIR";
            #region mapping mir_moex
            DataProvider.Input.MirReader.Mapping mapping = new DataProvider.Input.MirReader.Mapping();

            mapping.AI = new Dictionary<Enum, string>()
            {
                #region Финансовые инструменты
                {FinType.Equity, "12"},
                {FinType.Bond, "13"},
                {FinType.Fund, "14"},
                {FinType.Certificate, "15"},
                {FinType.DepositaryReceipt, "16"},
                {FinType.Default, "20"},
                {TimeSeriesAttribute.Close, "3"},
                {ScalarAttribute.Name, "4"},
                {ScalarAttribute.Nominal, "9"},
                {ScalarAttribute.MatDate, "11"},
                {ScalarAttribute.Currency, "8"},
                #endregion
            };

            mapping.TKE = new Dictionary<KeyValuePair<Type, string>, Enum>()
            {
                #region Валюта
                {new KeyValuePair<Type,string>(typeof(Currencies),"CHF"), Currencies.CHF},
                {new KeyValuePair<Type,string>(typeof(Currencies),"EUR"), Currencies.EUR},
                {new KeyValuePair<Type,string>(typeof(Currencies),"GBP"), Currencies.GBP},
                {new KeyValuePair<Type,string>(typeof(Currencies),"RUB"), Currencies.RUB},
                {new KeyValuePair<Type,string>(typeof(Currencies),"RUR"), Currencies.RUB},
                {new KeyValuePair<Type,string>(typeof(Currencies),"USD"), Currencies.USD},
                #endregion

                #region Фин.инструменты
                {new KeyValuePair<Type, string>(typeof(FinType), "CERTIFICATE"), FinType.Certificate},
                {new KeyValuePair<Type, string>(typeof(FinType), "DEPOSITARY RECEIPT"), FinType.DepositaryReceipt},
                {new KeyValuePair<Type, string>(typeof(FinType), "BOND"), FinType.Bond},
                {new KeyValuePair<Type, string>(typeof(FinType), "EQUITY"), FinType.Equity},
                {new KeyValuePair<Type, string>(typeof(FinType), "FUND"), FinType.Fund},
                {new KeyValuePair<Type, string>(typeof(FinType), "FX_RATE"), FinType.FxRate},
                {new KeyValuePair<Type, string>(typeof(FinType), "PERCENT_CURVE"), FinType.PercentCurve},
                #endregion
            };


            mapping.ET = new Dictionary<Enum, Type>()
            {
                {ScalarAttribute.DetailedType, typeof(FinTypeDetailedLevel)},
                {ScalarAttribute.Currency, typeof(Currencies)},
                {ScalarAttribute.FinType, typeof(FinType)},
            };

            Dictionary<string, object> sq = new Dictionary<string, object>()
            {
                {DataProvider.Input.MirReader.Provider.QUOTE, "MOEX"},
                {DataProvider.Input.MirReader.Provider.SCALAR, "MOEX"},
            };
            #endregion

            DataProvider.Input.MirReader.Provider MoexProvider =
                new DataProvider.Input.MirReader.Provider(connection, mapping);
            MoexProvider.SetParams(sq);

            return MoexProvider;
        }

        public IMarketProvider GetCbr()
        {
            string connection = "Host = localhost; Username =postgres; Password =roman; Database =MIR";
            #region mapping mir_cbr
            DataProvider.Input.MirReader.Mapping mapping2 = new DataProvider.Input.MirReader.Mapping();

            mapping2.AI = new Dictionary<Enum, string>()
            {
                #region Финансовые инструменты
                {FinType.Equity, "12"},
                {FinType.Bond, "13"},
                {FinType.Fund, "14"},
                {FinType.Certificate, "15"},
                {FinType.DepositaryReceipt, "16"},
                {FinType.Default, "20"},
                {TimeSeriesAttribute.Close, "3"},
                {ScalarAttribute.Name, "4"},
                {ScalarAttribute.Nominal, "9"},
                {ScalarAttribute.MatDate, "11"},
                {ScalarAttribute.Currency, "8"},
                {Currencies.CHF, "CHF"},
                {Currencies.EUR, "EUR"},
                {Currencies.USD, "USD"}
                #endregion
            };

            mapping2.TKE = new Dictionary<KeyValuePair<Type, string>, Enum>()
            {
                #region Валюта
                {new KeyValuePair<Type,string>(typeof(Currencies),"CHF"), Currencies.CHF},
                {new KeyValuePair<Type,string>(typeof(Currencies),"EUR"), Currencies.EUR},
                {new KeyValuePair<Type,string>(typeof(Currencies),"GBP"), Currencies.GBP},
                {new KeyValuePair<Type,string>(typeof(Currencies),"RUB"), Currencies.RUB},
                {new KeyValuePair<Type,string>(typeof(Currencies),"RUR"), Currencies.RUB},
                {new KeyValuePair<Type,string>(typeof(Currencies),"USD"), Currencies.USD},
                #endregion

                #region Фин.инструменты
                {new KeyValuePair<Type, string>(typeof(FinType), "CERTIFICATE"), FinType.Certificate},
                {new KeyValuePair<Type, string>(typeof(FinType), "DEPOSITARY RECEIPT"), FinType.DepositaryReceipt},
                {new KeyValuePair<Type, string>(typeof(FinType), "BOND"), FinType.Bond},
                {new KeyValuePair<Type, string>(typeof(FinType), "EQUITY"), FinType.Equity},
                {new KeyValuePair<Type, string>(typeof(FinType), "FUND"), FinType.Fund},
                {new KeyValuePair<Type, string>(typeof(FinType), "FX_RATE"), FinType.FxRate},
                {new KeyValuePair<Type, string>(typeof(FinType), "PERCENT_CURVE"), FinType.PercentCurve},
                #endregion
            };


            mapping2.ET = new Dictionary<Enum, Type>()
            {
                {ScalarAttribute.DetailedType, typeof(FinTypeDetailedLevel)},
                {ScalarAttribute.Currency, typeof(Currencies)},
                {ScalarAttribute.FinType, typeof(FinType)},
            };

            Dictionary<string, object> sq2 = new Dictionary<string, object>()
            {
                {DataProvider.Input.MirReader.Provider.QUOTE, "CBR"},
                {DataProvider.Input.MirReader.Provider.SCALAR, "CBR"},
            };
            #endregion


            DataProvider.Input.MirReader.Provider CbrProvider =
                new DataProvider.Input.MirReader.Provider(connection, mapping2);
            CbrProvider.SetParams(sq2);

            return CbrProvider;
        }


        [TestMethod]
        public void Yield_()
        {
            IMarketProvider moex = GetMoex();
            IMarketProvider cbr = GetCbr();

            DataProvider.Input.CombinedProvider.Provider provider =
                new DataProvider.Input.CombinedProvider.Provider(
                    new List<IMarketProvider>()
                    {
                        moex,
                        cbr
                    });

            Core.Mir.Environment environment =
                new Core.Mir.Environment();
            environment.Market = new Core.Mir.MarketData(provider);

            CalculationOneData calculation =
                new Yield();

            calculation.Environment = environment;
            calculation.ReportDate = new DateTime(2018, 05, 14);
            calculation.Run();

            foreach(var x in calculation.returnResultSet().TimeSeries)
            {
                if(x.Value.Series.ContainsKey(calculation.ReportDate))
                {
                    string message = string.Format("{0}\t{1}", x.Key.Ident, x.Value.Series[calculation.ReportDate]);
                    Console.WriteLine(message);
                }
            }
        }

        [TestMethod]
        public void FixedBond()
        {
            DateTime startDate = new DateTime(2008, 02, 15);
            DateTime endDate = new DateTime(2016, 10, 15);
            double percentRate = 5.75;
            double price = 95.04287;
            double redemptio = 100;

            var result =Excel.FinancialFunctions.Financial.Yield(startDate, endDate, percentRate,
                price, redemptio, Excel.FinancialFunctions.Frequency.SemiAnnual, Excel.FinancialFunctions.DayCountBasis.Actual360);
            Console.WriteLine(result);
            var service = new QuoteService();
            var symbols = new[] { "GOOG", "XCS.TO" };
            var quotes = service.Quote(symbols).Return(QuoteReturnParameter.Symbol, QuoteReturnParameter.LatestTradePrice);
            foreach(var zz in quotes)
            {
                Console.WriteLine(zz);
            }
        }
    }
}
