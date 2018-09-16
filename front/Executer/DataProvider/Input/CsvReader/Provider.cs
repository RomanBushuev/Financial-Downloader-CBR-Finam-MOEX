using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Core.Mir.BaseTypes;
using Core.Mir.Enumerations;
using Core.Mir.Interfaces;
using ExcelDataReader;
using ExcelDataReader.Core;
using System.IO;
using System.Data;

namespace DataProvider.Input.CsvReader
{
    public class Provider: IMarketProvider, IGetParams, ISetParams
    {
        #region field
        private string _ident = string.Empty;
        private Mapping _mapping;
        private string _connection;
        private DateTime _reportDate;
        private Encoding _encoding;
        private int _skipLines;
        private char _delimeter;
        private string _endOfLine;
        private List<KeyValuePair<string, Type>> _pages;
        private Dictionary<KeyValuePair<PortfolioPosition, Enum>, object> _cahce = new Dictionary<KeyValuePair<PortfolioPosition, Enum>, object>();
        private Dictionary<ScalarAttribute, ParamType> _sp = new Dictionary<ScalarAttribute, ParamType>();
        private bool _isCached = false;
        #endregion

        #region Конструкторы
        public Provider(string connection,
            Mapping mapping,
            List<KeyValuePair<string, Type>> pages,
            Dictionary<ScalarAttribute, ParamType> sp,
            string encoding = "windows-1251", 
            int skipLines = 0, 
            char delimeter = ';',
            string endOfLine = "\r",
            string ident = "")
            :this(connection,
            mapping,
            DateTime.Today,
            pages,
            sp,
            encoding,
            skipLines,
            delimeter,
            endOfLine,
            ident)
        {

        }

        public Provider(string connection,
            Mapping mapping,
            DateTime reportDate,
            List<KeyValuePair<string, Type>> pages,
            Dictionary<ScalarAttribute, ParamType> sp,
            string encoding = "windows-1251",
            int skipLines = 0,
            char delimeter = ';',
            string endOfLine = "\r",
            string ident = "")
        {
            _connection = connection;
            _mapping = mapping;
            _pages = pages;
            _reportDate = reportDate;
            _sp = sp;
            _encoding = Encoding.GetEncoding(encoding);
            _skipLines = skipLines;
            _delimeter = delimeter;
            _endOfLine = endOfLine;
            _ident = ident;
        }
        #endregion

        public IMapping GetIMapping()
        {
            return _mapping;
        }

        public ScalarDate GetScalarDate(PortfolioPosition position, ScalarAttribute attribute)
        {
            if(_mapping.FindAI(attribute))
            {
                if (!_isCached)
                    Download();

            }
            return null;
        }

        private void ExistFile(string filename)
        {
            if (!File.Exists(_connection))
            {
                string message = string.Format("Файла не существует по указанному пути:{0}", _connection);
                throw new Exception(message);
            }
        }
        private string CopyFile(string filename)
        {
            string copy = "COPY";
            string fileNameCopy = Path.GetFileNameWithoutExtension(_connection) + copy;
            string onlyFileName = Path.GetFileNameWithoutExtension(_connection);
            string extension = Path.GetExtension(_connection);
            string path = new string(_connection.Take(_connection.Count() - onlyFileName.Count() - extension.Count()).ToArray());
            string newFileName = path + fileNameCopy + extension;
            if (File.Exists(filename))
            {
                if(File.Exists(newFileName))
                {
                    File.Delete(newFileName);
                }
                File.Copy(filename, newFileName);
            }

            return newFileName;
        }

        private void TransformFile(string filename)
        {
            var intermediateResult = File.ReadAllLines(filename,
                _encoding).Skip(_skipLines);
            File.WriteAllLines(filename, intermediateResult);
        }

