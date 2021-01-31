using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace StockAnalysisWithSharesiesApp.Data
{
    public interface IStockService
    {
        IEnumerable<Stock> GetStocks(IEnumerable<string> stockIds, string token);
    }

    public class StockService : IStockService
    {
        public IEnumerable<Stock> GetStocks(IEnumerable<string> stockIds, string token)
        {
            var restClient = new RestSharp.RestClient("https://data.sharesies.nz/");
            RestSharp.RestRequest request = new RestSharp.RestRequest($"api/v1/instruments", RestSharp.Method.POST);
            request.AddHeader("authorization", $"Bearer {token}");
            request.AddJsonBody(new StockSearchRequest
            {
                instruments = stockIds
            });

            var response = restClient.Execute(request);

            return JsonConvert.DeserializeObject<StockSearchResponse>(response.Content).instruments;
        }
    }

    public class Stock
    {
        public string id { get; set; }

        public string name { get; set; }

        public string symbol { get; set; }

        public ComparisonPrices comparisonPrices { get; set; }

        public decimal peRatio { get; set; }

        public decimal marketPrice { get; set; }

        public decimal GrowthLastYear()
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

    public class StockSearchRequest
    {
        public IEnumerable<string> instruments { get; set; }

        public string query { get; set; } = string.Empty;
    }

    public class StockSearchResponse
    {
        public IEnumerable<Stock> instruments { get; set; }
    }
}
