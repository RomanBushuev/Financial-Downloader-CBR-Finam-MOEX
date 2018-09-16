using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Linq;
using DataProvider.Output.Excel;
using Core.Mir.BaseTypes;
using System.Xml;
namespace Test.DataProvider.Excel
{
    [TestClass]
    public class TestSaveDataSet
    {
        [TestMethod]
        public void TestSaveDataSetExcel()
        {
            //первая таблица 
            DataTable dataTable = new DataTable("roman");
            DataColumn firstfirst = new DataColumn("time", typeof(DateTime));
            DataColumn firstsecond = new DataColumn("ident", typeof(string));
            dataTable.Columns.AddRange(new DataColumn[] { firstfirst, firstsecond});
            DataRow dataRow = dataTable.NewRow();
            dataRow["time"] = DateTime.Now;
            dataRow["ident"] = "rucpi";
            dataTable.Rows.Add(dataRow);

            //вторая таблица 
            DataTable dts = new DataTable("bushuev");
            DataColumn secondfirst = new DataColumn("onemore", typeof(decimal));
            DataColumn secondsecond = new DataColumn("onemore2", typeof(string));
            dts.Columns.AddRange(new DataColumn[] { secondfirst, secondsecond });
            DataRow dr = dts.NewRow();
            dr["onemore"] = 10.32m;
            dr["onemore2"] = "roman";
            dts.Rows.Add(dr);

            Core.Mir.ResultSet resultSet = new Core.Mir.ResultSet();
            resultSet.DataTable =new System.Collections.Generic.List<DataTable>()
            {
                dataTable,
                dts
            };

            string connection = @"C:\Users\RomanBushuev\Desktop\roman.xlsx";
            Provider provider = new Provider(connection);
            provider.Save(resultSet);

        }
    }
}
