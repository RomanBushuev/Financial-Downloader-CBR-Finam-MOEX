using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Mir.BaseTypes
{
    public abstract class Scalar<T>
    {
        private SortedDictionary<DateTime, T> _store = new SortedDictionary<DateTime, T>();

        public Scalar()
        {

        }

        public Scalar(IDictionary<DateTime, T> store)
        {
            _store = new SortedDictionary<DateTime, T>(store);
        }
        public virtual void Add(DateTime dateTime, T value)
        {
            if(_store.ContainsKey(dateTime))
                _store[dateTime] = value;
            else
                _store.Add(dateTime, value);
        }

        public virtual T Get(DateTime dateTime)
        {
            if (_store == null || _store.Count == 0)
                throw new Exception("Последовательность равна 0");

            //проверяем первый элемент
            if (_store.First().Key > dateTime)
            {
                string message = string.Format("Первое валидное значение датириуется {0}", _store.First().Key.ToShortDateString());
                throw new Exception(message);
            }

            //проверяем последующие элементы
            for (int i = 0; i < _store.Count - 1; ++i)
            {
                DateTime previousDate = _store.ElementAt(i).Key;
                DateTime currentDate = _store.ElementAt(i + 1).Key;

                if (previousDate <= dateTime && currentDate > dateTime)
                {
                    T value = _store[previousDate];
                    return value;
                }
            }
            //првоеряем последний элемент

            if (_store.Last().Key <= dateTime)
            {
                T value = _store[_store.Last().Key];
                return value;
            }

            throw new Exception("Как ты сюда дошел ?");
        }

        public virtual bool HasValue(DateTime dateTime)
        {
            if (_store == null || _store.Count == 0)
                return false;

            //проверяем первый элемент
            if (_store.First().Key > dateTime)
                return false;
            
            //проверяем последующие элементы
            for(int i = 0; i < _store.Count - 1; ++i)
            {
                DateTime previousDate = _store.ElementAt(i).Key;
                DateTime currentDate = _store.ElementAt(i + 1).Key;

                if(previousDate <= dateTime && currentDate > dateTime)
                {
                    return true;
                }
            }
            //првоеряем последний элемент

            if(_store.Last().Key <= dateTime)
            {
                return true;
            }

            throw new Exception("Как ты сюда дошел ?");
        }

        public virtual IDictionary<DateTime, T> Dictionary { get { return _store; } }
    }

    public class ScalarStr:Scalar<string>
    {
        public ScalarStr(IDictionary<DateTime, string> store)
            :base(store)
        {

        }
    }

    public class ScalarNum:Scalar<decimal>
    {
        public ScalarNum(IDictionary<DateTime, decimal> store)
            :base(store)
        {

        }
    }

    public class ScalarDate:Scalar<DateTime>
    {
        public ScalarDate(IDictionary<DateTime, DateTime> store)
            :base(store)
        {

        }
    }

    public class ScalarEnum:Scalar<Enum>
    {
        public ScalarEnum(IDictionary<DateTime, Enum> store)
            : base(store)
        {

        }
    }
}
