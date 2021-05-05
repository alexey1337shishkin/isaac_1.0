using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = Parsing(url: "https://isaac-items.ru");
            var a = 5;
        }

        private static object Parsing(string url)
        {
            string nameOfPart = "";
            (string, string, List<string>) tpl = (null, null, new List<string>());
            var result = new List<Tuple<string, string, string, List<string>>>();

            try
            {
                using (HttpClientHandler hdl = new HttpClientHandler { AllowAutoRedirect = false, AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.None })
                {
                    using (var clnt = new HttpClient(hdl))
                    {
                        using (HttpResponseMessage resp = clnt.GetAsync(url).Result)
                        {
                            if (resp.IsSuccessStatusCode)
                            {
                                var html = resp.Content.ReadAsStringAsync().Result;
                                if (!string.IsNullOrEmpty(html))
                                {
                                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                                    doc.LoadHtml(html);
                                    var parts = doc.DocumentNode.SelectNodes(".//div[contains(@class, 'library')]//div[contains(@class, 'items-')]");
                                    if (parts != null && parts.Count > 0)
                                    {
                                        int i = 0;
                                        foreach (var part in parts)
                                        {
                                            i++;
                                            var fNameOfPart = part.SelectNodes(".//h1 [@class]");
                                            if (fNameOfPart != null)
                                            {
                                                nameOfPart = fNameOfPart.Select(n => n.InnerText).ToList().First();
                                            }
                                            var sNameOfPart = part.SelectNodes(".//h2 [@class]");
                                            if (sNameOfPart != null)
                                            {
                                                var temp = sNameOfPart.Select(n => n.InnerText).ToList().First().Split(new char[] { ' ' });
                                                nameOfPart = temp[0] + " " + temp[1];
                                            }

                                            var items = part.SelectNodes(".//div[@class='a ']");
                                            if (items != null && items.Count > 0)
                                            {
                                                foreach (var item in items)
                                                {
                                                    var itemDescription = item.SelectNodes(".//p[@class='item-title']");
                                                    if (itemDescription != null)
                                                        tpl.Item1 = itemDescription.Select(n => n.InnerHtml).ToList().First();

                                                    var itemId = item.SelectNodes(".//p[@class='r-itemid']");
                                                    if (itemId != null)
                                                        tpl.Item2 = itemId.Select(n => n.InnerHtml).ToList().First();

                                                    var itemInformation = item.SelectNodes(".//div[@class='description-middle']//p");
                                                    if (itemInformation != null && itemInformation.Count > 0)
                                                        tpl.Item3 = itemInformation.Select(c => c.InnerText).ToList();
                                                    result.Add(Tuple.Create(nameOfPart, tpl.Item1, tpl.Item2, tpl.Item3));
                                                }
                                            }
                                        }
                                    }
                                    return result;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }
    }
}
