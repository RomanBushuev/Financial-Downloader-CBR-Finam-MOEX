using Core.Mir.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProvider.Input.ExcelReader
{
    public class Mapping : IMapping
    {
        public Dictionary<Enum, string> AI
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool AddAI(Enum attribute, string ident)
        {
            throw new NotImplementedException();
        }

        public bool RemoveAI(Enum attribute)
        {
            throw new NotImplementedException();
        }

        public bool FindAI(Enum attribute)
        {
            throw new NotImplementedException();
        }

        public string GetAI(Enum attribute)
        {
            throw new NotImplementedException();
        }

        public bool FindAI(string ident)
        {
            throw new NotImplementedException();
        }

        public Enum GetAI(string ident)
        {
            throw new NotImplementedException();
        }

        public T GetAI_2<T>(string ident) where T : struct, IConvertible
        {
            throw new NotImplementedException();
        }

        public Dictionary<KeyValuePair<Type, string>, Enum> TKE
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool AddTKE<T>(string key, Enum value)
        {
            throw new NotImplementedException();
        }

        public bool RemoveTKE<T>(string key)
        {
            throw new NotImplementedException();
        }

        public bool FindTKE<T>(string key)
        {
            throw new NotImplementedException();
        }

        public bool FindTKE(Enum enumeration)
        {
            throw new NotImplementedException();
        }

        public string GetTKE(Enum enumeration)
        {
            throw new NotImplementedException();
        }

        public Enum GetTKE<T>(string key)
        {
            throw new NotImplementedException();
        }

        public T Get<T>(string ident) where T : struct, IConvertible
        {
            throw new NotImplementedException();
        }

        public T Get<T>(string ident, T defaultValue) where T : struct, IConvertible
        {
            throw new NotImplementedException();
        }

        public Dictionary<Enum, Type> ET
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool AddET(Enum attribute, Type type)
        {
            throw new NotImplementedException();
        }

        public bool RemoveET(Enum attribute)
        {
            throw new NotImplementedException();
        }

        public bool FindET(Enum attribute)
        {
            throw new NotImplementedException();
        }

        public Type GetET(Enum attribute)
        {
            throw new NotImplementedException();
        }
    }
}
