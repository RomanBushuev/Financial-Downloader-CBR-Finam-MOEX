using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

using Core.Mir.BaseTypes;
using Core.Mir.Enumerations;
using Core.Mir.Interfaces;

using DataProvider.Input.CsvReader;

namespace Test.DataProvider.CsvReader
{
    [TestClass]
    public class TestCsvReader
    {
        [TestMethod]
        public void ReadDataFromCSV()
        {
            string connection = @"C:\Users\RomanBushuev\YandexDisk\MarketData\MOEX\raw\2018.03.16\rates.csv";
            Mapping mapping = new Mapping();
            mapping.AI = new Dictionary<Enum, string>()
            {
                #region Финансовые инструменты
                {PositionAttribute.Ident, "ISIN" },
                {PositionAttribute.Type, "TYPENAME" },
                {ScalarAttribute.Currency, "FACEUNIT"},
                {ScalarAttribute.MatDate, "MATDATE"},
                {ScalarAttribute.Name, "NAME"},
                {ScalarAttribute.Nominal, "FACEVALUE"},
                {ScalarAttribute.SecId, "SECID"},
                {ScalarAttribute.ShortName, "SHORTNAME"},
                {ScalarAttribute.Size, "ISSUESIZE"},
                {TimeSeriesAttribute.Close, "PRICE"},
                #endregion
            };

            mapping.TKE = new Dictionary<KeyValuePair<Type, string>, Enum>()
            {
                #region Финансовые инструменты
                {new KeyValuePair<Type,string>(typeof(FinType),"Акция обыкновенная"), FinType.Equity},
                {new KeyValuePair<Type,string>(typeof(FinType),"Корпоративная облигация"), FinType.Bond},
                {new KeyValuePair<Type,string>(typeof(FinType),"ETF"), FinType.Fund},
                {new KeyValuePair<Type,string>(typeof(FinType),"Акция привилегированная"), FinType.Equity},
                {new KeyValuePair<Type,string>(typeof(FinType),"ОФЗ"), FinType.Bond},

                {new KeyValuePair<Type,string>(typeof(FinType),"Пай закрытого ПИФа"), FinType.Fund},
                {new KeyValuePair<Type,string>(typeof(FinType),"Пай интервального ПИФа"), FinType.Fund},
                {new KeyValuePair<Type,string>(typeof(FinType),"Региональная облигация"), FinType.Bond},
                {new KeyValuePair<Type,string>(typeof(FinType),"Пай открытого ПИФа"), FinType.Fund},
                {new KeyValuePair<Type,string>(typeof(FinType),"Облигация МФО"), FinType.Bond},

                {new KeyValuePair<Type,string>(typeof(FinType),"Биржевая облигация"), FinType.Bond},
                {new KeyValuePair<Type,string>(typeof(FinType),"Ипотечный сертификат"), FinType.Certificate},
                {new KeyValuePair<Type,string>(typeof(FinType),"Муниципальная облигация"), FinType.Bond},
                {new KeyValuePair<Type,string>(typeof(FinType),"Клиринговый сертификат участия"), FinType.Certificate},
                {new KeyValuePair<Type,string>(typeof(FinType),"Депозитарная расписка"), FinType.DepositaryReceipt},
                #endregion

                #region Детализированный уровень
                {new KeyValuePair<Type,string>(typeof(FinTypeDetailedLevel),"Акция обыкновенная"), FinTypeDetailedLevel.OrdinaryStock},
                {new KeyValuePair<Type,string>(typeof(FinTypeDetailedLevel),"Корпоративная облигация"), FinTypeDetailedLevel.CorporateBond},
                {new KeyValuePair<Type,string>(typeof(FinTypeDetailedLevel),"ETF"), FinTypeDetailedLevel.ETF},
                {new KeyValuePair<Type,string>(typeof(FinTypeDetailedLevel),"Акция привилегированная"), FinTypeDetailedLevel.PreferredStock},
                {new KeyValuePair<Type,string>(typeof(FinTypeDetailedLevel),"ОФЗ"), FinTypeDetailedLevel.FederalLoanBond},

                {new KeyValuePair<Type,string>(typeof(FinTypeDetailedLevel),"Пай закрытого ПИФа"), FinTypeDetailedLevel.PieOfClosedMutualFund},
                {new KeyValuePair<Type,string>(typeof(FinTypeDetailedLevel),"Пай интервального ПИФа"), FinTypeDetailedLevel.PieOfIntervalMutualFund},
                {new KeyValuePair<Type,string>(typeof(FinTypeDetailedLevel),"Региональная облигация"), FinTypeDetailedLevel.RegionalBond},
                {new KeyValuePair<Type,string>(typeof(FinTypeDetailedLevel),"Пай открытого ПИФа"), FinTypeDetailedLevel.PieOfOpenedMutualFund},
                {new KeyValuePair<Type,string>(typeof(FinTypeDetailedLevel),"Облигация МФО"), FinTypeDetailedLevel.MfoBond},

                {new KeyValuePair<Type,string>(typeof(FinTypeDetailedLevel),"Биржевая облигация"), FinTypeDetailedLevel.ExchangeTradedBond},
                {new KeyValuePair<Type,string>(typeof(FinTypeDetailedLevel),"Ипотечный сертификат"), FinTypeDetailedLevel.MortgageCertificate},
                {new KeyValuePair<Type,string>(typeof(FinTypeDetailedLevel),"Муниципальная облигация"), FinTypeDetailedLevel.MunicipalBond},
                {new KeyValuePair<Type,string>(typeof(FinTypeDetailedLevel),"Клиринговый сертификат участия"), FinTypeDetailedLevel.ClearingParticipationCertificate},
                {new KeyValuePair<Type,string>(typeof(FinTypeDetailedLevel),"Депозитарная расписка"), FinTypeDetailedLevel.DepositaryReceipt},
                #endregion

                #region Валюта
                {new KeyValuePair<Type,string>(typeof(Currencies),"CHF"), Currencies.CHF},
                {new KeyValuePair<Type,string>(typeof(Currencies),"EUR"), Currencies.EUR},
                {new KeyValuePair<Type,string>(typeof(Currencies),"GBP"), Currencies.GBP},
                {new KeyValuePair<Type,string>(typeof(Currencies),"RUB"), Currencies.RUB},
                {new KeyValuePair<Type,string>(typeof(Currencies),"RUR"), Currencies.RUB},
                {new KeyValuePair<Type,string>(typeof(Currencies),"USD"), Currencies.USD},
                #endregion
            };
           

            mapping.ET = new Dictionary<Enum, Type>()
            {
                {ScalarAttribute.DetailedType, typeof(FinTypeDetailedLevel)},
                {ScalarAttribute.Currency, typeof(Currencies)},
                {ScalarAttribute.FinType, typeof(FinType)},
            };

            Dictionary<ScalarAttribute, ParamType> sp = new Dictionary<ScalarAttribute, ParamType>()
            {
                {ScalarAttribute.Currency, ParamType.String},
                {ScalarAttribute.DetailedType, ParamType.String},
                {ScalarAttribute.FinType, ParamType.String},
                {ScalarAttribute.MatDate, ParamType.DateTime},
                {ScalarAttribute.Name, ParamType.String},
                {ScalarAttribute.Nominal, ParamType.Decimal},
                {ScalarAttribute.SecId, ParamType.String},
                {ScalarAttribute.ShortName, ParamType.String},
                {ScalarAttribute.Size, ParamType.Decimal},
            };


            List<KeyValuePair<string, Type>> pages = new List<KeyValuePair<string, Type>>()
            {
                new KeyValuePair<string,Type>("Table1", typeof(ScalarAttribute)),
                new KeyValuePair<string,Type>("Table1", typeof(TimeSeriesAttribute))
            };

            Provider provider = new Provider(connection,
                mapping,
                pages,
                sp,
                skipLines: 2);

            
            provider.Download();
            Console.WriteLine("done");
        }

