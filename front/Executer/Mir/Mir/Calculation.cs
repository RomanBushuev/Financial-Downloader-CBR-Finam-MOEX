using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Mir.Interfaces;
using Core.Mir.BaseTypes;
using Core.Mir.Enumerations;
using System.Data;

namespace Core.Mir
{
    public abstract class Calculation : IDataTable, IGetParams, ISetParams
    {
        protected Dictionary<string, object> _params = new Dictionary<string, object>();
        protected List<ParamDescriptor> _paramDescriptors = new List<ParamDescriptor>();
        protected ResultSet _resultSet = new ResultSet();

        public DateTime ReportDate;

        public virtual bool Run()
        {
            throw new NotImplementedException();
        }

        public T Param<T>(string ident)
        {
            object value = _params[ident];
            return (T)value;
        }

        public virtual List<ParamDescriptor> GetParams()
        {
            return new List<ParamDescriptor>();
        }

        public virtual void SetParams(Dictionary<string, object> objects)
        {
            foreach(var x in objects)
            {
                var ident = _paramDescriptors.First(z => z.Ident == x.Key);
                if(_params.ContainsKey(ident.Ident))
                {
                    _params[ident.Ident] = x.Value;
                }
                else
                {
                    _params.Add(ident.Ident, x.Value);
                }
            }
        }

        public ResultSet returnResultSet()
        {
            return _resultSet;
        }

        public void AddDataTable(string tableName, List<ParamDescriptor> columnTitles)
        {
            _resultSet.AddDataTable(tableName, columnTitles);
        }

        public Environment Environment
        {
            get;
            set;
        }
    }
}
