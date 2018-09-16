using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Core.Mir.BaseTypes;
using Core.Mir.Enumerations;
using Core.Mir.Interfaces;

using DataProvider.Output.Mir.DbObject;
using DataProvider.Output.Mir.DbRepository;
using DataBaseLink;
using System.Data;
using Core.Mir;


namespace DataProvider.Output.Mir
{
    public class Provider:IOutputMarketProvider, IGetParams, ISetParams, IDisposable
    {
        public const string SCALAR = "ScalarSource";
        public const string QUOTE = "QuoteSource";
        public const string REPORTDATE = "REPORTDATE";
        #region fields
        private string _connection = string.Empty;
        private Dictionary<string, object> _providerParams =
            new Dictionary<string, object>()
            {
                {"ScalarSource", "CALCULATED"},
                {"QuoteSource", "CALCULATED"},
                {REPORTDATE, DateTime.Today}
            };

        private List<ParamDescriptor> paramDescriptors =
            new List<ParamDescriptor>()
            {
                new ParamDescriptor()
                {
                    Ident = SCALAR,
                    Description = "",
                    ParamType = ParamType.String,
                    Value = "CALCULATED"
                },
                new ParamDescriptor()
                {
                    Ident = QUOTE,
                    Description = "",
                    ParamType = ParamType.String,
                    Value = "CALCULATED"
                },
                new ParamDescriptor()
                {
                    Ident = REPORTDATE,
                    Description = "",
                    ParamType = ParamType.DateTime,
                    Value = DateTime.Today
                }
            };
        private DbLink _dbLink;
        private Mapping _mapping;
        #endregion

        public Provider(string connection)
        {
            _connection = connection;
            var tempConnection = DataBaseLink.Fabricate.CreateConnection(_connection, ConnectionType.Npgsql);
            _dbLink = new DbLink(tempConnection);

            _mapping = new Mapping();
        }

        public Provider(string connection, Mapping mapping)
            :this(connection)
        {
            _mapping = mapping;
        }

        public List<ParamDescriptor> GetParams()
        {
            return paramDescriptors;
        }

        public void SetParams(Dictionary<string, object> objects)
        {
            foreach(var x in objects)
            {
                if(_providerParams.ContainsKey(x.Key))
                    _providerParams[x.Key] = x.Value;
                else
                    _providerParams.Add(x.Key, x.Value);
            }
        }

        public IMapping GetIMapping()
        {
            return _mapping;
        }

        public void Save(ResultSet resultSet)
        {
            //сохранение скаляров
            //дата
            Save(resultSet.Dates);
            //строки
            Save(resultSet.Strings);
            //числа
            Save(resultSet.Numbers);
            //перечисления
            Save(resultSet.Enumerations);

            //сохранение временной серии
            Save(resultSet.TimeSeries);

            //котировки
            Save(resultSet.CashFlows);
        }

        private void Save(Dictionary<KeyValuePair<PortfolioPosition, Enum>, CashFlow> dictionary)
        {
            foreach (var x in dictionary)
            {
                var transaction = _dbLink.GetConnection().BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    var finIdent = x.Key.Key.Ident;
                    var finType = x.Key.Key.FinType;
                    string finTypeIdent = _mapping.GetAI(finType);

                    #region финансовый инструмент
                    fin_instrument finInstrument = new fin_instrument()
                    {
                        ident = finIdent,
                        title = finIdent,
                        ft_id = int.Parse(finTypeIdent)
                    };

                    if (FinInstrument.FindId(_dbLink, finInstrument.ident) == null)
                        FinInstrument.Insert(_dbLink, finInstrument);

                    finInstrument = FinInstrument.FindId(_dbLink, finInstrument.ident);
                    #endregion

                    #region Data_source
                    data_source dataSource = new data_source()
                    {
                        ident = _providerParams[SCALAR].ToString(),
                    };

                    dataSource = DataSource.FindId(_dbLink, dataSource.ident);
                    #endregion

                    #region fcs
                    fcs ffd = new fcs()
                    {
                        ds_id = dataSource.ds_id,
                        fi_id = finInstrument.fi_id,
                        ct_id = int.Parse(_mapping.GetAI(x.Key.Value))
                    };

                    if (FCS.Find(_dbLink, ffd.fi_id, ffd.ds_id, ffd.ct_id) == null)
                        FCS.Insert(_dbLink, ffd);
                    ffd = FCS.Find(_dbLink, ffd.fi_id, ffd.ds_id, ffd.ct_id);
                    #endregion

                    #region Денежный поток
                    foreach (var z in x.Value.Values)
                    {
                        cashflow cf = new cashflow()
                        {
                            cf_id = ffd.cf_id,
                            dat = z.Key,
                            val = z.Value,
                            valid_dat = (DateTime)_providerParams[REPORTDATE]
                        };

                        if (Cashflow.FindId(_dbLink, cf.cf_id, cf.dat, cf.valid_dat) == null)
                        {
                            //первое значение, следовательно 01.01.1900 год
                            cf.valid_dat = new DateTime(1900, 01, 01);
                            Cashflow.Insert(_dbLink, cf);
                        }
                        else
                        {
                            //нашли, но не факт, что это нормально 
                            var t = Cashflow.FindId(_dbLink, cf.cf_id, cf.dat, cf.valid_dat);
                            if(t.val != z.Value)
                                Cashflow.Insert(_dbLink, cf);
                        }
                    }
                    #endregion


                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }


            }
        }

