using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetRuonia.CbrServices;
using System.Data;
using System.Threading;
using System.IO;

namespace GetRuonia
{
    class Program
    {
        static void Main(string[] args)
        {
            bool hasException = false;
            CbrServices.DailyInfoSoapClient client =
                new DailyInfoSoapClient();
            string generalPath = @"C:\Users\bushuevroman\YandexDisk\MarketData\CBR\raw\ruonia\ruonia.csv";
            DateTime from = new DateTime(2010, 01, 10).Date;
            DateTime to = DateTime.Today.Date;

            try
            {
                var result = client.Ruonia(from, to);
                List<string> strings = new List<string>()
                {
                    "Date\tvalue\tnominal\t"
                };
                foreach (DataTable dataTable in result.Tables)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        StringBuilder text = new StringBuilder();
                        foreach (DataColumn column in dataTable.Columns)
                        {
                            if (row[column].GetType() == typeof(DateTime))
                            {
                                //Добавление одной даты, т.к. у нас результат отличается благодаря 
                                DateTime date = ((DateTime)row[column]).AddDays(1);
                                string tempResult = string.Format("{0}.{1}.{2}",date.Day,date.Month,date.Year);
                                text.Append(tempResult + "\t");
                            }
                            else
                            {
                                text.Append(row[column].ToString() + "\t");
                            }
                        }
                        strings.Add(text.ToString());
                    }
                }
                System.IO.File.WriteAllLines(generalPath, strings);
            }
            catch (Exception ex)
            {
                hasException = true;
                Console.WriteLine(ex.Message);
                bool isWriteException = false;
                while (!isWriteException)
                {
                    try
                    {
                        string path = @"C:\Users\bushuevroman\YandexDisk\MarketData\logs.txt";
                        string pathError = @"C:\Users\bushuevroman\YandexDisk\MarketData\RuoniaExceptionLogs.txt";
                        using (FileStream fileStream = new FileStream(path,
                            FileMode.Append,
                            FileAccess.Write,
                            FileShare.Read))
                        {
                            string dataasstring = "Ruonia exception" + "\n";
                            byte[] info = new UTF8Encoding(true).GetBytes(dataasstring);
                            fileStream.Write(info, 0, info.Length);
                            isWriteException = true;
                            string input = string.Format("{0}->{1}",DateTime.Today.ToShortDateString(), ex.Message);
                            File.AppendAllLines(pathError, new List<string>() { input });
                        }
                    }
                    catch
                    {

                    }
                    finally
                    {
                        Thread.Sleep(1000);
                    }
                }

            }
            finally
            {
                client.Close();
            }

            if (hasException)
                return;

            bool isWrite = false;
            while (!isWrite)
            {
                try
                {
                    string path = @"C:\Users\bushuevroman\YandexDisk\MarketData\queue.txt";
                    using (FileStream fileStream = new FileStream(path,
                        FileMode.Append,
                        FileAccess.Write,
                        FileShare.Read))
                    {
                        string dataasstring = "Ruonia" + "\n";
                        byte[] info = new UTF8Encoding(true).GetBytes(dataasstring);
                        fileStream.Write(info, 0, info.Length);
                        isWrite = true;
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
