using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Mir.Enumerations;

namespace Core.Mir
{
    public class ParamDescriptor
    {
        public ParamDescriptor()
        {

        }

        private string _ident;
        private string _description;
        private ParamType _paramType;
        private object _value;

        public string Ident { get { return _ident; } set { _ident = value; } }
        public string Description { get { return _description; } set { _description = value; } }
        public ParamType ParamType { get { return _paramType; } set { _paramType = value; } }
        public object Value { get { return _value; } set { _value = value; } }
    }
}
