using Core.Mir.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProvider.Input.MirReader
{
    public class Mapping: IMapping
    {
        public Mapping()
        {
            AI = new Dictionary<Enum, string>();
            ET = new Dictionary<Enum, Type>();
            TKE = new Dictionary<KeyValuePair<Type, string>, Enum>();
        }

        #region AI
        public Dictionary<Enum, string> AI
        {
            get;
            set;
        }

        public bool AddAI(Enum attribute, string ident)
        {
            if (AI.ContainsKey(attribute))
            {
                AI[attribute] = ident;
                return false;
            }
            else
            {
                AI.Add(attribute, ident);
                return true;
            }
        }

        public bool RemoveAI(Enum attribute)
        {
            if (AI.ContainsKey(attribute))
            {
                AI.Remove(attribute);
                return true;
            }
            else
            {
                return true;
            }
        }

        public bool FindAI(Enum attribute)
        {
            if (AI.ContainsKey(attribute))
                return true;
            return false;
        }

        public string GetAI(Enum attribute)
        {
            if (FindAI(attribute))
            {
                string value = AI[attribute];
                return value;
            }
            string message = string.Format("Атрибут:{0} не был найден в маппинге", attribute.ToString());
            throw new Exception(message);
        }

        public bool FindAI(string ident)
        {
            if (AI.Values.Contains(ident))
                return true;
            return false;
        }

        public Enum GetAI(string ident)
        {
            if (AI.ContainsValue(ident))
                return AI.First(z => z.Value == ident).Key;
            throw new Exception();
        }
        #endregion

        #region TKE
        public Dictionary<KeyValuePair<Type, string>, Enum> TKE
        {
            get;
            set;
        }

        public bool AddTKE<T>(string key, Enum value)
        {
            Type t = typeof(T);
            KeyValuePair<Type, string> tk = new KeyValuePair<Type, string>(t, key);
            if (TKE.ContainsKey(tk))
            {
                TKE[tk] = value;
                return true;
            }
            else
            {
                TKE.Add(tk, value);
                return false;
            }
        }

        public bool RemoveTKE<T>(string key)
        {
            if (FindTKE<T>(key))
            {
                KeyValuePair<Type, string> tk = new KeyValuePair<Type, string>(typeof(T), key);
                TKE.Remove(tk);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool FindTKE<T>(string key)
        {
            KeyValuePair<Type, string> tk = new KeyValuePair<Type, string>(typeof(T), key);
            if (TKE.ContainsKey(tk))
            {
                return true;
            }
            return false;
        }

        public Enum GetTKE<T>(string key)
        {
            if (FindTKE<T>(key))
            {
                KeyValuePair<Type, string> tk = new KeyValuePair<Type, string>(typeof(T), key);
                return TKE[tk];
            }
            string message = string.Format("Not found");
            throw new Exception(message);
        }

        public T Get<T>(string ident) where T : struct, IConvertible
        {
            Enum enumeration = GetTKE<T>(ident);
            return (T)Enum.Parse(typeof(T), enumeration.ToString(), true);
        }

        public T Get<T>(string ident, T defaultValue) where T : struct, IConvertible
        {
            if (FindTKE<T>(ident))
            {
                Enum enumeration = GetTKE<T>(ident);
                return (T)Enum.Parse(typeof(T), enumeration.ToString(), true);
            }
            else
            {
                return defaultValue;
            }
        }

        public bool FindTKE<T>(Enum enumeration)
        {
            if (TKE.ContainsValue(enumeration))
                return true;
            return false;
        }

        public bool FindTKE(Enum enumeration)
        {
            if (TKE.ContainsValue(enumeration))
                return true;
            return false;
        }

        public string GetTKE(Enum enumeration)
        {
            if (FindTKE(enumeration))
            {
                var result = TKE.FirstOrDefault(z => z.Value.Equals(enumeration)).Key.Value;
                return result;
            }
            string message = "not found";
            throw new Exception(message);

        }

        public string GetTKE<T>(Enum enumeration)
        {
            if (FindTKE<T>(enumeration))
            {
                return TKE.FirstOrDefault(z => z.Key.Key == typeof(T) && z.Value.Equals(enumeration)).Key.Value;
            }
            else
            {
                string message = string.Format("Not found");
                throw new Exception(message);
            }
        }

        #endregion

        #region ET
        public Dictionary<Enum, Type> ET
        {
            get;
            set;
        }

        public bool AddET(Enum attribute, Type type)
        {
            if (ET.ContainsKey(attribute))
            {
                ET[attribute] = type;
                return false;
            }
            else
            {
                ET.Add(attribute, type);
                return true;
            }
        }

        public bool RemoveET(Enum attribute)
        {
            if (FindET(attribute))
            {
                ET.Remove(attribute);
                return true;
            }
            return false;
        }

        public bool FindET(Enum attribute)
        {
            if (ET.ContainsKey(attribute))
                return true;
            return false;
        }

        public Type GetET(Enum attribute)
        {
            if (FindET(attribute))
            {
                return ET[attribute];
            }
            string message = string.Format("NOT FOUND");
            throw new Exception(message);
        }
        #endregion


        public T GetAI_2<T>(string ident) where T : struct, IConvertible
        {
            if (AI.ContainsValue(ident))
            {
                Enum enumeration = AI.First(z => z.Value == ident).Key;
                return (T)Enum.Parse(typeof(T), enumeration.ToString(), true);
            }
            throw new Exception();
        }
    }
}
