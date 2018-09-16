using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core.Mir;
using System.Collections.Generic;
using System.Linq;
using Core.Mir.BaseTypes;

namespace TestCore
{
    [TestClass]
    public class TestScalar
    {
        [TestMethod]
        [TestCategory("Core")]
        [TestCategory("Scalar")]
        public void TestScalarGet()
        {
            Dictionary<DateTime, string> dateTimes = new Dictionary<DateTime, string>()
            {
                {DateTime.Now, "bushuev"},
                {DateTime.Now.AddDays(-1), "roman"},
                {DateTime.Now.AddDays(1), "nik"}
            };

            ScalarStr date = new ScalarStr(dateTimes);

            Assert.IsFalse(date.HasValue(DateTime.Now.AddDays(-2)));
            Assert.AreEqual(date.Get(DateTime.Now), "bushuev");
            Assert.AreEqual(date.Get(DateTime.Now.AddDays(-1)), "roman");
            Assert.AreEqual(date.Get(DateTime.Now.AddDays(100)), "nik");
        }

        [TestMethod]
        [TestCategory("Core")]
        [TestCategory("Scalar")]
        public void TestScalarHas()
        {
            Dictionary<DateTime, DateTime> dateTimes = new Dictionary<DateTime, DateTime>()
            {
                {DateTime.Now, DateTime.Now},
                {DateTime.Now.AddDays(-1), DateTime.Now.AddDays(-1)},
                {DateTime.Now.AddDays(1), DateTime.Now.AddDays(1)},
                {DateTime.Now.AddDays(2), DateTime.Now.AddDays(2)},
            };

            ScalarDate date = new ScalarDate(dateTimes);

            Assert.IsFalse(date.HasValue(DateTime.Now.AddDays(-2)));
            Assert.IsTrue(date.HasValue(DateTime.Now));
            Assert.IsTrue(date.HasValue(DateTime.Now.AddDays(3)));
        }

        [TestMethod]
        public void TestDictionary()
        {
            Roman roman = (Roman)Get<Roman>("roman");
            Bushuev one = Get<Bushuev>("one");
        }

        [TestMethod]
        public void R()
        {
            Dictionary<Enum, int> id = new Dictionary<Enum, int>();
            Roman r = Roman.Roman;
            Bushuev b = Bushuev.One;
            id.Add(r, 1);
            id.Add(b, 2);
            if(id.ContainsKey(Roman.Roman))
            {
                Console.WriteLine("true");
            }
            else
            {
                Console.WriteLine("false");
            }
        }

        public T Get<T>(string ident) where T : struct, IConvertible
        {
             //_dict[ident];
             Enum enumeration = _dict[ident];
             return (T)Enum.Parse(typeof(T), enumeration.ToString(), true);
        }        

        Dictionary<string, Enum> _dict = new Dictionary<string, Enum>()
        {
            {"roman", Roman.Roman},
            {"bushuev", Bushuev.Bushuev},
            {"one", Bushuev.One},
        };

        [TestMethod]
        public void TestBushuevMinValueEnum()
        {
            List<Bushuev> bushuevs = 
                Enum.GetValues(typeof(Bushuev)).OfType<Bushuev>().ToList();


        }

    }

    public enum Roman
    {
        Default = 1 << 0,
        Roman = 1 << 1,
    }

    public enum Bushuev
    {
        Default = 1 << 0,
        Bushuev = 1 << 1,
        One = 1<< 2,
    }


}
