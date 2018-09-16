using Core.Mir.BaseTypes;
using Core.Mir.Enumerations;
using Core.Mir.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
using System.IO;

using CalculateVolat;
using ConvertData;
using Algorithms.General.CalculateVar;
using Algorithms.General.CalculateYield;
using Core.Mir;


namespace MURRDesktop
{
    public partial class Form1 : Form
    {
        private const string IDENT = "Идентификатор";
        private const string DESCRIPTION = "Описание";
        private const string TYPE = "Тип";
        private const string VALUE = "Значение";
        DataTable dataTable = null;

        private Dictionary<string, CalculationOneData> actions = new Dictionary<string, CalculationOneData>()
            {
                {"debt_zcc_rub", new ConvertData.Convert() },
                {"Ruonia", new ConvertData.Convert() },
                {"Currencies", new ConvertData.Convert() },
                {"rates", new ConvertData.Convert() },
                {"volat", new Volat() },
                {"var", new Algorithms.General.CalculateVar.Var() },
                {"yield", new Algorithms.General.CalculateYield.Yield() },
            };

        private string _connection =
            ConfigurationManager.AppSettings["mirconnection"];
        List<string> curves = new List<string>();
        Dictionary<string, List<string>> subCurves = new Dictionary<string, List<string>>();

        private DataBaseLink.DbLink _dbLink = null;
        private Dictionary<string, string> currencies = new Dictionary<string, string>();
        private DataProvider.Input.CombinedProvider.Provider _provider = null;
        List<PortfolioPosition> positions = null;
        List<PortfolioPosition> _cachedPositions = null;
        public IMarketProvider GetMoex()
        {
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
                {ScalarAttribute.DetailedType, "6"},
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

                #region Детализированный уровень
                        {new KeyValuePair<Type,string>(typeof(FinTypeDetailedLevel),"ORDINARY_EQUITY"), FinTypeDetailedLevel.OrdinaryStock},
                        {new KeyValuePair<Type,string>(typeof(FinTypeDetailedLevel),"CORPORATE_BOND"), FinTypeDetailedLevel.CorporateBond},
                        {new KeyValuePair<Type,string>(typeof(FinTypeDetailedLevel),"ETF"), FinTypeDetailedLevel.ETF},
                        {new KeyValuePair<Type,string>(typeof(FinTypeDetailedLevel),"PREFERRED_EQUITY"), FinTypeDetailedLevel.PreferredStock},
                        {new KeyValuePair<Type,string>(typeof(FinTypeDetailedLevel),"FEDERAL_LOAN_BOND"), FinTypeDetailedLevel.FederalLoanBond},

                        {new KeyValuePair<Type,string>(typeof(FinTypeDetailedLevel),"PIE_OF_CLOSED_MUTUAL_FUND"), FinTypeDetailedLevel.PieOfClosedMutualFund},
                        {new KeyValuePair<Type,string>(typeof(FinTypeDetailedLevel),"PIE_OF_INTERVAL_MUTUAL_FUND"), FinTypeDetailedLevel.PieOfIntervalMutualFund},
                        {new KeyValuePair<Type,string>(typeof(FinTypeDetailedLevel),"REGIONAL_BOND"), FinTypeDetailedLevel.RegionalBond},
                        {new KeyValuePair<Type,string>(typeof(FinTypeDetailedLevel),"PIE_OF_OPENED_MUTUAL_FUND"), FinTypeDetailedLevel.PieOfOpenedMutualFund},
                        {new KeyValuePair<Type,string>(typeof(FinTypeDetailedLevel),"MFO_BOND"), FinTypeDetailedLevel.MfoBond},

                        {new KeyValuePair<Type,string>(typeof(FinTypeDetailedLevel),"EXCHANGE_TRADED_BOND"), FinTypeDetailedLevel.ExchangeTradedBond},
                        {new KeyValuePair<Type,string>(typeof(FinTypeDetailedLevel),"MORTGAGE_CERTIFICATE"), FinTypeDetailedLevel.MortgageCertificate},
                        {new KeyValuePair<Type,string>(typeof(FinTypeDetailedLevel),"MUNICIPAL_BOND"), FinTypeDetailedLevel.MunicipalBond},
                        {new KeyValuePair<Type,string>(typeof(FinTypeDetailedLevel),"CLEARING_PARTICIPATION_CERTIFICATE"), FinTypeDetailedLevel.ClearingParticipationCertificate},
                        {new KeyValuePair<Type,string>(typeof(FinTypeDetailedLevel),"DEPOSITARY_RECEIPT"), FinTypeDetailedLevel.DepositaryReceipt},
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
            string connection = ConfigurationManager.AppSettings["mirconnection"];
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
                {Currencies.GBP, "GBP"},
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

        public Form1()
        {
            InitializeComponent();

            var iConnection = DataBaseLink.Fabricate.CreateConnection(_connection, DataBaseLink.ConnectionType.Npgsql);
            _dbLink = new DataBaseLink.DbLink(iConnection);
            IMarketProvider moex = GetMoex();
            IMarketProvider cbr = GetCbr();
            _provider =
                new DataProvider.Input.CombinedProvider.Provider(new List<IMarketProvider>()
                    {
                        moex,
                        cbr
                    });

            LoadDataFinInstruments();
            Paint();
            LoadDataCurrencies();
            LoadAttributes();
            LoadDictionary();
            LoadPercentCurves();
            LoadCalculations();
        }

        private void LoadCalculations()
        {
            //загружаем все выпоненные задачи
            LoadUsedTasks();
            //загружаем все невыполненные задачи
            LoadUnUsedTasks();
            //загружаем все задачи, которые необходимо запустить и выполнить
            LoadWaitingTasks();
            //Загружаем калькуляторы 
            LoadCalculationsData();
        }

        private void LoadCalculationsData()
        {
            cbCalculations.Items.Clear();

            foreach(var x in actions)
            {
                cbCalculations.Items.Add(x.Key);
            }

            cbCalculations.SelectedItem = cbCalculations.Items[0];
        }

        private void LoadWaitingTasks()
        {
            rtbWaitingTasks.Text = string.Empty;

            string fileName = ConfigurationManager.AppSettings["list_task"];
            if(File.Exists(fileName))
            {
                string[] strings = File.ReadAllLines(fileName);
                foreach(var x in strings)
                {
                    rtbWaitingTasks.Text += x + "\r\n";
                }
            }
        }

        private void LoadUnUsedTasks()
        {
            rtbUndoneTasks.Text = string.Empty;

            string fileName = ConfigurationManager.AppSettings["list_failed_task"];
            if(File.Exists(fileName))
            {
                string[] strings = File.ReadAllLines(fileName);
                foreach(var x in strings)
                {
                    rtbUndoneTasks.Text += x + "\r\n";
                }
            }
        }

        private void LoadUsedTasks()
        {
            rtbUsedTasks.Text = string.Empty;

            string fileName = ConfigurationManager.AppSettings["list_done_task"];
            if(File.Exists(fileName))
            {
                string[] strings = File.ReadAllLines(fileName);
                foreach(var x in strings)
                {
                    rtbUsedTasks.Text += x + "\r\n";
                }
            }
        }

        private void LoadPercentCurves()
        {
            curves = new List<string>();
            using(NpgsqlConnection connection = new NpgsqlConnection(_connection))
            {
                connection.Open();
                using(NpgsqlCommand command = new NpgsqlCommand())
                {
                    command.Connection = connection;
                    string query = "select t.ident as ident from curves t";
                    command.CommandText = query;
                    using(NpgsqlDataReader dataReader = command.ExecuteReader())
                    {
                        while(dataReader.Read())
                        {
                            string ident = dataReader["ident"].ToString();
                            curves.Add(ident);
                        }
                    }
                }
            }

            foreach(var x in curves)
            {
                List<string> strings = new List<string>();
                using(NpgsqlConnection connection = new NpgsqlConnection(_connection))
                {
                    connection.Open();
                    using(NpgsqlCommand command = new NpgsqlCommand())
                    {
                        command.Connection = connection;
                        string query =
                            string.Format(@"select fi.ident as ident from 
                                curve_list cl join curves c on cl.cur_id = c.cur_id
                                join fin_instrument fi on fi.fi_id = cl.fi_id
                                where c.ident = '{0}'", x);
                        command.CommandText = query;
                        using(NpgsqlDataReader dataReader = command.ExecuteReader())
                        {
                            while(dataReader.Read())
                            {
                                string ident = dataReader["ident"].ToString();
                                strings.Add(ident);
                            }
                        }
                    }
                }
                subCurves.Add(x, strings);
            }

            foreach(var x in subCurves)
            {
                TreeNode treeNode = tvPercentCurve.Nodes.Add(x.Key, x.Key);
                foreach(var z in x.Value)
                {
                    treeNode.Nodes.Add(z, z);
                }
            }
        }

        private void LoadDataFinInstruments()
        {
            if (_cachedPositions == null)
            {
                List<FinType> finTypes = new List<FinType>()
            {
                FinType.Bond,
                FinType.Certificate,
                FinType.DepositaryReceipt,
                FinType.Equity,
                FinType.Fund
            };
                positions = _provider.GetAllPositions(finTypes);
                _cachedPositions = positions.ToList();
            }
            else
            {
                positions = _cachedPositions;
            }
        }

        private void Paint()
        {
            tvFinInstruments.Nodes.Clear();
            //достаем финансовые инструменты и сортируем по папкам 
            List<FinType> finTypes = new List<FinType>()
            {
                FinType.Bond,
                FinType.Certificate,
                FinType.DepositaryReceipt,
                FinType.Equity,
                FinType.Fund
            };

            Dictionary<FinType, string> types = new Dictionary<FinType, string>()
            {
                {FinType.Bond, "Облигации"},
                {FinType.Certificate, "Сертификаты"},
                {FinType.DepositaryReceipt, "Депозитарные распискы"},
                {FinType.Equity, "Акции"},
                {FinType.Fund, "Фонды"},
            };

            Dictionary<FinTypeDetailedLevel, string> finTypeDetailedLevels =
                new Dictionary<FinTypeDetailedLevel, string>();

            foreach (var z in Enum.GetValues(typeof(FinTypeDetailedLevel)).OfType<FinTypeDetailedLevel>())
            {
                var type = z.GetType();
                var memInfo = type.GetMember(z.ToString());
                var attribute = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                var description = ((DescriptionAttribute)attribute[0]).Description;
                finTypeDetailedLevels.Add(z, description);
            }

            foreach (var x in types)
            {
                int amount = positions.Count(p => p.FinType == x.Key);
                if (amount == 0)
                    continue;

                string key = string.Format("{1} ({0})", amount, x.Value);
                
                TreeNode root = tvFinInstruments.Nodes.Add(key, key);

                //у нас есть тип данных
                var secondLevelPositions = positions.Where(z => z.FinType == x.Key);

                Dictionary<FinTypeDetailedLevel, List<PortfolioPosition>> dictionary =
                    new Dictionary<FinTypeDetailedLevel, List<PortfolioPosition>>();

                foreach (var zz in secondLevelPositions)
                {
                    FinTypeDetailedLevel detailLevel = _provider.Get<FinTypeDetailedLevel>(zz,
                        ScalarAttribute.DetailedType,
                        DateTime.Today,
                        FinTypeDetailedLevel.Default);

                    if (dictionary.ContainsKey(detailLevel))
                        dictionary[detailLevel].Add(zz);
                    else
                        dictionary.Add(detailLevel, new List<PortfolioPosition>() { zz });
                }

                foreach (var z in dictionary)
                {
                    int secondAmount = z.Value.Count();
                    if (secondAmount == 0)
                        continue;
                    string secondKey = string.Format("{1} ({0})", secondAmount, finTypeDetailedLevels[z.Key]);
                    TreeNode secondRoot = root.Nodes.Add(secondKey, secondKey);
                    foreach (var y in z.Value)
                    {
                        secondRoot.Nodes.Add(y.Ident, y.Ident);
                    }
                }
            }
        }

        private void LoadDataCurrencies()
        {          
            //foreach(var x in curs)
            //{
            //    tvCurrencies.Nodes.Add(x.Value);
            //}

            using(NpgsqlConnection connection = new NpgsqlConnection(_connection))
            {
                connection.Open();
                using(NpgsqlCommand command = new NpgsqlCommand())
                {
                    string query = "select t.ident as ident, t.title as title from fin_instrument t where t.ft_id = 20";
                    command.Connection = connection;
                    command.CommandText = query;
                    using(NpgsqlDataReader dataReader = command.ExecuteReader())
                    {
                        while(dataReader.Read())
                        {
                            currencies.Add(dataReader["ident"].ToString(), dataReader["title"].ToString());
                        }
                    }
                }
            }

            foreach(var x in currencies)
            {
                tvCurrencies.Nodes.Add(x.Key, x.Value);
            }
        }

        private void LoadAttributes()
        {
            string query = "select t.fif_id as Id, t.Ident as Идентификатор, t.title as Наименование, t.description as Описание from fin_field t";
            using (NpgsqlConnection connection = new NpgsqlConnection(_connection))
            {
                connection.Open();
                NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(query, connection);
                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet, "Атрибуты");
                dgvAttributes.DataSource = dataSet.Tables[0]; // dataset
            }
        }      

        private void LoadDictionary()
        {
            string query = @"select di.key_v as Идентификатор, di.val as Описание, t.title as Множество, di.ord_id as Номер 
                                from fin_field t join dict_item di on t.fif_id = di.fif_id
                                order by t.fif_id, di.ord_id";
            using (NpgsqlConnection connection = new NpgsqlConnection(_connection))
            {
                connection.Open();
                NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(query, connection);
                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet, "Словарь");
                //dgvAttributes.DataSource = dataSet.Tables[0];
                dgvDictionary.DataSource = dataSet.Tables[0]; // dataset

            }
        }

