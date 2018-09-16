using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core.Mir.BaseTypes;
using Core.Mir.Enumerations;
using Core.Mir.Interfaces;
using DataBaseLink;
using DataProvider.Input.MirReader;
using System.Collections.Generic;
using System.Linq;

namespace Test.DataProvider.MirProvider
{
    [TestClass]
    public class TestMirReader
    {
        string connection = "Host = localhost; Username =postgres; Password =roman; Database =MIR";
        BalancePosition position = new BalancePosition("CH0205819433",
            FinType.Bond);

        public IMarketProvider TestConnection()
        {
            Mapping mapping = new Mapping();
            
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
                {Provider.QUOTE, "MOEX"},
                {Provider.SCALAR, "MOEX"},
            };

            Provider provider = new Provider(connection, mapping);
            provider.SetParams(sq);
            return provider;
        }

        [TestMethod]
        public void TestScalarStr()
        {
            IMarketProvider marketProvider = TestConnection();
            var result = marketProvider.GetScalarStr(position, ScalarAttribute.Name);
            for(int i =0;i< 100;++i)
            {
                marketProvider.GetScalarStr(position, ScalarAttribute.Name);
            }
        }

        [TestMethod]
        public void TestScalarNum()
        {
            IMarketProvider marketProvider = TestConnection();
            var result = marketProvider.GetScalarNum(position, ScalarAttribute.Nominal);
            for(int i =0;i< 100; ++i)
            {
                marketProvider.GetScalarNum(position, ScalarAttribute.Nominal);
            }
        }

        [TestMethod]
        public void TestGetAllFinInstruments()
        {
            IMarketProvider marketProvider = TestConnection();
            var result = marketProvider.GetAllPositions();
            Assert.IsTrue(result != null && result.Count != 0 && result.Count(z=>z.FinType!= FinType.Default)!=0);
        }

        [TestMethod]
        public void TestGetOnlyBonds()
        {
            IMarketProvider marketProvider = TestConnection();
            var result = marketProvider.GetAllPositions(FinType.Bond);
            Assert.IsTrue(result != null && result.Count != 0 && result.Count(z => z.FinType != FinType.Default) != 0);
        }

        [TestMethod]
        public void TestScalarDateTime()
        {
            IMarketProvider marketProvider = TestConnection();
            var result = marketProvider.GetScalarDate(position, ScalarAttribute.MatDate);
            for(int i =0;i< 100;++i)
            {
                marketProvider.GetScalarDate(position, ScalarAttribute.MatDate);
            }
        }

        [TestMethod]
        public void TestScalarEnum()
        {
            IMarketProvider marketProvider = TestConnection();
            var result = marketProvider.GetScalarEnum(position, ScalarAttribute.Currency);
            for (int i = 0; i < 100; ++i)
            {
                marketProvider.GetScalarDate(position, ScalarAttribute.MatDate);
            }
        }

        [TestMethod]
        public void TestQuotes()
        {
            string ident = "CH0317921671";
            position.Ident = ident;
            IMarketProvider marketProvider = TestConnection();
            var result = marketProvider.GetTimeSeries(position, TimeSeriesAttribute.Close);
        }

        [TestMethod]
        public void TestGetEnum()
        {
            IMarketProvider marketProvider = TestConnection();
            var result = marketProvider.Get<Currencies>(position, ScalarAttribute.Currency, DateTime.Today, Currencies.Default);
        }
    }
}
