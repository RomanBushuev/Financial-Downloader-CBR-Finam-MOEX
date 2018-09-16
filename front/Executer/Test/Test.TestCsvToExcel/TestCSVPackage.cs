using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeOpenXml;
using System.IO;
using System.Linq;
using System.Text;
using ExcelDataReader.Core;
using ExcelDataReader;
using System.Data;

namespace Test.TestCsvToExcel
{
    [TestClass]
    public class TestCSVPackage
    {
        [TestMethod]
        public void UseEEPlus()
        {
            string csvFileName = @"C:\Users\RomanBushuev\YandexDisk\MarketData\MOEX\raw\2018.03.12\rates.csv";
            string str = Path.GetFileNameWithoutExtension(csvFileName);
            string outputName = @"C:\Users\RomanBushuev\YandexDisk\MarketData\MOEX\raw\2018.03.12\rates.csv";

            var tempText = File.ReadAllText(csvFileName, Encoding.GetEncoding("windows-1251"));
            if (File.Exists(outputName))
                File.Delete(outputName);
            string worksheetsName = "rates";
            bool firstRowIsHeader = true;
            var format = new ExcelTextFormat();
            format.Encoding = Encoding.GetEncoding("Windows-1251");
            format.SkipLinesBeginning = 2;
            format.Delimiter = ';';
            format.EOL = "\r";
            var result = File.ReadAllLines(csvFileName);
            var t = new FileInfo(csvFileName);
            int amount = result.Skip(2).First().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Count();
            string text = File.ReadAllText(csvFileName, Encoding.Default);

            format.DataTypes = new eDataTypes[]
            {
            };
            using (ExcelPackage package = new ExcelPackage(new FileInfo(outputName)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(worksheetsName);
                worksheet.Cells["A1"].LoadFromText(text, format, OfficeOpenXml.Table.TableStyles.Medium27, firstRowIsHeader);
                package.Save();
            }

            Console.WriteLine("Finished!");
        }

        [TestMethod]
        public void UseExcelDataReader()
        {
            string outputName = @"C:\Users\RomanBushuev\YandexDisk\MarketData\MOEX\raw\2018.03.12\rates.xlsx";
            FileStream stream = File.Open(outputName, FileMode.Open, FileAccess.Read );

            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            ExcelDataSetConfiguration configureation = new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = true,
                }
            };
            DataSet dataSet = excelReader.AsDataSet(configureation);
            foreach(DataTable datatable in dataSet.Tables)
            {
                foreach(DataRow dataRow in datatable.Rows)
                {
                    object var = dataRow["MATDATE"];
                    Console.WriteLine(dataRow["MATDATE"].GetType());
                }
            }
        }    

        [TestMethod]
        public void UseExcelDataReader_1()
        {
            string outputName = @"C:\Users\RomanBushuev\YandexDisk\MarketData\MOEX\raw\2018.03.12\rates.csv";
            string outputName_2 = @"C:\Users\RomanBushuev\YandexDisk\MarketData\MOEX\raw\2018.03.12\rates2.csv";
            var intermediateResult = File.ReadAllLines(outputName, Encoding.GetEncoding("windows-1251")).Skip(2);
            File.WriteAllLines(outputName_2, intermediateResult);

            FileStream stream = File.Open(outputName_2, FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateCsvReader(stream);
            ExcelDataSetConfiguration configureation = new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = true                    
                }                
            };
            DataSet dataSet = excelReader.AsDataSet(configureation);
            foreach (DataTable datatable in dataSet.Tables)
            {
                foreach (DataRow dataRow in datatable.Rows)
                {
                    object var = dataRow["MATDATE"];
                    Console.WriteLine(dataRow["MATDATE"].GetType());
                    if(var != null)
                    {
                        Console.WriteLine(var.ToString());
                    }
                }
            }
        }


    }
}
