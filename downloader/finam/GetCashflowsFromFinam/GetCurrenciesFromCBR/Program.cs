using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetCurrenciesFromCBR.CbrServices;
using System.Data;
using System.IO;
using System.Threading;
using System.Net;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Globalization;
using Npgsql;

namespace GetCurrenciesFromCBR
{
    class Program
    {
        static void Main(string[] args)
        {
            var t = DownloadIsins();
            DownloadData(t);

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
                        string dataasstring = "Cashflows" + " " + shortFormat + "\n";
                        byte[] info = new UTF8Encoding(true).GetBytes(dataasstring);
                        fileStream.Write(info, 0, info.Length);
                        dataasstring = "ScalarFromFinam" + " " + shortFormat + "\n";
                        info = new UTF8Encoding(true).GetBytes(dataasstring);
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

        public static Dictionary<string, string> DownloadIsins()
        {
            Stopwatch st = new Stopwatch();
            st.Start();
            Dictionary<string, string> dict = new Dictionary<string, string>();
            List<string> isins = new List<string>();

            #region 
            string connection = "Host = localhost; Username =postgres; Password =roman; Database =MIR";
            string nquery = @"select t.ident as ident from fin_instrument t where t.ft_id = 13";
            using(NpgsqlConnection npgConnection = new NpgsqlConnection(connection))
            {
                npgConnection.Open();
                using(NpgsqlCommand command = new NpgsqlCommand())
                {
                    command.Connection = npgConnection;
                    command.CommandText = nquery;
                    using(NpgsqlDataReader dataReader = command.ExecuteReader())
                    {
                        while(dataReader.Read())
                        {
                            string ident = dataReader["ident"].ToString();
                            isins.Add(ident);
                        }
                    }
                }
            }
            #endregion

            var doc = new HtmlAgilityPack.HtmlDocument();
            string query = @"http://bonds.finam.ru/issue/search/default.asp?emitterCustomName=";
            string pattern = @"<a href='/issue/[a-zA-Z0-9]+/default\.asp'";
            string newEnding = @"00002/default.asp";
            string oldEnding = @"/default.asp";
            string newDomain = @"http://bonds.finam.ru";
            string delete = @"<a href='";
            Regex regex = new Regex(pattern);
            try
            {
                using (var client = new WebClient())
                {
                    int index = 1;
                    foreach (var x in isins)
                    {
                        using (var stream = client.OpenRead(query + x))
                        {
                            doc.Load(stream);
   
                            if (regex.IsMatch(doc.Text))
                            {
                                Console.WriteLine(string.Format("{0} {1}", index, isins.Count()));
                                index++;
                                string given = (regex.Match(doc.Text)).ToString();
                                given = given.Replace(delete, string.Empty).Trim('\'');
                                string result = newDomain + given.Replace(oldEnding, newEnding);
                                dict.Add(x, result);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return dict;
        }

        public static void DownloadData(Dictionary<string, string> dict)
        {
            List<string> outputs = new List<string>();
            List<string> newoutputs = new List<string>();
            Dictionary<string, DateTime> startDate = new Dictionary<string, DateTime>();
            string column = "ISIN;VALID_DATE;TYPE;DATETIME;CASHFLOW;VALUE";
            string newColumn = "ISIN;VALID_DATE;TYPE;DATETIME";
            outputs.Add(column);
            newoutputs.Add(newColumn);
            var doc = new HtmlAgilityPack.HtmlDocument();
            int gIndex = 0;

            foreach (var x in dict)
            {
                gIndex++;
                Console.WriteLine(string.Format("{0} {1}",gIndex, dict.Count));
                SortedDictionary<DateTime, decimal?> couponRates = new SortedDictionary<DateTime, decimal?>();
                SortedDictionary<DateTime, decimal?> redemptions = new SortedDictionary<DateTime, decimal?>();
                bool isDownload = false;
                try
                {
                    using (var client = new WebClient())
                    {
                        client.Encoding = Encoding.GetEncoding("windows-1251");
                        using (var stream = client.OpenRead(x.Value))
                        {
                            doc.Load(stream);

                            try
                            {
                                var dateOfStartPlace = doc.DocumentNode
                                    .Descendants("td")
                                    .Where(z => (z.InnerText.Contains("Дата начала размещения") || z.InnerText.Contains("Äàòà íà÷àëà ðàçìåùåíèÿ:"))
                                && z.Attributes.Count == 0);

                                if (dateOfStartPlace != null && dateOfStartPlace.FirstOrDefault() != null)
                                {
                                    var span = dateOfStartPlace.First().Descendants("span");
                                    if (span != null)
                                    {
                                        DateTime dateTime = DateTime.ParseExact(span.First().InnerText,
                                            "dd.MM.yyyy",
                                            CultureInfo.InvariantCulture);
                                        startDate.Add(x.Key, dateTime);
                                    }
                                }
                            }
                            catch(Exception ex)
                            {

                            }

                            var cashflows = doc.DocumentNode
                                .Descendants("tr")
                                .Where(z => z.Attributes.Contains("class") && z.Attributes["class"].Value.Contains("bline"));
                            int index = 1;
                            foreach(var z in cashflows)
                            {
                                string nbsp = "&nbsp";
                                string[] strings = z.InnerText.Split(';');

                                if(strings.Length == 8)
                                {
                                    DateTime dateTime = DateTime.ParseExact(strings[0].Replace(nbsp, string.Empty).Remove(0, index.ToString().Count()),
                                          "dd.MM.yyyy",
                                          CultureInfo.InvariantCulture);
                                    decimal? percent = null;
                                    couponRates.Add(dateTime, percent);
                                }

                                if(strings.Length == 9)
                                {
                                    DateTime dateTime = DateTime.ParseExact(strings[0].Replace(nbsp, string.Empty).Remove(0, index.ToString().Count()),
                                        "dd.MM.yyyy",
                                        CultureInfo.InvariantCulture);
                                    decimal? percent = null;

                                    if(!string.IsNullOrEmpty(strings[1].Replace(nbsp, string.Empty)))
                                    {
                                        if (strings[1].Any(zz => char.IsDigit(zz)))
                                            percent = Convert.ToDecimal(strings[1].Replace(nbsp, string.Empty).Replace("%", string.Empty));
                                    }
                                    else
                                    {
                                        if (strings[7].Any(zz => char.IsDigit(zz)))
                                        {
                                            decimal red = Convert.ToDecimal(strings[7].Replace(nbsp, string.Empty).Replace("%", string.Empty));
                                            redemptions.Add(dateTime, red);
                                        }
                                    }

                                    couponRates.Add(dateTime, percent);
                                }

                                if(strings.Length == 10)
                                {
                                    DateTime dateTime = DateTime.ParseExact(strings[0].Replace(nbsp, string.Empty).Remove(0, index.ToString().Count()),
                                       "dd.MM.yyyy",
                                       CultureInfo.InvariantCulture);

                                    decimal? percent = null;
                                    if(strings[1].Any(zz=>char.IsDigit(zz)))
                                    {
                                        percent = Convert.ToDecimal(strings[1].Replace(nbsp, string.Empty).Replace("%", string.Empty));
                                    }

                                    decimal? redemption = null;
                                    if(strings[8].Any(zz=>char.IsDigit(zz)))
                                    {
                                        redemption = Convert.ToDecimal(strings[8].Replace(nbsp, string.Empty));
                                    }

                                    couponRates.Add(dateTime, percent);
                                    redemptions.Add(dateTime, redemption);
                                }
                                index++;
                            }                       
                        }
                    }

                    foreach(var coupon in couponRates)
                    {
                        string outter = string.Format("{0};{1};{2};{3};{4};{5}", x.Key, 
                            DateTime.Today.ToString("dd.MM.yyyy"),
                            "BOND",
                            coupon.Key.ToString("dd.MM.yyyy"), 
                            "COUPON",
                            coupon.Value.HasValue?coupon.Value.ToString():string.Empty);
                        outputs.Add(outter);
                    }
                    foreach(var redemption in redemptions)
                    {
                        string outter = string.Format("{0};{1};{2};{3};{4};{5}", x.Key,
                            DateTime.Today.ToString("dd.MM.yyyy"),
                            "BOND",
                            redemption.Key.ToString("dd.MM.yyyy"),
                            "REDEMPTION",
                            redemption.Value.HasValue ? redemption.Value.ToString() : string.Empty);
                        outputs.Add(outter);
                    }



                }
                catch(Exception ex)
                {

                }
            }
            string generalPath = @"C:\Users\bushuevroman\YandexDisk\MarketData\finam\raw\cashflows\";
            string secondGeneralPath = @"C:\Users\bushuevroman\YandexDisk\MarketData\finam\raw\scalars\";

            File.WriteAllLines(generalPath + DateTime.Today.ToString("yyyy.MM.dd") + ".csv", outputs);
            foreach(var x in startDate)
            {
                string outter = string.Format("{0};{1};{2};{3}",
                    x.Key,
                    DateTime.Today.ToString("dd.MM.yyyy"),
                    "BOND",
                    x.Value.ToString("dd.MM.yyyy"));
                newoutputs.Add(outter);
            }
            File.WriteAllLines(secondGeneralPath + DateTime.Today.ToString("yyyy.MM.dd") + ".csv", newoutputs);
        }
    }
}
