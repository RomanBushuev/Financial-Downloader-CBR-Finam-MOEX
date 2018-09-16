using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using DataProvider.Input.MirReader;
using DataProvider.Input.CombinedProvider;
using Core.Mir;
using Core.Mir.BaseTypes;
using Core.Mir.Enumerations;
using Core.Mir.Interfaces;
using System.Collections.Generic;
using Algorithms.General.CalculateVar;
using System.Configuration;
using DataProvider.Output.Excel;
using DataProvider.Output.Mir;

namespace Test.Algorithms.General.TestVar
{
    [TestClass]
    public class TestVar
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
        public void TestCalculateVar()
        {
            IMarketProvider moex = GetMoex();
            IMarketProvider cbr = GetCbr();

            DataProvider.Input.CombinedProvider.Provider provider =
                new DataProvider.Input.CombinedProvider.Provider(new List<IMarketProvider>()
                    {
                        moex,
                        cbr
                    });

            Core.Mir.Environment environment =
                new Core.Mir.Environment();
            environment.Market = new MarketData(provider);

            CalculationOneData calculation = new Var();
            calculation.ReportDate = new DateTime(2018, 05, 04);
            Dictionary<string, object> pars = new Dictionary<string, object>()
            {
                {Var.WINDOW, 5},
                {Var.QUANTILE, 0.9m},
            };
            calculation.SetParams(pars);
            calculation.Environment = environment;
            bool result = false;
            try
            {
                result = calculation.Run();
            }
            catch
            {
                result = false;
            }
            #region Settings Output
            #region Выгрузка данных в мир
            if (result)
            {
                #region Settings outputProvider
                string mirConnection = ConfigurationManager.AppSettings["mirconnection"];
                DataProvider.Output.Mir.Mapping outMapping = new DataProvider.Output.Mir.Mapping();

                outMapping.AI = new Dictionary<Enum, string>()
                    {
                        #region Финансовые инструменты
                        {FinType.Equity, "12"},
                        {FinType.Bond, "13"},
                        {FinType.Fund, "14"},
                        {FinType.Certificate, "15"},
                        {FinType.DepositaryReceipt, "16"},
                        {FinType.FxRate, "20"}, 
                        {TimeSeriesAttribute.Var, "21675"},
                        #endregion
                    };

                outMapping.TKE = new Dictionary<KeyValuePair<Type, string>, Enum>()
                    {
                        #region Валюта
                        {new KeyValuePair<Type,string>(typeof(Currencies),"CHF"), Currencies.CHF},
                        {new KeyValuePair<Type,string>(typeof(Currencies),"EUR"), Currencies.EUR},
                        {new KeyValuePair<Type,string>(typeof(Currencies),"GBP"), Currencies.GBP},
                        {new KeyValuePair<Type,string>(typeof(Currencies),"RUB"), Currencies.RUB},
                        {new KeyValuePair<Type,string>(typeof(Currencies),"RUR"), Currencies.RUB},
                        {new KeyValuePair<Type,string>(typeof(Currencies),"USD"), Currencies.USD},
                        #endregion
                    };


                outMapping.ET = new Dictionary<Enum, Type>()
                    {
                        {ScalarAttribute.DetailedType, typeof(FinTypeDetailedLevel)},
                        {ScalarAttribute.Currency, typeof(Currencies)},
                        {ScalarAttribute.FinType, typeof(FinType)},
                    };

                mirConnection = "Host = localhost; Username =postgres; Password =roman; Database =MIR";
                using (var outProvider = new DataProvider.Output.Mir.Provider(mirConnection, outMapping))
                {
                    outProvider.SetParams(new Dictionary<string, object>()
                        {
                            {DataProvider.Output.Mir.Provider.SCALAR, "CALCULATED"},
                            {DataProvider.Output.Mir.Provider.QUOTE, "CALCULATED"}
                        });

                    outProvider.Save(calculation.returnResultSet());
                }


                
                #endregion
            }
            #endregion

            #region Выгрузка данных в Excel 
            string excelPath = @"C:\Users\RomanBushuev\YandexDisk\MarketData\Reports\Var\";
            string excelFileName = string.Format("{0}.xlsx",
                calculation.ReportDate.ToString("yyyy.MM.dd"));

            DataProvider.Output.Excel.Provider excelProvider =
                new DataProvider.Output.Excel.Provider(excelPath + excelFileName);

            excelProvider.Save(calculation.returnResultSet());
            #endregion
            #endregion
        }
    }
}
