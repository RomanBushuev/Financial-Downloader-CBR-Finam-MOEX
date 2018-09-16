using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

using Core.Mir;
using Core.Mir.BaseTypes;
using Core.Mir.Enumerations;
using Core.Mir.Interfaces;
using ConvertData;

using DataProvider.Input.CsvReader;
using System.IO;

namespace TestConvertData
{
    [TestClass]
    public class TestConvert
    {

        [TestMethod]
        public void FindAllRates()
        {
            string path = @"C:\Users\RomanBushuev\YandexDisk\MarketData\MOEX\raw\";

            foreach (var file in Directory.GetFiles(path, "*.csv", SearchOption.AllDirectories))
            {
                var roman = file.Substring(path.Length, 10).Split('.');
                DateTime  date = new DateTime(int.Parse(roman[0]), int.Parse(roman[1]),int.Parse(roman[2]));

                #region Settings inputProvider
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
                {ScalarAttribute.DetailedType, "TYPENAME"},
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
                    date,
                    pages,
                    sp,
                    skipLines: 2);              


                provider.Download();
                #endregion

                Core.Mir.Environment environment = new Core.Mir.Environment();
                environment.Market = new MarketData(provider);

                Calculation calculation = new ConvertData.Convert();

                calculation.Environment = environment;
                calculation.Run();
                ResultSet result = calculation.returnResultSet();

                #region Settings outputProvider
                string mirConnection = "Host = localhost; Username =postgres; Password =roman; Database =MIR";
                DataProvider.Output.Mir.Mapping outMapping = new DataProvider.Output.Mir.Mapping();

                outMapping.AI = new Dictionary<Enum, string>()
            {
                #region Финансовые инструменты
                {ScalarAttribute.Currency, "8"},
                {ScalarAttribute.MatDate, "11"},
                {ScalarAttribute.Name, "5"},
                {ScalarAttribute.Nominal, "9"},
                {ScalarAttribute.SecId, "2"},
                {ScalarAttribute.ShortName, "4"},
                {ScalarAttribute.Size, "10"},
                {ScalarAttribute.DetailedType, "8"},
                {TimeSeriesAttribute.Close, "3"},
                {FinType.Bond, "13"},
                {FinType.Equity, "12"},
                {FinType.Certificate, "15"},
                {FinType.Fund, "14"},
                {FinType.DepositaryReceipt, "16"},
                #endregion
            };

                outMapping.TKE = new Dictionary<KeyValuePair<Type, string>, Enum>()
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

                var outProvider = new DataProvider.Output.Mir.Provider(mirConnection, outMapping);
                outProvider.SetParams(new Dictionary<string, object>()
                {
                    {DataProvider.Output.Mir.Provider.SCALAR, "MOEX"},
                    {DataProvider.Output.Mir.Provider.QUOTE, "MOEX"}
                });
                #endregion

                outProvider.Save(calculation.returnResultSet());
                break;
            }
        }

