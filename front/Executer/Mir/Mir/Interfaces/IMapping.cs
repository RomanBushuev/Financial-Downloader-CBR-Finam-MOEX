using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Mir.BaseTypes;
using Core.Mir.Enumerations;

namespace Core.Mir.Interfaces
{
    public interface IMapping
    {
        #region Attribute -> ident
        Dictionary<Enum, string> AI { get; set; }
        bool AddAI(Enum attribute, string ident);
        bool RemoveAI(Enum attribute);
        bool FindAI(Enum attribute);
        string GetAI(Enum attribute);
        bool FindAI(string ident);
        Enum GetAI(string ident);

        T GetAI_2<T>(string ident) where T : struct, IConvertible;
        #endregion

        #region type + key -> enum 
        Dictionary<KeyValuePair<Type, string>, Enum> TKE { get; set; }
        bool AddTKE<T>(string key, Enum value);
        bool RemoveTKE<T>(string key);
        bool FindTKE<T>(string key);
        bool FindTKE(Enum enumeration);
        string GetTKE(Enum enumeration);
        Enum GetTKE<T>(string key);
        T Get<T>(string ident) where T : struct, IConvertible;

        T Get<T>(string ident, T defaultValue) where T : struct, IConvertible;
        #endregion

        #region scalarType -> Type
        Dictionary<Enum, Type> ET { get; set; }
        bool AddET(Enum attribute, Type type);
        bool RemoveET(Enum attribute);
        bool FindET(Enum attribute);
        Type GetET(Enum attribute);
        #endregion
    }
}
