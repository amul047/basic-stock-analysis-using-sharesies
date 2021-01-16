using Newtonsoft.Json;
using System;

namespace StockAnalysisWithSharesiesApp.Data
{
    public interface IStockService
    {
        Stock GetStock(string symbol);
        Stock GetStock(string symbol, string token);
    }

    public class StockService : IStockService
    {
        private readonly ILoginService _loginService;

        public StockService(ILoginService loginService)
        {
            _loginService = loginService;
        }

        public Stock GetStock(string symbol)
        {
            var restClient = new RestSharp.RestClient("https://data.sharesies.nz/");
            RestSharp.RestRequest request = new RestSharp.RestRequest($"api/v1/instruments/urlslug/{symbol}", RestSharp.Method.GET);
            request.AddHeader("authorization", $"Bearer {_loginService.Login()}");
            return JsonConvert.DeserializeObject<Stock>(restClient.Execute(request).Content);
        }

        public Stock GetStock(string symbol, string token)
        {
            var restClient = new RestSharp.RestClient("https://data.sharesies.nz/");
            RestSharp.RestRequest request = new RestSharp.RestRequest($"api/v1/instruments/urlslug/{symbol}", RestSharp.Method.GET);
            request.AddHeader("authorization", $"Bearer {token}");
            return JsonConvert.DeserializeObject<Stock>(restClient.Execute(request).Content);
        }
    }

    public class Stock
    {
        public string name { get; set; }

        public string symbol { get; set; }

        public ComparisonPrices comparisonPrices { get; set; }

        public decimal peRatio { get; set; }

        public decimal marketPrice { get; set; }

        public decimal GrowthLastyear()
        {
            return Math.Round((comparisonPrices.OneYear?.percent ?? 0) * 100, 3);
        }

        public decimal GrowthLastWeek()
        {
            return Math.Round((comparisonPrices.OneWeek?.percent ?? 0) * 100, 3);
        }

        public int PositiveIndicators() {

            var positiveIndicators = 0;

            if (comparisonPrices.OneDay?.percent > 0)
            {
                positiveIndicators++;
            }

            if (comparisonPrices.OneWeek?.percent > 0)
            {
                positiveIndicators++;
            }

            if (comparisonPrices.OneMonth?.percent > 0)
            {
                positiveIndicators++;
            }

            if (comparisonPrices.ThreeMonths?.percent > 0)
            {
                positiveIndicators++;
            }

            if (comparisonPrices.SixMonths?.percent > 0)
            {
                positiveIndicators++;
            }

            if (comparisonPrices.OneYear?.percent > 0)
            {
                positiveIndicators++;
            }

            if (comparisonPrices.FiveYears?.percent > 0)
            {
                positiveIndicators++;
            }

            if (peRatio <= 0)
            {
                positiveIndicators++;
            }

            return positiveIndicators;
        }
    }

    public class ComparisonPrices
    {
        [JsonProperty("1d")]
        public PriceChange OneDay { get; set; }
        
        [JsonProperty("1m")]
        public PriceChange OneMonth { get; set; }
        
        [JsonProperty("1w")]
        public PriceChange OneWeek { get; set; }
        
        [JsonProperty("1y")]
        public PriceChange OneYear { get; set; }

        [JsonProperty("3m")]
        public PriceChange ThreeMonths { get; set; }

        [JsonProperty("5y")]
        public PriceChange FiveYears { get; set; }

        [JsonProperty("6m")]
        public PriceChange SixMonths { get; set; }
    }

    public class PriceChange
    {
        public decimal percent { get; set; }
    }
}