        private void tvFinInstruments_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            
        }

        private void tvFinInstruments_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                try
                {
                    string ident = e.Node.Text;
                    if (positions.FirstOrDefault(z => z.Ident == ident) != null)
                    {
                        ScalarAndTimeSeries form = new ScalarAndTimeSeries(ident);
                        form.ShowDialog();
                    }
                }
                // If the file is not found, handle the exception and inform the user.
                catch (System.ComponentModel.Win32Exception)
                {
                    MessageBox.Show("File not found.");
                }
            }
        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void tvFinInstruments_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void btFind_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tbSearch.Text))
            {
                positions = _cachedPositions.Where(z => z.Ident.Contains(tbSearch.Text)).ToList();
                Paint();
            }
            else
            {
                LoadDataFinInstruments();
                Paint();
            }
        }

        private void tvCurrencies_NodeMouseDoubleClick(object sender,
            TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                PaintTvCurrencies(e.Node);
            }
        }

        private void PaintTvCurrencies(TreeNode treeNode)
        {
            try
            {
                string ident = treeNode.Name;
                using (NpgsqlConnection connection = new NpgsqlConnection(_connection))
                {
                    connection.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        command.Connection = connection;
                        string query = string.Format(
                            @"SELECT fd.dat as DAT, fd.val as VAL
                                                from fin_instrument fi join
	                                                ffd t on t.fi_id = fi.fi_id join
	                                                fisd_dq fd on fd.fisd_id = t.fisd_id join
	                                                data_source ds on ds.ds_id = t.ds_id
	                                                and fi.ident = '{0}'
	                                                and ds.ident = '{1}'
	                                                and t.fif_id = 3",
                                    ident,
                                    "CBR");
                        command.CommandText = query;

                        using (NpgsqlDataReader dataReader = command.ExecuteReader())
                        {
                            Dictionary<DateTime, decimal> curvalues = new Dictionary<DateTime, decimal>();

                            while (dataReader.Read())
                            {
                                DateTime dateTime = (DateTime)dataReader["DAT"];
                                decimal val = (Decimal)dataReader["VAL"];
                                curvalues.Add(dateTime, val);
                            }

                            PaintWindow windows = new PaintWindow(curvalues)
                            {
                                Text = treeNode.Text
                            };
                            windows.ShowDialog();
                        }
                    }
                }
            }
            // If the file is not found, handle the exception and inform the user.
            catch (System.ComponentModel.Win32Exception)
            {
                MessageBox.Show("File not found.");
            }
        }

        private void tvPercentCurve_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void tvPercentCurve_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            
        }

        private void tvPercentCurve_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if(e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                PaintTvPercent(e.Node);
            }
            else
            {

            }
        }

        private void tvPercentCurve_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                TreeNode treeNode = tvPercentCurve.SelectedNode;
                PaintTvPercent(treeNode);
            }
        }

        private void PaintTvPercent(TreeNode treeNode)
        {
            if (treeNode.Nodes.Count != 0)
            {
                Dictionary<decimal, decimal> dictionary = new Dictionary<decimal, decimal>();
                //child
                string query = string.Format(@"select cl.term as term, fd.val as val
                                    from curve_list cl join curves c on cl.cur_id = c.cur_id
                                    join ffd ffd on ffd.fi_id = cl.fi_id
                                    join fisd_dq fd on fd.fisd_id = ffd.fisd_id
                                    where c.ident = '{0}'
                                    and ffd.fif_id = 3
                                    and dat = to_date('{1}', 'dd.mm.yyyy')",
                    treeNode.Text,
                    dtpActualDate.Value.ToString("dd.MM.yyyy"));
                using (NpgsqlConnection connection = new NpgsqlConnection(_connection))
                {
                    connection.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = query;
                        using (NpgsqlDataReader dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                decimal term = (decimal)dataReader["term"];
                                decimal val = (decimal)dataReader["val"];
                                dictionary.Add(term, val);
                            }
                        }
                    }
                }

                if (dictionary.Count != 0)
                {
                    var t = new PaintWindow(dictionary) { Text = treeNode.Text };
                    t.ShowDialog();
                }
            }
            else
            {
                PortfolioPosition position = new BalancePosition(treeNode.Text, FinType.PercentCurve);
                TimeSeries timeSeries = _provider.GetTimeSeries(position, TimeSeriesAttribute.Close);
                if (timeSeries != null || timeSeries.Series.Count != 0)
                {
                    var t = new PaintWindow(timeSeries.Series.ToDictionary(z => z.Key, z => z.Value));
                    t.ShowDialog();
                }
            }
        }

        private void tvFinInstruments_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                try
                {
                    string ident = tvFinInstruments.SelectedNode.Text;
                    if (positions.FirstOrDefault(z => z.Ident == ident) != null)
                    {
                        ScalarAndTimeSeries form = new ScalarAndTimeSeries(ident);
                        form.ShowDialog();
                    }
                }
                // If the file is not found, handle the exception and inform the user.
                catch (System.ComponentModel.Win32Exception)
                {
                    MessageBox.Show("File not found.");
                }

            }
        }

        private void tvCurrencies_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void tvCurrencies_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void tvCurrencies_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                TreeNode treeNode = tvCurrencies.SelectedNode;
                PaintTvCurrencies(treeNode);
            }
        }

        private void tableLayoutPanel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel8_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string command = string.Format("{0} {1}", 
                cbCalculations.SelectedItem.ToString(),
                dateTimePicker1.Value.ToString("yyyy.MM.dd"));

            if(dataTable!= null)
            {
                string attributes = string.Empty;
                foreach(DataRow x in dataTable.Rows)
                {
                    attributes = string.Format("{0}:{1}", x[IDENT].ToString(), x[VALUE].ToString());
                    command += " " + attributes;
                }
            }

            command = command.Trim();
            string fileName = ConfigurationManager.AppSettings["list_task"];
            File.AppendAllLines(fileName, new List<string>(){command});

            LoadWaitingTasks();
        }

        private void cbCalculations_SelectedValueChanged(object sender, EventArgs e)
        {
            if(cbCalculations.SelectedItem != null &&
                !string.IsNullOrEmpty(cbCalculations.SelectedItem.ToString()))
            {
                string key = cbCalculations.SelectedItem.ToString();
                
                var calculation = actions[key];
                calculation.GetParams();
                DataColumn identColumn = new DataColumn(IDENT, typeof(string));
                DataColumn descriptionColumn = new DataColumn(DESCRIPTION, typeof(string));
                DataColumn typeColumn = new DataColumn(TYPE, typeof(string));
                DataColumn valueColumn = new DataColumn(VALUE, typeof(string));
                dataTable = new DataTable();
                dataTable.Columns.AddRange(new DataColumn[] { 
                    identColumn,
                    descriptionColumn,
                    typeColumn,
                    valueColumn});
                
                foreach(var x in calculation.GetParams())
                {
                    var newRow = dataTable.NewRow();
                    newRow[IDENT] = x.Ident;
                    newRow[DESCRIPTION] = x.Description;
                    newRow[TYPE] = x.ParamType.ToString();
                    newRow[VALUE] = x.Value;
                    dataTable.Rows.Add(newRow);
                }

                dataGridView1.DataSource = dataTable;
            }
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            //загружаем все выпоненные задачи
            LoadUsedTasks();
            //загружаем все невыполненные задачи
            LoadUnUsedTasks();
            //загружаем все задачи, которые необходимо запустить и выполнить
            LoadWaitingTasks();
        }

        private void RunCalculation_Click(object sender, EventArgs e)
        {
            string commandExe = ConfigurationManager.AppSettings["run_program"];
            System.Diagnostics.Process.Start(commandExe);
            _provider.ClearCache();
            _provider.Dispose();

            curves = new List<string>();
            subCurves = new Dictionary<string, List<string>>();
            currencies = new Dictionary<string, string>();
            _dbLink = null;
            positions = null;
            _cachedPositions = null;
            var iConnection = DataBaseLink.Fabricate.CreateConnection(_connection, DataBaseLink.ConnectionType.Npgsql);
            _dbLink = new DataBaseLink.DbLink(iConnection);
            IMarketProvider moex = GetMoex();
            IMarketProvider cbr = GetCbr();
            _provider =
                new DataProvider.Input.CombinedProvider.Provider(new List<IMarketProvider>()
                    {
                        moex,
                        cbr
                    });

            LoadDataFinInstruments();
            Paint();
            LoadDataCurrencies();
            LoadAttributes();
            LoadDictionary();
            LoadPercentCurves();
            LoadCalculations();
        }
    }
}
