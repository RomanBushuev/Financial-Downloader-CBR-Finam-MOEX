using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Threading;
using Core.Mir.Enumerations;
using DataProvider.Input.CsvReader;
using Core.Mir;
using Core.Mir.Interfaces;
using Algorithms.General.CalculateVar;
using Algorithms.General.CalculateYield;

namespace ExecutorTask
{
    class Program
    {
        public static Dictionary<string, Action<string>> actions = new Dictionary<string, Action<string>>()
        {
            {"debt_zcc_rub", convert_debt_zcc_rub },
            {"Ruonia", convert_ruonia },
            {"Currencies", convert_currencies},
            {"rates", convert_rate},
            {"volat", calculate_volat},
            {"var", calculate_var},
            {"yield", calculate_yield},
            {"Cashflows", convert_cashflow},
            {"ScalarFromFinam", convert_scalar_from_finam},
        };
        static void Main(string[] args)
        {
            //прочитали все задания из списка, блокировка на время чтения  
            List<string> tasks = ReadAllMessages();

            foreach (var x in tasks)
            {
                string[] innerArgs = x.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                //сравнили со списком доступных

                if(actions.ContainsKey(innerArgs.FirstOrDefault()))
                {
                    //запустили на исполнение 
                    try
                    {
                        actions[innerArgs.FirstOrDefault()](x);
                    }
                    catch (Exception ex)
                    {
                        //завершилось с ошибками
                        string message = string.Format("{0}",
                            x);
                        WriteMessageToFailedLog(message, ex.ToString());
                        GC.Collect();
                        continue;
                    }
                    //завершилось без ошибок
                    {
                        string message = string.Format("{0}",
                            x);
                        WriteMessageToSeccess(message);
                    }
                }
            }
        }