        /// <summary>
        /// Вставка значений, которые имеются у нас в файлах
        /// </summary>
        [TestMethod]
        public void InsertFinancialData()
        {
            DateTime date = new DateTime(2017, 11, 12);
            string dtext = date.ToString("yyyy.MM.dd");
            string dpath = string.Format(@"C:\Users\RomanBushuev\YandexDisk\MarketData\MOEX\raw\{0}\rates.csv", dtext);
            while (date <= DateTime.Today.AddDays(1))
            {
                dtext = date.ToString("yyyy.MM.dd");
                dpath = string.Format(@"C:\Users\RomanBushuev\YandexDisk\MarketData\MOEX\raw\{0}\rates.csv", dtext);
                if (System.IO.File.Exists(dpath))
                {
                    #region Исполняемый код

                    #region Settings inputProvider
                    string connection = dpath;
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
                        {ScalarAttribute.DetailedType, "TYPENAME"},
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
                        date,
                        pages,
                        sp,
                        skipLines: 2);


                    provider.Download();
                    #endregion

                    Core.Mir.Environment environment = new Core.Mir.Environment();
                    environment.Market = new MarketData(provider);

                    Calculation calculation = new ConvertData.Convert();

                    calculation.Environment = environment;
                    calculation.Run();
                    ResultSet result = calculation.returnResultSet();


                    #region Settings outputProvider
                    string mirConnection = "Host = localhost; Username =postgres; Password =roman; Database =MIR";
                    DataProvider.Output.Mir.Mapping outMapping = new DataProvider.Output.Mir.Mapping();

                    outMapping.AI = new Dictionary<Enum, string>()
                    {
                        #region Финансовые инструменты
                        {ScalarAttribute.SecId, "2"},
                        {TimeSeriesAttribute.Close, "3"},
                        {ScalarAttribute.ShortName, "4"},
                        {ScalarAttribute.Name, "5"},
                        {ScalarAttribute.DetailedType, "6"},
                        {ScalarAttribute.Currency, "8"},
                        {ScalarAttribute.Nominal, "9"},
                        {ScalarAttribute.Size, "10"},
                        {ScalarAttribute.MatDate, "11"},
                        {FinType.Equity, "12"},
                        {FinType.Bond, "13"},
                        {FinType.Fund, "14"},
                        {FinType.Certificate, "15"},
                        {FinType.DepositaryReceipt, "16"},
                        #endregion
                    };

                    outMapping.TKE = new Dictionary<KeyValuePair<Type, string>, Enum>()
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

                    var outProvider = new DataProvider.Output.Mir.Provider(mirConnection, outMapping);
                    outProvider.SetParams(new Dictionary<string, object>()
                    {
                        {DataProvider.Output.Mir.Provider.SCALAR, "MOEX"},
                        {DataProvider.Output.Mir.Provider.QUOTE, "MOEX"}
                    });
                    #endregion

                    outProvider.Save(calculation.returnResultSet());
                    #endregion

                    string text = string.Format("{0} {1}\n", "rates",dtext);
                    System.IO.File.AppendAllText("rates.txt", text);
                }

                date = date.AddDays(1);                
            }
        }

        /// <summary>
        /// Вставка валютных пар
        /// </summary>
        [TestMethod]
        public void InsertCurrenciesData()
        {
            string connection = @"C:\Users\RomanBushuev\YandexDisk\MarketData\CBR\raw\currencies\2018.03.18.csv";
            DateTime date = new DateTime(1998, 01, 01);
            string dtext = date.ToString("yyyy.MM.dd");
            string dpath = string.Format(@"C:\Users\RomanBushuev\YandexDisk\MarketData\CBR\raw\currencies\{0}.csv", dtext);
            while (date <= DateTime.Today.AddDays(1))
            {
                dtext = date.ToString("yyyy.MM.dd");
                dpath = string.Format(@"C:\Users\RomanBushuev\YandexDisk\MarketData\CBR\raw\currencies\{0}.csv", dtext);
                connection = dpath;
                if (File.Exists(dpath))
                {
                    Mapping mapping = new Mapping();

                    mapping.AI = new Dictionary<Enum, string>()
                    {
                        #region Финансовые инструменты
                        {PositionAttribute.Ident, "VchCode" },
                        //{ScalarAttribute.Size, "Vnom" },
                        {TimeSeriesAttribute.Close, "Vcurs" },
                        //{ScalarAttribute.Name, "Vname" },
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
                        date,
                        pages,
                        sp);

                    provider.Download();
                    Core.Mir.Environment environment = new Core.Mir.Environment();
                    environment.Market = new MarketData(provider);

                    Calculation calculation = new ConvertData.Convert();

                    calculation.Environment = environment;
                    calculation.Run();
                    ResultSet result = calculation.returnResultSet();

                    #region Settings outputProvider
                    string mirConnection = "Host = localhost; Username =postgres; Password =roman; Database =MIR";
                    DataProvider.Output.Mir.Mapping outMapping = new DataProvider.Output.Mir.Mapping();

                    outMapping.AI = new Dictionary<Enum, string>()
                    {
                        #region Финансовые инструменты
                        {ScalarAttribute.SecId, "2"},
                        {TimeSeriesAttribute.Close, "3"},
                        {ScalarAttribute.ShortName, "4"},
                        {ScalarAttribute.Name, "5"},
                        {ScalarAttribute.DetailedType, "6"},
                        {ScalarAttribute.Currency, "8"},
                        {ScalarAttribute.Nominal, "9"},
                        {ScalarAttribute.Size, "10"},
                        {ScalarAttribute.MatDate, "11"},
                        {FinType.Equity, "12"},
                        {FinType.Bond, "13"},
                        {FinType.Fund, "14"},
                        {FinType.Certificate, "15"},
                        {FinType.DepositaryReceipt, "16"},
                        {FinType.Default, "20"}
                        #endregion
                    };

                    outMapping.TKE = new Dictionary<KeyValuePair<Type, string>, Enum>()
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

                    using (var outProvider = new DataProvider.Output.Mir.Provider(mirConnection, outMapping))
                    {
                        outProvider.SetParams(new Dictionary<string, object>()
                        {
                            {DataProvider.Output.Mir.Provider.SCALAR, "CBR"},
                            {DataProvider.Output.Mir.Provider.QUOTE, "CBR"}
                        });

                        outProvider.Save(calculation.returnResultSet());
                    }
                    #endregion

                    string text = string.Format("{0} {1}\n", "Currencies", dtext);
                    System.IO.File.AppendAllText("Currencies.txt", text);
                }


                date = date.AddDays(1);
            }
        }

        /// <summary>
        /// Вставка кривой дисконтирования
        /// </summary>
        [TestMethod]
        public void InsertDebtZccRub()
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
            Core.Mir.Environment environment = new Core.Mir.Environment();
            environment.Market = new MarketData(provider);

            Calculation calculation = new ConvertData.Convert();

            calculation.Environment = environment;
            calculation.Run();
            ResultSet result = calculation.returnResultSet();

            #region Settings outputProvider
            string mirConnection = "Host = localhost; Username =postgres; Password =roman; Database =MIR";
            DataProvider.Output.Mir.Mapping outMapping = new DataProvider.Output.Mir.Mapping();

            outMapping.AI = new Dictionary<Enum, string>()
          {
              #region Финансовые инструменты
                        {FinType.PercentCurve, "19"},
                        #endregion
          };

            outMapping.TKE = new Dictionary<KeyValuePair<Type, string>, Enum>()
          {

          };


            mapping.ET = new Dictionary<Enum, Type>()
          {
              {ScalarAttribute.DetailedType, typeof(FinTypeDetailedLevel)},
              {ScalarAttribute.Currency, typeof(Currencies)},
              {ScalarAttribute.FinType, typeof(FinType)},
          };

            using (var outProvider = new DataProvider.Output.Mir.Provider(mirConnection, outMapping))
            {
                outProvider.SetParams(new Dictionary<string, object>()
              {
                  {DataProvider.Output.Mir.Provider.SCALAR, "CBR"},
                  {DataProvider.Output.Mir.Provider.QUOTE, "CBR"}
              });

                outProvider.Save(calculation.returnResultSet());
            }


            #endregion


        }

        [TestMethod]
        public void InsertRuonia()
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

            Dictionary<ScalarAttribute, ParamType> sp = new Dictionary<ScalarAttribute, ParamType>();

            Provider provider = new Provider(connection,
                mapping,
                pages,
                sp,
                ident: "RUONIA");

            provider.Download();

            Core.Mir.Environment environment = new Core.Mir.Environment();
            environment.Market = new MarketData(provider);

            Calculation calculation = new ConvertData.Convert();

            calculation.Environment = environment;
            calculation.Run();
            ResultSet result = calculation.returnResultSet();

            #region Settings outputProvider
            string mirConnection = "Host = localhost; Username =postgres; Password =roman; Database =MIR";
            DataProvider.Output.Mir.Mapping outMapping = new DataProvider.Output.Mir.Mapping();

            outMapping.AI = new Dictionary<Enum, string>()
                    {
                        #region Финансовые инструменты
                        {ScalarAttribute.SecId, "2"},
                        {TimeSeriesAttribute.Close, "3"},
                        {ScalarAttribute.ShortName, "4"},
                        {ScalarAttribute.Name, "5"},
                        {ScalarAttribute.DetailedType, "6"},
                        {ScalarAttribute.Currency, "8"},
                        {ScalarAttribute.Nominal, "9"},
                        {ScalarAttribute.Size, "10"},
                        {ScalarAttribute.MatDate, "11"},
                        {FinType.Equity, "12"},
                        {FinType.Bond, "13"},
                        {FinType.Fund, "14"},
                        {FinType.Certificate, "15"},
                        {FinType.DepositaryReceipt, "16"},
                        {FinType.Default, "20"}
                        #endregion
                    };

            outMapping.TKE = new Dictionary<KeyValuePair<Type, string>, Enum>()
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

            using (var outProvider = new DataProvider.Output.Mir.Provider(mirConnection, outMapping))
            {
                outProvider.SetParams(new Dictionary<string, object>()
                        {
                            {DataProvider.Output.Mir.Provider.SCALAR, "CBR"},
                            {DataProvider.Output.Mir.Provider.QUOTE, "CBR"}
                        });

                outProvider.Save(calculation.returnResultSet());
            }
            #endregion

        }

        [TestMethod]
        public void InsertCashFlows()
        {
            string connection = @"C:\Users\RomanBushuev\YandexDisk\MarketData\finam\raw\cashflows\27.05.2018.csv";
            Mapping mapping = new Mapping();

            mapping.AI = new Dictionary<Enum, string>()
            {
                #region Финансовые инструменты
                {PositionAttribute.Ident, "ISIN"},
                {PositionAttribute.ActualDate, "VALID_DATE"},
                {PositionAttribute.Type, "TYPE"},
                {ScalarAttribute.MatDate, "DATETIME"},
                {ScalarAttribute.DetailedType, "CASHFLOW"},
                {ScalarAttribute.Nominal, "VALUE"},
                #endregion
            };

            Dictionary<ScalarAttribute, ParamType> sp = new Dictionary<ScalarAttribute, ParamType>()
                    {
                        {ScalarAttribute.MatDate, ParamType.DateTime},
                        {ScalarAttribute.Nominal, ParamType.Decimal},
                        {ScalarAttribute.DetailedType, ParamType.String},
                    };

            mapping.TKE = new Dictionary<KeyValuePair<Type, string>, Enum>()
                    {
                        #region Финансовые инструменты
                        {new KeyValuePair<Type,string>(typeof(FinType),"BOND"), FinType.Bond},
                        #endregion
                    };

            mapping.ET = new Dictionary<Enum,Type>()
            {
                {ScalarAttribute.FinType, typeof(FinType)},
            };

            List<KeyValuePair<string, Type>> pages = new List<KeyValuePair<string, Type>>()
            {
                new KeyValuePair<string,Type>("Table1", typeof(CashFlowAttribute))
            };

            Provider provider = new Provider(connection,
                mapping,
                pages,
                sp);

            provider.Download();
            var t = provider.Cache;

            Core.Mir.Environment environment = new Core.Mir.Environment();
            environment.Market = new MarketData(provider);

            Calculation calculation = new ConvertData.Convert();

            calculation.Environment = environment;
            calculation.Run();
            ResultSet result = calculation.returnResultSet();

            //output settings
            #region Settings outputProvider
            string mirConnection = "Host = localhost; Username =postgres; Password =roman; Database =MIR";
            DataProvider.Output.Mir.Mapping outMapping = new DataProvider.Output.Mir.Mapping();

            outMapping.AI = new Dictionary<Enum, string>()
                    {
                        #region Финансовые инструменты
                        {ScalarAttribute.SecId, "2"},
                        {TimeSeriesAttribute.Close, "3"},
                        {ScalarAttribute.ShortName, "4"},
                        {ScalarAttribute.Name, "5"},
                        {ScalarAttribute.DetailedType, "6"},
                        {ScalarAttribute.Currency, "8"},
                        {ScalarAttribute.Nominal, "9"},
                        {ScalarAttribute.Size, "10"},
                        {ScalarAttribute.MatDate, "11"},
                        {FinType.Equity, "12"},
                        {FinType.Bond, "13"},
                        {FinType.Fund, "14"},
                        {FinType.Certificate, "15"},
                        {FinType.DepositaryReceipt, "16"},
                        {CashFlowAttribute.Coupon, "27616" },
                        {CashFlowAttribute.Redemption, "27617"}
                        #endregion
                    };


            outMapping.TKE = new Dictionary<KeyValuePair<Type, string>, Enum>()
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
                    };


            mapping.ET = new Dictionary<Enum, Type>()
                    {
                        {ScalarAttribute.DetailedType, typeof(FinTypeDetailedLevel)},
                        {ScalarAttribute.Currency, typeof(Currencies)},
                        {ScalarAttribute.FinType, typeof(FinType)},
                    };

            using (var outProvider = new DataProvider.Output.Mir.Provider(mirConnection, outMapping))
            {
                outProvider.SetParams(new Dictionary<string, object>()
                    {
                        {DataProvider.Output.Mir.Provider.SCALAR, "FINAM"},
                        {DataProvider.Output.Mir.Provider.QUOTE, "FINAM"},
                        {DataProvider.Output.Mir.Provider.REPORTDATE, DateTime.Today}
                    });

                outProvider.Save(calculation.returnResultSet());
            }
            #endregion
        }

        [TestMethod]
        public void InsertDateStart()
        {
            string connection = @"C:\Users\RomanBushuev\YandexDisk\MarketData\finam\raw\scalars\2018.05.27.csv";
            Mapping mapping = new Mapping();

            mapping.AI = new Dictionary<Enum, string>()
            {
                #region Финансовые инструменты
                {PositionAttribute.Ident, "ISIN"},
                {PositionAttribute.ActualDate, "VALID_DATE"},
                {PositionAttribute.Type, "TYPE"},
                {ScalarAttribute.StartDate, "DATETIME"}
                #endregion
            };

            Dictionary<ScalarAttribute, ParamType> sp = new Dictionary<ScalarAttribute, ParamType>()
                    {
                        {ScalarAttribute.StartDate, ParamType.DateTime},
                    };

            mapping.TKE = new Dictionary<KeyValuePair<Type, string>, Enum>()
                    {
                        #region Финансовые инструменты
                        {new KeyValuePair<Type,string>(typeof(FinType),"BOND"), FinType.Bond},
                        #endregion
                    };

            mapping.ET = new Dictionary<Enum, Type>()
            {
                {ScalarAttribute.FinType, typeof(FinType)},
            };

            List<KeyValuePair<string, Type>> pages = new List<KeyValuePair<string, Type>>()
            {
                new KeyValuePair<string,Type>("Table1", typeof(ScalarAttribute))
            };

            Provider provider = new Provider(connection,
                mapping,
                pages,
                sp);

            provider.Download();
            var t = provider.Cache;

            Core.Mir.Environment environment = new Core.Mir.Environment();
            environment.Market = new MarketData(provider);

            Calculation calculation = new ConvertData.Convert();

            calculation.Environment = environment;
            calculation.Run();
            ResultSet result = calculation.returnResultSet();

            //output settings
            #region Settings outputProvider
            string mirConnection = "Host = localhost; Username =postgres; Password =roman; Database =MIR";
            DataProvider.Output.Mir.Mapping outMapping = new DataProvider.Output.Mir.Mapping();

            outMapping.AI = new Dictionary<Enum, string>()
                    {
                        #region Финансовые инструменты
                        {ScalarAttribute.SecId, "2"},
                        {TimeSeriesAttribute.Close, "3"},
                        {ScalarAttribute.ShortName, "4"},
                        {ScalarAttribute.Name, "5"},
                        {ScalarAttribute.DetailedType, "6"},
                        {ScalarAttribute.Currency, "8"},
                        {ScalarAttribute.Nominal, "9"},
                        {ScalarAttribute.Size, "10"},
                        {ScalarAttribute.MatDate, "11"},
                        {FinType.Equity, "12"},
                        {FinType.Bond, "13"},
                        {FinType.Fund, "14"},
                        {FinType.Certificate, "15"},
                        {FinType.DepositaryReceipt, "16"},
                        {ScalarAttribute.StartDate, "27680"},

                        #endregion
                    };


            outMapping.TKE = new Dictionary<KeyValuePair<Type, string>, Enum>()
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
                    };


            mapping.ET = new Dictionary<Enum, Type>()
                    {
                        {ScalarAttribute.DetailedType, typeof(FinTypeDetailedLevel)},
                        {ScalarAttribute.Currency, typeof(Currencies)},
                        {ScalarAttribute.FinType, typeof(FinType)},
                    };

            using (var outProvider = new DataProvider.Output.Mir.Provider(mirConnection, outMapping))
            {
                outProvider.SetParams(new Dictionary<string, object>()
                    {
                        {DataProvider.Output.Mir.Provider.SCALAR, "FINAM"},
                        {DataProvider.Output.Mir.Provider.QUOTE, "FINAM"},
                        {DataProvider.Output.Mir.Provider.REPORTDATE, DateTime.Today}
                    });

                outProvider.Save(calculation.returnResultSet());
            }
            #endregion
        }
    }
}