        private void Save(Dictionary<PortfolioPosition, CashFlow> dictionary)
        {
            foreach(var x in dictionary)
            {
                var transaction = _dbLink.GetConnection().BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    var finIdent = x.Key.Ident;
                    var finType = x.Key.FinType;
                    string finTypeIdent = _mapping.GetAI(finType);

                    #region финансовый инструмент
                    fin_instrument finInstrument = new fin_instrument()
                    {
                        ident = finIdent,
                        title = finIdent,
                        ft_id = int.Parse(finTypeIdent)
                    };

                    if (FinInstrument.FindId(_dbLink, finInstrument.ident) == null)
                        FinInstrument.Insert(_dbLink, finInstrument);

                    finInstrument = FinInstrument.FindId(_dbLink, finInstrument.ident);
                    #endregion

                    #region Data_source
                    data_source dataSource = new data_source()
                    {
                        ident = _providerParams[SCALAR].ToString(),
                    };

                    dataSource = DataSource.FindId(_dbLink, dataSource.ident);
                    #endregion

                    #region fcs
                    fcs ffd = new fcs()
                    {
                        ds_id = dataSource.ds_id,
                        fi_id = finInstrument.fi_id,
                        ct_id = int.Parse(_mapping.GetAI(x.Value.Attribute))
                    };

                    if (FCS.Find(_dbLink, ffd.fi_id, ffd.ds_id, ffd.ct_id) == null)
                        FCS.Insert(_dbLink, ffd);
                    ffd = FCS.Find(_dbLink, ffd.fi_id, ffd.ds_id, ffd.ct_id);
                    #endregion

                    #region Денежный поток
                    foreach(var z in x.Value.Values)
                    {
                        cashflow cf = new cashflow()
                        {
                            cf_id = ffd.cf_id,
                            dat = z.Key,
                            val = z.Value,
                            valid_dat = (DateTime)_providerParams[REPORTDATE]
                        };

                        if(Cashflow.FindId(_dbLink, cf.cf_id, cf.dat, cf.valid_dat) == null)
                        {
                            //первое значение, следовательно 01.01.1900 год
                            cf.valid_dat = new DateTime(1900, 01, 01);
                            Cashflow.Insert(_dbLink, cf);
                        }
                        else
                        {
                            Cashflow.Insert(_dbLink, cf);
                        }
                    }
                    #endregion


                    transaction.Commit();
                }
                catch(Exception ex)
                {
                    transaction.Rollback();
                }


            }
        }

        private void Save(Dictionary<KeyValuePair<PortfolioPosition, Enum>, ScalarDate> storage)
        {
            foreach (var x in storage)
            {
                var transaction = _dbLink.GetConnection().BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    // найдем тип финансового инструмента
                    var finIdent = x.Key.Key.Ident;
                    var finType = x.Key.Key.FinType;
                    string finTypeIdent = _mapping.GetAI(finType);

                    #region финансовый инструмент
                    fin_instrument finInstrument = new fin_instrument()
                    {
                        ident = finIdent,
                        title = finIdent,
                        ft_id = int.Parse(finTypeIdent)
                    };

                    if (FinInstrument.FindId(_dbLink, finInstrument.ident) == null)
                        FinInstrument.Insert(_dbLink, finInstrument);

                    finInstrument = FinInstrument.FindId(_dbLink, finInstrument.ident);

                    #endregion

                    #region Data_source
                    data_source dataSource = new data_source()
                    {
                        ident = _providerParams[SCALAR].ToString(),
                    };

                    dataSource = DataSource.FindId(_dbLink, dataSource.ident);
                    #endregion

                    #region fisd_id
                    ffd ffd = new ffd()
                    {
                        ds_id = dataSource.ds_id,
                        fi_id = finInstrument.fi_id,
                        fif_id = int.Parse(_mapping.GetAI(x.Key.Value))
                    };

                    if (FFD.Find(_dbLink, ffd.fi_id, ffd.ds_id, ffd.fif_id) == null)
                        FFD.Insert(_dbLink, ffd);
                    ffd = FFD.Find(_dbLink, ffd.fi_id, ffd.ds_id, ffd.fif_id);
                    #endregion

                    #region date
                    fisd_date fisdDate = new fisd_date()
                    {
                        fisd_id = ffd.fisd_id,
                        dat_from = x.Value.Dictionary.First().Key,
                        val = x.Value.Dictionary.First().Value,
                    };

                    FisdDate.Insert(_dbLink, fisdDate);
                    #endregion

                    transaction.Commit();
                }
                catch(Exception ex)
                {
                    transaction.Rollback();
                }
            }
        }

        private void Save(Dictionary<KeyValuePair<PortfolioPosition, Enum>, ScalarEnum> storage)
        {
            foreach(var x in storage)
            {
                var transaction = _dbLink.GetConnection().BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    // найдем тип финансового инструмента
                    var finIdent = x.Key.Key.Ident;
                    var finType = x.Key.Key.FinType;
                    string finTypeIdent = _mapping.GetAI(finType);

                    #region финансовый инструмент
                    fin_instrument finInstrument = new fin_instrument()
                    {
                        ident = finIdent,
                        title = finIdent,
                        ft_id = int.Parse(finTypeIdent)
                    };

                    if (FinInstrument.FindId(_dbLink, finInstrument.ident) == null)
                        FinInstrument.Insert(_dbLink, finInstrument);

                    finInstrument = FinInstrument.FindId(_dbLink, finInstrument.ident);

                    #endregion

                    #region Data_source
                    data_source dataSource = new data_source()
                    {
                        ident = _providerParams[SCALAR].ToString(),
                    };

                    dataSource = DataSource.FindId(_dbLink, dataSource.ident);
                    #endregion

                    #region fisd_id
                    ffd ffd = new ffd()
                    {
                        ds_id = dataSource.ds_id,
                        fi_id = finInstrument.fi_id,
                        fif_id = int.Parse(_mapping.GetAI(x.Key.Value))
                    };

                    if (FFD.Find(_dbLink, ffd.fi_id, ffd.ds_id, ffd.fif_id) == null)
                        FFD.Insert(_dbLink, ffd);
                    ffd = FFD.Find(_dbLink, ffd.fi_id, ffd.ds_id, ffd.fif_id);
                    #endregion

                    #region dict_item
                    string key_v = _mapping.GetTKE(x.Value.Dictionary.First().Value);
                    int fif_id = int.Parse(_mapping.GetAI(x.Key.Value));

                    dict_item dict_item = new dict_item()
                    {
                        key_v = key_v,
                        fif_id = fif_id,
                    };

                    var resultDictItem =
                        DictItem.FindId(_dbLink, dict_item.key_v, dict_item.fif_id);
                    #endregion

                    #region fisd_item
                    fisd_item fisd_item = new fisd_item()
                    {
                        dat_from = x.Value.Dictionary.First().Key,
                        fisd_id = ffd.fisd_id,
                        val = resultDictItem.key_v
                    };
                    #endregion

                    FisdItem.Insert(_dbLink, fisd_item);
                    transaction.Commit();
                }
                catch(Exception ex)
                {
                    transaction.Rollback();
                }
            }
        }

        private void Save(Dictionary<KeyValuePair<PortfolioPosition, Enum>, ScalarStr> storage)
        {
            foreach(var x in storage)
            {
                var transaction = _dbLink.GetConnection().BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    // найдем тип финансового инструмента
                    var finIdent = x.Key.Key.Ident;
                    var finType = x.Key.Key.FinType;
                    string finTypeIdent = _mapping.GetAI(finType);

                    #region финансовый инструмент
                    fin_instrument finInstrument = new fin_instrument()
                    {
                        ident = finIdent,
                        title = finIdent,
                        ft_id = int.Parse(finTypeIdent)
                    };

                    if (FinInstrument.FindId(_dbLink, finInstrument.ident) == null)
                        FinInstrument.Insert(_dbLink, finInstrument);

                    finInstrument = FinInstrument.FindId(_dbLink, finInstrument.ident);

                    #endregion

                    #region Data_source
                    data_source dataSource = new data_source()
                    {
                        ident = _providerParams[SCALAR].ToString(),
                    };

                    dataSource = DataSource.FindId(_dbLink, dataSource.ident);
                    #endregion

                    #region fisd_id
                    ffd ffd = new ffd()
                    {
                        ds_id = dataSource.ds_id,
                        fi_id = finInstrument.fi_id,
                        fif_id = int.Parse(_mapping.GetAI(x.Key.Value))
                    };

                    if (FFD.Find(_dbLink, ffd.fi_id, ffd.ds_id, ffd.fif_id) == null)
                        FFD.Insert(_dbLink, ffd);
                    ffd = FFD.Find(_dbLink, ffd.fi_id, ffd.ds_id, ffd.fif_id);
                    #endregion

                    fisd_str fisdStr = new fisd_str()
                    {
                        fisd_id = ffd.fisd_id,
                        dat_from = x.Value.Dictionary.First().Key,
                        val = x.Value.Dictionary.First().Value
                    };

                    FisdStr.Insert(_dbLink, fisdStr);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                }
            }
        }

        private void Save(Dictionary<KeyValuePair<PortfolioPosition, Enum>, ScalarNum> storage)
        {
            foreach(var x in storage)
            {
                var transaction = _dbLink.GetConnection().BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    // найдем тип финансового инструмента
                    var finIdent = x.Key.Key.Ident;
                    var finType = x.Key.Key.FinType;
                    string finTypeIdent = _mapping.GetAI(finType);

                    #region финансовый инструмент
                    fin_instrument finInstrument = new fin_instrument()
                    {
                        ident = finIdent,
                        title = finIdent,
                        ft_id = int.Parse(finTypeIdent)
                    };

                    if (FinInstrument.FindId(_dbLink, finInstrument.ident) == null)
                        FinInstrument.Insert(_dbLink, finInstrument);

                    finInstrument = FinInstrument.FindId(_dbLink, finInstrument.ident);

                    #endregion

                    #region Data_source
                    data_source dataSource = new data_source()
                    {
                        ident = _providerParams[SCALAR].ToString(),
                    };

                    dataSource = DataSource.FindId(_dbLink, dataSource.ident);
                    #endregion

                    #region fisd_id
                    ffd ffd = new ffd()
                    {
                        ds_id = dataSource.ds_id,
                        fi_id = finInstrument.fi_id,
                        fif_id = int.Parse(_mapping.GetAI(x.Key.Value))
                    };

                    if (FFD.Find(_dbLink, ffd.fi_id, ffd.ds_id, ffd.fif_id) == null)
                        FFD.Insert(_dbLink, ffd);
                    ffd = FFD.Find(_dbLink, ffd.fi_id, ffd.ds_id, ffd.fif_id);
                    #endregion

                    fisd_num fisdNum = new fisd_num()
                    {
                        dat_from = x.Value.Dictionary.First().Key,
                        fisd_id = ffd.fisd_id,
                        val = x.Value.Dictionary.First().Value
                    };

                    FisdNum.Insert(_dbLink, fisdNum);
                    transaction.Commit();
                }
                catch(Exception ex)
                {
                    transaction.Rollback();
                }
            }
        }

        private void Save(Dictionary<PortfolioPosition, TimeSeries> storage)
        {
            foreach(var x in storage)
            {
                var transaction = _dbLink.GetConnection().BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    // найдем тип финансового инструмента
                    var finIdent = x.Key.Ident;
                    var finType = x.Key.FinType;
                    string finTypeIdent = _mapping.GetAI(finType);

                    #region финансовый инструмент
                    fin_instrument finInstrument = new fin_instrument()
                    {
                        ident = finIdent,
                        title = finIdent,
                        ft_id = int.Parse(finTypeIdent)
                    };

                    if (FinInstrument.FindId(_dbLink, finInstrument.ident) == null)
                        FinInstrument.Insert(_dbLink, finInstrument);

                    finInstrument = FinInstrument.FindId(_dbLink, finInstrument.ident);
                    #endregion

                    #region Data_source
                    data_source dataSource = new data_source()
                    {
                        ident = _providerParams[SCALAR].ToString(),
                    };

                    dataSource = DataSource.FindId(_dbLink, dataSource.ident);
                    #endregion

                    #region fisd_id
                    ffd ffd = new ffd()
                    {
                        ds_id = dataSource.ds_id,
                        fi_id = finInstrument.fi_id,
                        fif_id = int.Parse(_mapping.GetAI(x.Value.Attribute))
                    };

                    if (FFD.Find(_dbLink, ffd.fi_id, ffd.ds_id, ffd.fif_id) == null)
                        FFD.Insert(_dbLink, ffd);
                    ffd = FFD.Find(_dbLink, ffd.fi_id, ffd.ds_id, ffd.fif_id);
                    #endregion

                    #region TimeSeries
                    foreach(var z in x.Value.Series)
                    {
                        fisd_dq fisd_dq = new fisd_dq()
                        {
                            dat = z.Key,
                            val = z.Value,
                            fisd_id = ffd.fisd_id
                        };

                        if (FisdDq.FindId(_dbLink, fisd_dq.fisd_id, fisd_dq.dat) == null)
                            FisdDq.Insert(_dbLink, fisd_dq);
                        else
                            FisdDq.Update(_dbLink, fisd_dq);
                    }
                    #endregion

                    transaction.Commit();
                }
                catch(Exception ex)
                {
                    transaction.Rollback();
                }
            }
        }

        public void Dispose()
        {
            _dbLink.Close();
        }
    }
}
