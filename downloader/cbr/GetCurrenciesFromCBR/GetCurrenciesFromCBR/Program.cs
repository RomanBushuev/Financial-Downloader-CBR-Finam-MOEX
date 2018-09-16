using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetCurrenciesFromCBR.CbrServices;
using System.Data;
using System.IO;
using System.Threading;

namespace GetCurrenciesFromCBR
{
    class Program
    {
        static void Main(string[] args)
        {
            bool hasException = false;
            CbrServices.DailyInfoSoapClient client = new DailyInfoSoapClient();
            try
            {
                string generalPath = @"C:\Users\bushuevroman\YandexDisk\MarketData\CBR\raw\currencies\";
                DateTime from = DateTime.Now;
                StringBuilder text = new StringBuilder();

                var result = client.GetCursOnDate(from);
                foreach (DataTable dataTable in result.Tables)
                {
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        text.Append(column.ColumnName + "\t");
                    }
                    text.Append("\n");
                    foreach (DataRow row in dataTable.Rows)
                    {
                        foreach (DataColumn column in dataTable.Columns)
                        {
                            text.Append(row[column].ToString() + "\t");
                        }
                        text.Append("\n");
                    }
                }
                string dateTimeText = from.Year.ToString() + "." + from.Month.ToString("00") + "." + from.Day.ToString("00");
                System.IO.File.WriteAllText(generalPath + dateTimeText + ".csv", text.ToString());
                from = from.AddDays(1);
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
                        string pathError = @"C:\Users\bushuevroman\YandexDisk\MarketData\CurrencyExceptionLogs.txt";
                        using (FileStream fileStream = new FileStream(path,
                            FileMode.Append,
                            FileAccess.Write,
                            FileShare.Read))
                        {
                            string dataasstring = "Currencies exception" + "\n" + DateTime.Now.ToShortDateString();
                            byte[] info = new UTF8Encoding(true).GetBytes(dataasstring);
                            fileStream.Write(info, 0, info.Length);
                            isWriteException = true;
                            string input = string.Format("{2}->{0}->{1}", DateTime.Today.ToShortDateString(), ex.Message, "Currencies exception");
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

            if(hasException)
            {
                return;
            }
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
                        DateTime date = DateTime.Today;
                        string shortFormat = date.ToString("yyyy.MM.dd");
                        string dataasstring = "Currencies" + " " + shortFormat + "\n";
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