        public DataSet ReadDataTables(string filename)
        {
            using(FileStream stream = File.Open(filename, FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader excelReader = ExcelReaderFactory.CreateCsvReader(stream);
                ExcelDataSetConfiguration configuration = new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                };
                DataSet dataSet = excelReader.AsDataSet(configuration);
                return dataSet;
            }
        }
        public void Download()
        {
            string copyFileName = string.Empty;

            try
            {
                //проверим файл
                ExistFile(_connection);

                //копируем файл отдельно         
                copyFileName = CopyFile(_connection);

                //удаляем необходимое кол-во строк
                TransformFile(copyFileName);

                //начинаем считывать данные из таблицы в кэш
                DataSet dataSet = ReadDataTables(copyFileName);

                foreach(DataTable dataTable in dataSet.Tables)
                {
                    if(_pages.Where(z=>z.Value == typeof(ScalarAttribute))
                        .Select(z=>z.Key)
                        .Contains(dataTable.TableName))
                    {
                        ReadScalarDate(dataTable);
                    }

                    if (_pages.Where(z => z.Value == typeof(TimeSeriesAttribute))
                        .Select(z => z.Key)
                        .Contains(dataTable.TableName))
                    {
                        ReadTimeSeries(dataTable);
                    }

                    if(_pages.Where(z=>z.Value == typeof(CurveRateAttribute))
                        .Select(z=>z.Key)
                        .Contains(dataTable.TableName))
                    {
                        ReadCurve(dataTable);
                    }

                    if (_pages.Where(z => z.Value == typeof(CashFlowAttribute))
                        .Select(z => z.Key)
                        .Contains(dataTable.TableName))
                    {
                        ReadCashFlow(dataTable);
                    }

                }

                _isCached = true;
            }
            catch (Exception exception)
            {
                string message = string.Format("Ошибка произошла при чтении данных из csv файла: {0}. {1}", _connection, exception.Message);
                throw new Exception(message);
            }
            finally
            {
                if(File.Exists(copyFileName))
                {
                    try
                    {
                        File.Delete(copyFileName);
                    }
                    catch(Exception ex)
                    {

                    }
                }
            }

           
        }

