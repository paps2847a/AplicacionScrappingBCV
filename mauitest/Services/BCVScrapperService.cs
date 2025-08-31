using HtmlAgilityPack;
using Microsoft.Maui.Platform;
using System.Globalization;

namespace mauitest.Services
{
    public interface IBCVScrapperService
    {
        Task<Decimal> GetBCVDolar(string wantedCurrecy);
    }

    internal class BCVScrapperService : IBCVScrapperService
    {
        private string BaseUrl;
        private string DolarPriceXPath;
        private string EuroPriceXPath;
        private DateTime? TakeItTime;
        private decimal dolarValue;
        private decimal euroValue;

        public BCVScrapperService()
        {
            BaseUrl = "https://bcv.org.ve";
            DolarPriceXPath = "/html/body/div[4]/div/div[2]/div/div[1]/div[1]/section[1]/div/div[2]/div/div[7]/div/div/div[2]/strong";
            EuroPriceXPath = "/html/body/div[4]/div/div[2]/div/div[1]/div[1]/section[1]/div/div[2]/div/div[3]/div/div/div[2]/strong";
            dolarValue = 0;
            euroValue = 0;
            TakeItTime = null;
        }

        public async Task<Decimal> GetBCVDolar(string wantedCurrecy)
        {
            try
            {
                if (TakeItTime != null)
                {
                    var timeDiff = DateTime.Now - TakeItTime;
                    if (timeDiff <= TimeSpan.FromHours(1))
                        return wantedCurrecy == "USD" ? dolarValue : euroValue;
                }

                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };

                using var client = new HttpClient(handler);
                var html = await client.GetStringAsync(BaseUrl);

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                var dolarPrice = htmlDoc.DocumentNode.SelectNodes(DolarPriceXPath);
                var euroPrice = htmlDoc.DocumentNode.SelectNodes(EuroPriceXPath);

                if (dolarPrice is null || euroPrice is null)
                    return 0;

                var dolarStr = dolarPrice.First().InnerText.TrimStart();
                var euroStr = euroPrice.First().InnerText.TrimStart();
                if (!decimal.TryParse(dolarStr, new CultureInfo("es-ve"),out decimal dolarDecimal) || !decimal.TryParse(euroStr, new CultureInfo("es-ve"), out decimal euroDecimal))
                    return 0;

                var wantedDolarPrice = decimal.Round(dolarDecimal, 2);
                var wantedEuroPrice = decimal.Round(euroDecimal, 2);

                TakeItTime = DateTime.Now;

                dolarValue = wantedDolarPrice;
                euroValue = wantedEuroPrice;

                return wantedCurrecy == "USD" ? dolarValue : euroValue;
            }
            catch(Exception ex)
            {
                return 0;
            }
        }

    }
}