        public static List<string> ReadAllMessages()
        {
            List<string> tasks = new List<string>();

            string tasksFile = ConfigurationManager.AppSettings["list_task"];
            int attemptions = 0;
            if(File.Exists(tasksFile))
            {
                bool isRead = true;
                bool isRepeat = true;
                while (isRepeat)
                {
                    try
                    {
                        string path = tasksFile;
                        using (FileStream fileStream = new FileStream(path,
                            FileMode.Open,
                            FileAccess.ReadWrite,
                            FileShare.None))
                        {
                            string contents;
                            using (var sr = new StreamReader(fileStream))
                            {
                                contents = sr.ReadToEnd();
                            }
                            isRead = false;
                            tasks = contents.Split(new char[] {'\r', '\n'}, 
                                StringSplitOptions.RemoveEmptyEntries).ToList();
                        }
                        if(!isRead)
                        {
                            File.WriteAllText(path, string.Empty);
                            isRepeat = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Thread.Sleep(1000);
                        attemptions++;
                        isRepeat = true;
                        if (attemptions > 10)
                        {
                            isRepeat = false;
                        }
                    }
                    finally
                    {

                    }
                }
            }
            else
            {
                //написать в лог сообщений об ошибке
            }

            return tasks;
        }

        public static void WriteMessageToFailedLog(string message, string exception = "")
        {
            string filename = ConfigurationManager.AppSettings["list_failed_task"];
            try
            {
                File.AppendAllLines(filename, new string[] { message });
            }
            catch (Exception ex)
            {

            }

            filename = ConfigurationManager.AppSettings["list_failed_task_details"];
            try
            {
                message = string.Format("{0}:{1}", message, exception);
                File.AppendAllLines(filename, new string[] { message });
            }
            catch(Exception ex)
            {

            }
        }

        public static void WriteMessageToSeccess(string message)
        {
            string filename = ConfigurationManager.AppSettings["list_done_task"];
            try
            {
                File.AppendAllLines(filename, new string[] { message });
            }
            catch (Exception ex)
            {

            }
        }

        public static void WriteMessageToQueue(string message)
        {
            string path = ConfigurationManager.AppSettings["list_task"];
            bool isWrite = false;
            while (!isWrite)
            {
                try
                {
                    using (FileStream fileStream = new FileStream(path,
                        FileMode.Append,
                        FileAccess.Write,
                        FileShare.Read))
                    {
                        string dataasstring = message;
                        byte[] info = new UTF8Encoding(true).GetBytes(dataasstring);
                        fileStream.Write(info, 0, info.Length);
                        isWrite = true;
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    Thread.Sleep(1000);
                }
            }
        }

        public static void convert_debt_zcc_rub(string arg)
        {
            string connection = ConfigurationManager.AppSettings["debt_zcc_rub"];
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
            string mirConnection = ConfigurationManager.AppSettings["mirconnection"];
            DataProvider.Output.Mir.Mapping outMapping = new DataProvider.Output.Mir.Mapping();

            outMapping.AI = new Dictionary<Enum, string>()
          {
              #region Финансовые инструменты
                        {FinType.PercentCurve, "19"},
                        {TimeSeriesAttribute.Close, "3"},
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

        public static void convert_ruonia(string arg)
        {
            string connection = ConfigurationManager.AppSettings["ruonia"];
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
            string mirConnection = ConfigurationManager.AppSettings["mirconnection"];
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
                        {FinType.Default, "19"}
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

        public static void convert_currencies(string arg)
        {
            string[] aargs = arg.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            string connection = string.Format(@"{0}{1}.csv",
                ConfigurationManager.AppSettings["currencies"],
                aargs[1]);
            DateTime date = DateTime.ParseExact(aargs[1], "yyyy.MM.dd", System.Globalization.CultureInfo.InvariantCulture);
            string dtext = date.ToString("yyyy.MM.dd");
            if (File.Exists(connection))
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
                        {FinType.Default, "20"},
                        {TimeSeriesAttribute.Close, "3"},
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

                string message = string.Format("var {0}\n", aargs[1]);
                WriteMessageToQueue(message);
            }
        }

        public static void convert_rate(string arg)
        {
            string[] aargs = arg.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            string connection = string.Format(@"{0}{1}\{2}",
                ConfigurationManager.AppSettings["rates_path"],
                aargs[1],
                ConfigurationManager.AppSettings["rates_file"]);
            DateTime date = DateTime.ParseExact(aargs[1], "yyyy.MM.dd", System.Globalization.CultureInfo.InvariantCulture);

            if (System.IO.File.Exists(connection))
            {
                #region Исполняемый код

                #region Settings inputProvider
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
                        {ScalarAttribute.Isin, "ISIN"},
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
                        {ScalarAttribute.Isin, ParamType.String},
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
                string mirConnection = ConfigurationManager.AppSettings["mirconnection"];
                DataProvider.Output.Mir.Mapping outMapping = new DataProvider.Output.Mir.Mapping();

                outMapping.AI = new Dictionary<Enum, string>()
                    {
                        #region Финансовые инструменты
                        {ScalarAttribute.SecId, "2"},
                        {TimeSeriesAttribute.Close, "3"},
                        {ScalarAttribute.ShortName, "4"},
                        {ScalarAttribute.Name, "5"},
                        {ScalarAttribute.DetailedType, "6"},
                        {ScalarAttribute.Isin, "7"},
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

                using (var outProvider = new DataProvider.Output.Mir.Provider(mirConnection, outMapping))
                {
                    outProvider.SetParams(new Dictionary<string, object>()
                    {
                        {DataProvider.Output.Mir.Provider.SCALAR, "MOEX"},
                        {DataProvider.Output.Mir.Provider.QUOTE, "MOEX"}
                    });
                #endregion

                    outProvider.Save(calculation.returnResultSet());
                }
                #endregion

                //добавляем задачу 
                //volat
                //var
                string message = string.Format("volat {0}\n", aargs[1]);
                WriteMessageToQueue(message);

                message = string.Format("var {0}\n", aargs[1]);
                WriteMessageToQueue(message);

            }
        }

        public static void calculate_volat(string arg)
        {
            Console.WriteLine(arg);
            string[] aargs = arg.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            DateTime reportDate = DateTime.ParseExact(aargs[1], "yyyy.MM.dd", System.Globalization.CultureInfo.InvariantCulture);
           
            #region исполняемый код
            string connection = ConfigurationManager.AppSettings["mirconnection"];
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


            DataProvider.Input.CombinedProvider.Provider provider =
                new DataProvider.Input.CombinedProvider.Provider(new List<IMarketProvider>()
                    {
                        MoexProvider,
                        CbrProvider
                    });

            Core.Mir.Environment environment =
                new Core.Mir.Environment();
            environment.Market = new MarketData(provider);

            CalculationOneData calculation = new CalculateVolat.Volat();
            calculation.ReportDate = reportDate;
            Dictionary<string, object> pars = new Dictionary<string, object>()
            {
                {CalculateVolat.Volat.WINDOW, 5},
                {CalculateVolat.Volat.MINIMAL_AMOUT, 10},
            };

            if(aargs.Length == 4)
            {
                for(int i = 2; i< aargs.Length;++i)
                {
                    string[] values = aargs[i].Split(':');
                    
                    if(values.Length != 2)
                    {
                        continue;
                    }
                    string ident = values[0];
                    string val = values[1];
                    if(calculation.GetParams().FirstOrDefault(z=>z.Ident == ident)!= null)
                    {
                        var pd = calculation.GetParams().FirstOrDefault(z => z.Ident == ident);
                        if(pd.ParamType == ParamType.DateTime)
                        {
                            object obj = DateTime.ParseExact(val.ToString(), "dd.MM.yyyy", null);
                            pars[ident] = obj;
                        }
                        if(pd.ParamType == ParamType.Decimal)
                        {
                            object obj = Decimal.Parse(val.ToString());
                            pars[ident] = obj;
                        }
                        if(pd.ParamType == ParamType.Int)
                        {
                            object obj = int.Parse(val.ToString());
                            pars[ident] = obj;
                        }
                        if(pd.ParamType == ParamType.String)
                        {
                            object obj = val.ToString();
                            pars[ident] = obj;
                        }
                    }
                    
                }
            }
            calculation.SetParams(pars);
            calculation.Environment = environment;

            bool result = false;
            try
            {
                result = calculation.Run();
            }
            catch(Exception ex)
            {
                result = false;
            }
            #region Выгрузка данных в мир 
            if(result)
            {
                Console.WriteLine(reportDate);
                Console.WriteLine(calculation.returnResultSet().TimeSeries.Count);
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
                        {TimeSeriesAttribute.Volat, "20225"},
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
                            {DataProvider.Output.Mir.Provider.SCALAR, "CALCULATED"},
                            {DataProvider.Output.Mir.Provider.QUOTE, "CALCULATED"}
                        });

                    outProvider.Save(calculation.returnResultSet());
                }
                #endregion
            }
            #endregion
            #endregion
            provider.Dispose();
            GC.Collect();
        }

        public static void calculate_var(string arg)
        {
            Console.WriteLine(arg);
            string[] aargs = arg.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            DateTime reportDate = DateTime.ParseExact(aargs[1], "yyyy.MM.dd", System.Globalization.CultureInfo.InvariantCulture);

            #region исполняемый код
            string connection = ConfigurationManager.AppSettings["mirconnection"];
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
                {Currencies.USD, "USD"},
                { Currencies.GBP, "GBP"},
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


            DataProvider.Input.CombinedProvider.Provider provider =
                new DataProvider.Input.CombinedProvider.Provider(new List<IMarketProvider>()
                    {
                        MoexProvider,
                        CbrProvider
                    });

            Core.Mir.Environment environment =
                new Core.Mir.Environment();
            environment.Market = new MarketData(provider);

            CalculationOneData calculation = new Var();
            calculation.ReportDate = reportDate;
            Dictionary<string, object> pars = new Dictionary<string, object>()
            {
                {Var.WINDOW, 5},
                {Var.QUANTILE, 0.9m},
            };

            if (aargs.Length == 4)
            {
                for (int i = 2; i < aargs.Length; ++i)
                {
                    string[] values = aargs[i].Split(':');

                    if (values.Length != 2)
                    {
                        continue;
                    }
                    string ident = values[0];
                    string val = values[1];
                    if (calculation.GetParams().FirstOrDefault(z => z.Ident == ident) != null)
                    {
                        var pd = calculation.GetParams().FirstOrDefault(z => z.Ident == ident);
                        if (pd.ParamType == ParamType.DateTime)
                        {
                            object obj = DateTime.ParseExact(val.ToString(), "dd.MM.yyyy", null);
                            pars[ident] = obj;
                        }
                        if (pd.ParamType == ParamType.Decimal)
                        {
                            object obj = Decimal.Parse(val.ToString());
                            pars[ident] = obj;
                        }
                        if (pd.ParamType == ParamType.Int)
                        {
                            object obj = int.Parse(val.ToString());
                            pars[ident] = obj;
                        }
                        if (pd.ParamType == ParamType.String)
                        {
                            object obj = val.ToString();
                            pars[ident] = obj;
                        }
                    }

                }
            }

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
            #endregion

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
                        {TimeSeriesAttribute.Var, "20226"},
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
            string excelPath = ConfigurationManager.AppSettings["var"];
            string excelFileName = string.Format("{0}.xlsx",
                calculation.ReportDate.ToString("yyyy.MM.dd"));

            DataProvider.Output.Excel.Provider excelProvider =
                new DataProvider.Output.Excel.Provider(excelPath + excelFileName);

            excelProvider.Save(calculation.returnResultSet());
            #endregion
            #endregion

            provider.Dispose();
            GC.Collect();
        }

        public static void calculate_yield(string arg)
        {
            Console.WriteLine(arg);
            string[] aargs = arg.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            DateTime reportDate = DateTime.ParseExact(aargs[1], "yyyy.MM.dd", System.Globalization.CultureInfo.InvariantCulture);

            #region Исполняемый код
            string connection = ConfigurationManager.AppSettings["mirconnection"];
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
                {Currencies.USD, "USD"},
                { Currencies.GBP, "GBP"},
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


            DataProvider.Input.CombinedProvider.Provider provider =
                new DataProvider.Input.CombinedProvider.Provider(new List<IMarketProvider>()
                    {
                        MoexProvider,
                        CbrProvider
                    });

            Core.Mir.Environment environment =
                new Core.Mir.Environment();
            environment.Market = new MarketData(provider);
            #endregion

            CalculationOneData calculation = new Yield();
            calculation.ReportDate = reportDate;
            calculation.Environment = environment;

            bool result = false;
            try
            {
                result = calculation.Run();
            }
            catch (Exception ex)
            {
                result = false;
            }
            #region Выгрузка данных в мир
            if (result)
            {
                Console.WriteLine(reportDate);
                Console.WriteLine(calculation.returnResultSet().TimeSeries.Count);
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
                        {TimeSeriesAttribute.Yield, "23218"},
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
                            {DataProvider.Output.Mir.Provider.SCALAR, "CALCULATED"},
                            {DataProvider.Output.Mir.Provider.QUOTE, "CALCULATED"}
                        });

                    outProvider.Save(calculation.returnResultSet());
                }
                #endregion
            }
            #endregion
            provider.Dispose();
            GC.Collect();
        }