        private void ReadCashFlow(DataTable dataTable)
        {
            KeyValuePair<PortfolioPosition, Enum> key =
                new KeyValuePair<PortfolioPosition, Enum>();
            Dictionary<KeyValuePair<PortfolioPosition, Enum>, CashFlow> cashFlows = new Dictionary<KeyValuePair<PortfolioPosition, Enum>, CashFlow>();
            
            foreach(DataRow dataRow in dataTable.Rows)
            {
                string ident = _ident;
                FinType finType = FinType.Default;
                DateTime actualDate = _reportDate;

                #region actualDate
                if (_mapping.FindAI(PositionAttribute.ActualDate))
                {
                    string columnName = _mapping.GetAI(PositionAttribute.ActualDate);
                    //Она есть в таблице, но ее нельзя распарсить
                    if (string.IsNullOrEmpty(dataRow[columnName].ToString()))
                        continue;

                    //у нас в таблице дата есть, но она может быть в формате
                    if (dataRow[columnName].GetType() == typeof(string))
                    {
                        string[] splits = dataRow[columnName].ToString().Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                        int day = int.Parse(splits[0]);
                        int month = int.Parse(splits[1]);
                        int year = int.Parse(splits[2]);
                        actualDate = new DateTime(year, month, day);
                    }
                    //просто берем 
                    if (dataRow[columnName].GetType() == typeof(DateTime))
                    {
                        actualDate = (DateTime)dataRow[columnName];
                    }
                }
                #endregion

                #region ident
                if (_mapping.FindAI(PositionAttribute.Ident))
                {
                    string columnName = _mapping.GetAI(PositionAttribute.Ident);
                    if (string.IsNullOrEmpty(dataRow[columnName].ToString()))
                        continue;
                    if (dataRow[columnName].GetType() == typeof(string))
                    {
                        ident = dataRow[columnName].ToString().Trim();
                    }
                }
                #endregion

                #region type
                if (_mapping.FindAI(PositionAttribute.Type))
                {
                    string columnName = _mapping.GetAI(PositionAttribute.Type);
                    if (string.IsNullOrEmpty(dataRow[columnName].ToString()))
                        continue;
                    if (dataRow[columnName].GetType() == typeof(string))
                    {
                        string typeIdent = dataRow[columnName].ToString().Trim();
                        finType = _mapping.Get<FinType>(typeIdent, FinType.Default);
                    }
                }
                #endregion

                DateTime dateTime = DateTime.Today;
                decimal? val = null;
                CashFlowAttribute cfa = CashFlowAttribute.Default;

                #region parse
                foreach (DataColumn dataColumn in dataTable.Columns)
                {
                    if (_mapping.FindAI(dataColumn.ColumnName))
                    {
                        Enum enumeration = _mapping.GetAI(dataColumn.ColumnName);
                        if (enumeration.GetType() == typeof(ScalarAttribute)
                                    && _sp.ContainsKey((ScalarAttribute)enumeration))
                        {
                            ParamType paramType = _sp[(ScalarAttribute)enumeration];

                            if(paramType == ParamType.DateTime)
                            {
                                dateTime = DateTime.ParseExact(dataRow[dataColumn].ToString(),
                                    "dd.MM.yyyy",
                                    System.Globalization.CultureInfo.InvariantCulture);
                            }

                            if(paramType == ParamType.String)
                            {
                                if (dataRow[dataColumn].ToString()
                                    == "COUPON")
                                    cfa = CashFlowAttribute.Coupon;
                                if (dataRow[dataColumn].ToString()
                                    == "REDEMPTION")
                                    cfa = CashFlowAttribute.Redemption;
                            }

                            if(paramType == ParamType.Decimal)
                            {
                                bool isGood = false;

                                if(dataRow[dataColumn] == null ||
                                    string.IsNullOrEmpty(dataRow[dataColumn].ToString()))
                                    continue;

                                try
                                {
                                    val = Convert.ToDecimal(dataRow[dataColumn].ToString());
                                    isGood = true;
                                }
                                catch
                                {

                                }

                                try
                                {
                                    if (!isGood)
                                    {
                                        string otherValue = dataRow[dataColumn].ToString();
                                        if (otherValue.Contains(','))
                                            otherValue = otherValue.Replace(',', '.');
                                        else
                                            otherValue = otherValue.Replace('.', ',');
                                        val = Convert.ToDecimal(otherValue);
                                        isGood = true;
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                    }
                }
                #endregion

                BalancePosition position = new BalancePosition(ident, finType);
                key = new KeyValuePair<PortfolioPosition,Enum>(position, cfa);
                
                if(_cahce.Count == 1)
                {
                    bool t = _cahce.FirstOrDefault().Key.Key.Equals(key.Key);
                    bool t2 = _cahce.FirstOrDefault().Key.Value.Equals(key.Value);
                }

                if(_cahce.FirstOrDefault(z=>z.Key.Key.Equals(key.Key) && z.Key.Value.Equals(key.Value)).Value != null)
                {
                    key = _cahce.FirstOrDefault(z => z.Key.Key.Equals(key.Key) && z.Key.Value.Equals(key.Value)).Key;
                    ((CashFlow)_cahce[key]).Add(dateTime, val);
                }
                else
                {
                    CashFlow cashFlow = new CashFlow(cfa);
                    cashFlow.Add(dateTime, val);
                    _cahce.Add(key, cashFlow);
                }
            }
        }

        private void ReadTimeSeries(DataTable dataTable)
        {
            //Находим финансовый инструмент 
            //находим 
            foreach(DataRow dataRow in dataTable.Rows)
            {
                string ident = _ident;
                FinType finType = FinType.Default;
                DateTime actualDate = _reportDate;

                #region actualDate
                if (_mapping.FindAI(PositionAttribute.ActualDate))
                {
                    string columnName = _mapping.GetAI(PositionAttribute.ActualDate);
                    //Она есть в таблице, но ее нельзя распарсить
                    if (string.IsNullOrEmpty(dataRow[columnName].ToString()))
                        continue; ;

                    //у нас в таблице дата есть, но она может быть в формате
                    if (dataRow[columnName].GetType() == typeof(string))
                    {
                        string[] splits = dataRow[columnName].ToString().Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                        int day = int.Parse(splits[0]);
                        int month = int.Parse(splits[1]);
                        int year = int.Parse(splits[2]);
                        actualDate = new DateTime(year, month, day);
                    }
                    //просто берем 
                    if (dataRow[columnName].GetType() == typeof(DateTime))
                    {
                        actualDate = (DateTime)dataRow[columnName];
                    }
                }
                #endregion

                #region ident
                if (_mapping.FindAI(PositionAttribute.Ident))
                {
                    string columnName = _mapping.GetAI(PositionAttribute.Ident);
                    if (string.IsNullOrEmpty(dataRow[columnName].ToString()))
                        continue;
                    if (dataRow[columnName].GetType() == typeof(string))
                    {
                        ident = dataRow[columnName].ToString().Trim();
                    }
                }
                #endregion

                #region type
                if (_mapping.FindAI(PositionAttribute.Type))
                {
                    string columnName = _mapping.GetAI(PositionAttribute.Type);
                    if (string.IsNullOrEmpty(dataRow[columnName].ToString()))
                        continue;
                    if (dataRow[columnName].GetType() == typeof(string))
                    {
                        string typeIdent = dataRow[columnName].ToString().Trim();
                        finType = _mapping.Get<FinType>(typeIdent, FinType.Default);
                    }
                }
                #endregion

                foreach(DataColumn dataColumn in dataTable.Columns)
                {
                    try
                    {
                        if (_mapping.FindAI(dataColumn.ColumnName))
                        {
                            Enum enumeration = _mapping.GetAI(dataColumn.ColumnName);
                            if (enumeration.GetType() == typeof(PositionAttribute))
                            {
                                PositionAttribute pa = (PositionAttribute)enumeration;
                                if (pa == PositionAttribute.Type || pa == PositionAttribute.Ident)
                                    continue;
                            }
                            if (enumeration.GetType() == typeof(TimeSeriesAttribute))
                            {
                                #region TimeSeries
                                decimal value = decimal.Zero;
                                bool isGood = false;

                                try
                                {
                                    value = Convert.ToDecimal(dataRow[dataColumn].ToString());
                                    isGood = true;
                                }
                                catch
                                {

                                }

                                try
                                {
                                    if (!isGood)
                                    {
                                        string otherValue = dataRow[dataColumn].ToString();
                                        if (otherValue.Contains(','))
                                            otherValue = otherValue.Replace(',', '.');
                                        else
                                            otherValue = otherValue.Replace('.', ',');
                                        value = Convert.ToDecimal(otherValue);
                                        isGood = true;
                                    }
                                }
                                catch
                                {

                                }

                                if (!isGood)
                                    continue;

                                SortedDictionary<DateTime, decimal> values =
                                    new SortedDictionary<DateTime, decimal>()
                                    {
                                        {actualDate, value}
                                    };

                                TimeSeries timeSeries = new TimeSeries(values);
                                #endregion

                                #region OneMoreDataReader
                                PortfolioPosition position = new BalancePosition(ident, finType);
                                KeyValuePair<PortfolioPosition, Enum> key =
                                        new KeyValuePair<PortfolioPosition, Enum>(position, enumeration);
                                if (_cahce.ContainsKey(key))
                                {
                                    if (_cahce[key].GetType() == typeof(TimeSeries))
                                    {
                                        TimeSeries moreTimeSeries = (TimeSeries)_cahce[key];
                                        moreTimeSeries.Add(actualDate, value);
                                        _cahce[key] = moreTimeSeries;
                                    }
                                }
                                else
                                {
                                    _cahce.Add(key, timeSeries);
                                }
                                #endregion

                            }
                        }
                    }
                    catch(Exception ex)
                    {
                       
                    }
                }
            }
        }

        private void ReadScalarDate(DataTable dataTable)
        {
            foreach(DataRow dataRow in dataTable.Rows)
            {
                string ident = _ident;
                FinType finType = FinType.Default;
                DateTime actualDate = _reportDate;

                #region actualDate
                if (_mapping.FindAI(PositionAttribute.ActualDate))
                {
                    string columnName = _mapping.GetAI(PositionAttribute.ActualDate);
                    //Она есть в таблице, но ее нельзя распарсить
                    if (string.IsNullOrEmpty(dataRow[columnName].ToString()))
                        continue;

                    //у нас в таблице дата есть, но она может быть в формате
                    if (dataRow[columnName].GetType() == typeof(string))
                    {
                        string[] splits = dataRow[columnName].ToString().Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                        int day = int.Parse(splits[0]);
                        int month = int.Parse(splits[1]);
                        int year = int.Parse(splits[2]);
                        actualDate = new DateTime(year, month, day);
                    }
                    //просто берем 
                    if (dataRow[columnName].GetType() == typeof(DateTime))
                    {
                        actualDate = (DateTime)dataRow[columnName];
                    }
                }
                #endregion

                #region ident
                if (_mapping.FindAI(PositionAttribute.Ident))
                {
                    string columnName = _mapping.GetAI(PositionAttribute.Ident);
                    if (string.IsNullOrEmpty(dataRow[columnName].ToString()))
                        continue;
                    if (dataRow[columnName].GetType() == typeof(string))
                    {
                        ident = dataRow[columnName].ToString().Trim();
                    }
                }
                #endregion

                #region type
                if (_mapping.FindAI(PositionAttribute.Type))
                {
                    string columnName = _mapping.GetAI(PositionAttribute.Type);
                    if (string.IsNullOrEmpty(dataRow[columnName].ToString()))
                        continue;
                    if (dataRow[columnName].GetType() == typeof(string))
                    {
                        string typeIdent = dataRow[columnName].ToString().Trim();
                        finType = _mapping.Get<FinType>(typeIdent, FinType.Default);
                    }
                }
                #endregion

                foreach (DataColumn dataColumn in dataTable.Columns)
                {
                    try
                    {
                        //если у нас читается столбец, в котором есть поля идентифкатора, типа или время, то пропускаем их 
                        if (_mapping.FindAI(dataColumn.ColumnName))
                        {
                            Enum enumeration = _mapping.GetAI(dataColumn.ColumnName);
                            
                            if (enumeration.GetType() == typeof(PositionAttribute))
                            {
                                PositionAttribute pa = (PositionAttribute)enumeration;
                                if (pa == PositionAttribute.Type || pa == PositionAttribute.Ident)
                                {
                                    if(!_mapping.AI.Where(zz=>zz.Key.GetType() == typeof(ScalarAttribute)).Select(zz=>zz.Value).Contains(dataColumn.ColumnName))
                                        continue;
                                    else
                                    {
                                        enumeration = _mapping.AI.Where(zz => zz.Key.GetType() == typeof(ScalarAttribute))
                                            .First(zz => zz.Value == dataColumn.ColumnName).Key;
                                    }
                                }
                            }
                            //Данный скалярный атрибут надо прочитать 
                            if (enumeration.GetType() == typeof(ScalarAttribute)
                                && _sp.ContainsKey((ScalarAttribute)enumeration))
                            {
                                ParamType paramType = _sp[(ScalarAttribute)enumeration];

                                if (ident == string.Empty)
                                    continue;

                                #region ScalarDate
                                if (paramType == ParamType.DateTime)
                                {
                                    PortfolioPosition position = new BalancePosition(ident, finType);
                                    bool isCanToDoIt = false;
                                    DateTime needDate = DateTime.Now;
                                    #region Парсировка
                                    //Она есть в таблице, но ее нельзя распарсить
                                    if (string.IsNullOrEmpty(dataRow[dataColumn].ToString()))
                                        continue;

                                    //у нас в таблице дата есть, но она может быть в формате
                                    if (dataRow[dataColumn].GetType() == typeof(string))
                                    {
                                        string[] splits = dataRow[dataColumn].ToString().Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                                        if (splits.Count() != 3)
                                            continue;
                                        int day = int.Parse(splits[0]);
                                        int month = int.Parse(splits[1]);
                                        int year = int.Parse(splits[2]);
                                        needDate = new DateTime(year, month, day);
                                        isCanToDoIt = true;
                                    }
                                    //просто берем 
                                    if (dataRow[dataColumn].GetType() == typeof(DateTime))
                                    {
                                        needDate = (DateTime)dataRow[dataColumn];
                                        isCanToDoIt = true;
                                    }
                                    #endregion

                                    if (isCanToDoIt)
                                    {
                                        Dictionary<DateTime, DateTime> dict = new Dictionary<DateTime, DateTime>()
                                    {
                                        {actualDate, needDate}
                                    };
                                        ScalarDate str = new ScalarDate(dict);
                                        KeyValuePair<PortfolioPosition, Enum> key =
                                            new KeyValuePair<PortfolioPosition, Enum>(position, enumeration);
                                        if (_cahce.ContainsKey(key))
                                        {
                                            if (_cahce[key].GetType() == typeof(ScalarStr))
                                            {
                                                ScalarDate newStrs = (ScalarDate)_cahce[key];
                                                newStrs.Add(actualDate, dict.First().Value);
                                                _cahce.Remove(key);
                                                _cahce.Add(key, newStrs);
                                            }
                                        }
                                        else
                                        {
                                            _cahce.Add(key, str);
                                        }
                                    }
                                }
                                #endregion

                                #region ScalarDecimal
                                if (paramType == ParamType.Decimal)
                                {
                                    decimal value = decimal.Zero;
                                    bool isGood = false;

                                    try
                                    {
                                        value = Convert.ToDecimal(dataRow[dataColumn].ToString());
                                        isGood = true;
                                    }
                                    catch
                                    {

                                    }

                                    try
                                    {
                                        if (!isGood)
                                        {
                                            string otherValue = dataRow[dataColumn].ToString();
                                            if (otherValue.Contains(','))
                                                otherValue = otherValue.Replace(',', '.');
                                            else
                                                otherValue = otherValue.Replace('.', ',');
                                            value = Convert.ToDecimal(otherValue);
                                            isGood = true;
                                        }
                                    }
                                    catch
                                    {

                                    }

                                    if (!isGood)
                                        continue;

                                    Dictionary<DateTime, decimal> str = new Dictionary<DateTime, decimal>()
                                    {
                                        {actualDate, value}
                                    };
                                    ScalarNum strs = new ScalarNum(str);
                                    PortfolioPosition position = new BalancePosition(ident, finType);
                                    KeyValuePair<PortfolioPosition, Enum> key =
                                            new KeyValuePair<PortfolioPosition, Enum>(position, enumeration);
                                    if (_cahce.ContainsKey(key))
                                    {
                                        if (_cahce[key].GetType() == typeof(ScalarNum))
                                        {
                                            ScalarNum newStrs = (ScalarNum)_cahce[key];
                                            newStrs.Add(actualDate, str.First().Value);
                                            _cahce.Remove(key);
                                            _cahce.Add(key, newStrs);
                                        }
                                    }
                                    else
                                    {
                                        _cahce.Add(key, strs);
                                    }
                                }
                                #endregion

                                #region ScalarStr
                                if (paramType == ParamType.String)
                                {
                                    #region enum
                                    if (_mapping.FindET(enumeration))
                                    {
                                        PortfolioPosition position = new BalancePosition(ident, finType);
                                        Type typeEnum = _mapping.GetET(enumeration);
                                        string value = dataRow[dataColumn].ToString();
                                        KeyValuePair<Type, string> otherKey = new KeyValuePair<Type, string>(typeEnum, value);
                                        if (!_mapping.TKE.ContainsKey(otherKey))
                                            continue;
                                        var tt = _mapping.TKE[otherKey];
                                        Dictionary<DateTime, Enum> dictDE = new Dictionary<DateTime, Enum>()
                                    {
                                        {actualDate, tt}
                                    };
                                        ScalarEnum se = new ScalarEnum(dictDE);
                                        KeyValuePair<PortfolioPosition, Enum> key =
                                            new KeyValuePair<PortfolioPosition, Enum>(position, enumeration);

                                        if (_cahce.ContainsKey(key))
                                        {
                                            if (_cahce[key].GetType() == typeof(ScalarEnum))
                                            {
                                                ScalarEnum scalarEnum = (ScalarEnum)_cahce[key];
                                                scalarEnum.Add(actualDate, dictDE.First().Value);
                                                _cahce.Remove(key);
                                                _cahce.Add(key, scalarEnum);
                                            }
                                        }
                                        else
                                        {
                                            _cahce.Add(key, se);
                                        }
                                    }
                                    #endregion

                                    #region string
                                    else
                                    {
                                        Dictionary<DateTime, string> str = new Dictionary<DateTime, string>()
                                    {
                                        {actualDate, dataRow[dataColumn].ToString()}
                                    };
                                        ScalarStr strs = new ScalarStr(str);
                                        PortfolioPosition position = new BalancePosition(ident, finType);
                                        KeyValuePair<PortfolioPosition, Enum> key =
                                            new KeyValuePair<PortfolioPosition, Enum>(position, enumeration);
                                        if (_cahce.ContainsKey(key))
                                        {
                                            if (_cahce[key].GetType() == typeof(ScalarStr))
                                            {
                                                ScalarStr newStrs = (ScalarStr)_cahce[key];
                                                newStrs.Add(actualDate, str.First().Value);
                                                _cahce.Remove(key);
                                                _cahce.Add(key, newStrs);
                                            }
                                        }
                                        else
                                        {
                                            _cahce.Add(key, strs);
                                        }
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ;
                    }
                }
            }
        }

        private void ReadCurve(DataTable dataTable)
        {
            //Находим финансовый инструмент 
            //находим 
            foreach (DataRow dataRow in dataTable.Rows)
            {
                string ident = _ident;
                FinType finType = FinType.Default;
                DateTime actualDate = _reportDate;

                #region actualDate
                if (_mapping.FindAI(PositionAttribute.ActualDate))
                {
                    string columnName = _mapping.GetAI(PositionAttribute.ActualDate);
                    //Она есть в таблице, но ее нельзя распарсить
                    if (string.IsNullOrEmpty(dataRow[columnName].ToString()))
                        continue; ;

                    //у нас в таблице дата есть, но она может быть в формате
                    if (dataRow[columnName].GetType() == typeof(string))
                    {
                        string[] splits = dataRow[columnName].ToString().Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                        int day = int.Parse(splits[0]);
                        int month = int.Parse(splits[1]);
                        int year = int.Parse(splits[2]);
                        actualDate = new DateTime(year, month, day);
                    }
                    //просто берем 
                    if (dataRow[columnName].GetType() == typeof(DateTime))
                    {
                        actualDate = (DateTime)dataRow[columnName];
                    }
                }
                #endregion

                #region ident
                if (_mapping.FindAI(PositionAttribute.Ident))
                {
                    string columnName = _mapping.GetAI(PositionAttribute.Ident);
                    if (string.IsNullOrEmpty(dataRow[columnName].ToString()))
                        continue;
                    if (dataRow[columnName].GetType() == typeof(string))
                    {
                        ident = dataRow[columnName].ToString().Trim();
                    }
                }
                #endregion

                #region type
                if (_mapping.FindAI(PositionAttribute.Type))
                {
                    string columnName = _mapping.GetAI(PositionAttribute.Type);
                    if (string.IsNullOrEmpty(dataRow[columnName].ToString()))
                        continue;
                    if (dataRow[columnName].GetType() == typeof(string))
                    {
                        string typeIdent = dataRow[columnName].ToString().Trim();
                        finType = _mapping.Get<FinType>(typeIdent, FinType.Default);
                    }
                }
                #endregion

                foreach (DataColumn dataColumn in dataTable.Columns)
                {
                    try
                    {
                        if (_mapping.FindAI(dataColumn.ColumnName))
                        {
                            //field
                            Enum enumeration = _mapping.GetAI(dataColumn.ColumnName);
                            if (enumeration.GetType() == typeof(PositionAttribute))
                            {
                                PositionAttribute pa = (PositionAttribute)enumeration;
                                if (pa == PositionAttribute.Type || pa == PositionAttribute.Ident)
                                    continue;
                            }
                        }
                        else
                        {
                            //rates
                            if (string.IsNullOrEmpty(dataRow[dataColumn].ToString()))
                                continue;

                            #region rates
                            decimal rate = decimal.Zero;
                            bool isGood = false;
                            try
                            {
                                rate = Convert.ToDecimal(dataColumn.ColumnName);
                                isGood = true;
                            }
                            catch(Exception ex)
                            {

                            }
                            try
                            {
                                if (!isGood)
                                {
                                    string otherValue = dataColumn.ColumnName;
                                    if (otherValue.Contains(','))
                                        otherValue = otherValue.Replace(',', '.');
                                    else
                                        otherValue = otherValue.Replace('.', ',');
                                    rate = Convert.ToDecimal(otherValue);
                                    isGood = true;
                                }
                            }
                            catch(Exception ex)
                            {

                            }
                            #endregion

                            #region value
                            decimal value = decimal.Zero;
                            bool valueIsGood = false;
                            try
                            {
                                value = Convert.ToDecimal(dataRow[dataColumn].ToString());
                                valueIsGood = true;
                            }
                            catch (Exception ex)
                            {

                            }
                            try
                            {
                                if (!valueIsGood)
                                {
                                    string otherValue = dataRow[dataColumn].ToString();
                                    if (otherValue.Contains(','))
                                        otherValue = otherValue.Replace(',', '.');
                                    else
                                        otherValue = otherValue.Replace('.', ',');
                                    value = Convert.ToDecimal(otherValue);
                                    valueIsGood = true;
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                            #endregion

                            if(isGood && valueIsGood)
                            {
                                string rateString = rate.ToString().Replace('.','_').Replace(',','_');
                                string newIdent = string.Format("{0}_{1}", ident, rateString);
                                SortedDictionary<DateTime, decimal> values =
                                    new SortedDictionary<DateTime, decimal>()
                                    {
                                        {actualDate, value},
                                    };
                                TimeSeries timeSeries = new TimeSeries(values);

                                PortfolioPosition position = new BalancePosition(newIdent, FinType.PercentCurve);
                                KeyValuePair<PortfolioPosition, Enum> key =
                                    new KeyValuePair<PortfolioPosition, Enum>(position, TimeSeriesAttribute.Close);
                                if(_cahce.ContainsKey(key))
                                {
                                    if(_cahce[key].GetType() == typeof(TimeSeries))
                                    {
                                        TimeSeries moreTimeSeries = (TimeSeries)_cahce[key];
                                        moreTimeSeries.Add(actualDate, value);
                                        _cahce[key] = moreTimeSeries;
                                    }
                                    else
                                    {
                                        _cahce.Add(key, timeSeries);
                                    }
                                }
                                else
                                {
                                    _cahce.Add(key, timeSeries);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }

        public ScalarEnum GetScalarEnum(PortfolioPosition position, ScalarAttribute attribute)
        {
            if(_mapping.FindAI(attribute))
            {
                if (!_isCached)
                    Download();

            }
            return null;
        }

        public ScalarNum GetScalarNum(PortfolioPosition position, ScalarAttribute attribute)
        {
            if (_mapping.FindAI(attribute))
            {
                if (!_isCached)
                    Download();

            }
            return null;
        }

        public ScalarStr GetScalarStr(PortfolioPosition position, ScalarAttribute attribute)
        {
            if (_mapping.FindAI(attribute))
            {
                if (!_isCached)
                    Download();
            }
            return null;
        }

        public TimeSeries GetTimeSeries(PortfolioPosition position, TimeSeriesAttribute attribute)
        {
            if (_mapping.FindAI(attribute))
            {
                if (!_isCached)
                    Download();
            }
            return null;
        }

        public TimeSeries GetTimeSeries(PortfolioPosition position, TimeSeriesAttribute attribute, DateTime from, DateTime to)
        {
            if (_mapping.FindAI(attribute))
            {
                if (!_isCached)
                    Download();
            }
            return null;
        }

        public List<Core.Mir.ParamDescriptor> GetParams()
        {
            throw new NotImplementedException();
        }

        public void SetParams(Dictionary<string, object> objects)
        {
            throw new NotImplementedException();
        }

        public List<PortfolioPosition> GetAllPositions()
        {
            if (!_isCached)
                Download();

            var listResult =_cahce.Select(z => z.Key.Key).Distinct().ToList();
            return listResult;
        }

        public List<PortfolioPosition> GetAllPositions(FinType finType)
        {
            if (!_isCached)
                Download();

            var listResult = _cahce.Select(z => z.Key.Key).Distinct().Where(z=>z.FinType == finType).ToList();
            return listResult;
        }

        public List<PortfolioPosition> GetAllPositions(List<FinType> fintypes)
        {
            if (!_isCached)
                Download();

            var listResult = _cahce.Select(z => z.Key.Key).Distinct().Where(z => fintypes.Contains(z.FinType)).ToList();
            return listResult; 
        }

        public Dictionary<KeyValuePair<PortfolioPosition, Enum>, object> GetAllData()
        {
            if (!_isCached)
                Download();

            return _cahce;
        }


        public bool Get<T>(PortfolioPosition position, ScalarAttribute attribute, DateTime dateTime)
        {
            return false;
        }

        public T Get<T>(PortfolioPosition position, ScalarAttribute attribute, DateTime dateTime, T defaultValue) where T : struct, IConvertible
        {
            return defaultValue;
        }


        public TimeSeries GetTimeSeries(Enum enumeration, TimeSeriesAttribute attribute)
        {
            return null;
        }

        public TimeSeries GetTimeSeries(Enum enumeration, TimeSeriesAttribute attribute, DateTime from, DateTime to)
        {
            return null;
        }


        public void ClearCache()
        {
            
        }

        public Dictionary<KeyValuePair<PortfolioPosition, Enum>, object> Cache
        {
            get
            {
                return _cahce;
            }
        }
    }
}