        [TestMethod]
        public void ReadDataFromCurrencies()
        {
            string connection = @"C:\Users\RomanBushuev\YandexDisk\MarketData\CBR\raw\currencies\2018.03.18.csv";
            Mapping mapping = new Mapping();

            mapping.AI = new Dictionary<Enum, string>()
            {
                #region Финансовые инструменты
                {PositionAttribute.Ident, "VchCode" },
                {ScalarAttribute.Size, "Vnom" },
                {TimeSeriesAttribute.Close, "Vcurs" },
                {ScalarAttribute.Name, "Vname" },
                #endregion
            };

            mapping.ET = new Dictionary<Enum, Type>();

            Dictionary<ScalarAttribute, ParamType> sp = new Dictionary<ScalarAttribute, ParamType>()
            {
                {ScalarAttribute.Size, ParamType.Decimal},
                {ScalarAttribute.Name, ParamType.String},
            };

            List<KeyValuePair<string, Type>> pages = new List<KeyValuePair<string, Type>>()
            {
                new KeyValuePair<string,Type>("Table1", typeof(ScalarAttribute)),
                new KeyValuePair<string,Type>("Table1", typeof(TimeSeriesAttribute))
            };

            Provider provider = new Provider(connection,
                mapping,
                pages,
                sp);

            provider.Download();
            Console.WriteLine("done");
        }

        [TestMethod]
        public void ReadDataFromRuonia()
        {
            string connection = @"C:\Users\RomanBushuev\YandexDisk\MarketData\CBR\raw\ruonia\ruonia.csv";
            Mapping mapping = new Mapping();

            mapping.AI = new Dictionary<Enum, string>()
            {
                #region Финансовые инструменты
                {PositionAttribute.ActualDate, "Date"},
                {TimeSeriesAttribute.Close, "value"},
                #endregion
            };

            List<KeyValuePair<string, Type>> pages = new List<KeyValuePair<string, Type>>()
            {
                new KeyValuePair<string,Type>("Table1", typeof(TimeSeriesAttribute))
            };

             Dictionary<ScalarAttribute, ParamType> sp = new Dictionary<ScalarAttribute,ParamType>();

             Provider provider = new Provider(connection,
                 mapping,
                 pages,
                 sp,
                 ident:"RUONIA");

             provider.Download();
             Console.WriteLine("done");
        }

        [TestMethod]
        public void ReadDataFromDebt_ZCC_RUB()
        {
            string connection = @"C:\Users\RomanBushuev\YandexDisk\MarketData\CBR\raw\debt_zcc_rub\debt_zcc_rub.csv";
            Mapping mapping = new Mapping();
            mapping.AI = new Dictionary<Enum, string>()
            {
                #region Финансовые инструменты
                {PositionAttribute.ActualDate, "Date"},
                #endregion
            };


            List<KeyValuePair<string, Type>> pages = new List<KeyValuePair<string, Type>>()
            {
                new KeyValuePair<string,Type>("Table1", typeof(CurveRateAttribute))
            };

            Dictionary<ScalarAttribute, ParamType> sp = new Dictionary<ScalarAttribute, ParamType>();

            Provider provider = new Provider(connection,
                 mapping,
                 pages,
                 sp,
                 ident: "DEBT_ZCC_RUB");

            provider.Download();
            Console.WriteLine("done");
        }
    }
}