        public static void convert_cashflow(string arg)
        {
            string[] aargs = arg.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            DateTime reportDate = DateTime.ParseExact(aargs[1], "yyyy.MM.dd", System.Globalization.CultureInfo.InvariantCulture);

            string connection = string.Format(@"{0}{1}.csv",
                ConfigurationManager.AppSettings["cashflow"],
                aargs[1]);

            //DateTime ReportDate = DateTime.ParseExact(aargs[1], "yyyy.MM.dd", System.Globalization.CultureInfo.InvariantCulture);

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

            mapping.ET = new Dictionary<Enum, Type>()
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
            string mirConnection = ConfigurationManager.AppSettings["mirconnection"];
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
                        {ScalarAttribute.StartDate, "31125"},
                        {CashFlowAttribute.Coupon, "27677"},
                        {CashFlowAttribute.Redemption, "27678"},
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
                        {DataProvider.Output.Mir.Provider.REPORTDATE, reportDate}
                    });

                outProvider.Save(calculation.returnResultSet());
            }
            #endregion
        }

        public static void convert_scalar_from_finam(string arg)
        {
            string[] aargs = arg.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            DateTime reportDate = DateTime.ParseExact(aargs[1], "yyyy.MM.dd", System.Globalization.CultureInfo.InvariantCulture);

            string connection = string.Format(@"{0}{1}.csv",
                ConfigurationManager.AppSettings["scalar"],
                aargs[1]);

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
            string mirConnection = ConfigurationManager.AppSettings["mirconnection"];
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
                        {DataProvider.Output.Mir.Provider.REPORTDATE, reportDate}
                    });

                outProvider.Save(calculation.returnResultSet());
            }
            #endregion
        }
    }
}
