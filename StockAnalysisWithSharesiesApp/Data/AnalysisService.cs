using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace StockAnalysisWithSharesiesApp.Data
{
    public interface IAnalysisService
    {
        IEnumerable<Stock> GetFastestGrowingStocks();

        IEnumerable<Stock> GetStocksWithMostPositiveIndicators();

        IEnumerable<Stock> GetStocksOnBargainThisWeek();

        string GetAllSymbolsAnalysed();
    }

    public class AnalysisService : IAnalysisService
    {
        private const int NumberOfResults = 5;
        private readonly IStockService _stockService;
        private readonly ILoginService _loginService;
        private LoginResponse _loginResponse;
        private IEnumerable<Stock> _allStocks = new List<Stock>();

        public AnalysisService(IStockService stockService, ILoginService loginService)
        {
            _stockService = stockService;
            _loginService = loginService;
        }

        public IEnumerable<Stock> GetFastestGrowingStocks()
        {
            GetAllStockData();

            return _allStocks
                .OrderByDescending(s => s.GrowthLastyear())
                .Take(NumberOfResults);
        }

        public IEnumerable<Stock> GetStocksWithMostPositiveIndicators()
        {
            GetAllStockData();

            return _allStocks
                .OrderByDescending(s => s.PositiveIndicators())
                .ThenByDescending(s => s.GrowthLastyear())
                .Take(NumberOfResults);
        }

        public IEnumerable<Stock> GetStocksOnBargainThisWeek()
        {
            GetAllStockData();

            return _allStocks
                .OrderBy(s => s.GrowthLastWeek())
                .Take(NumberOfResults);
        }

        public string GetAllSymbolsAnalysed()
        {
            return string.Join(", ", _allStocks.Select(s => s.symbol));
        }

        private void GetAllStockData()
        {
            if (_allStocks.Any())
            {
                return;
            }

            _loginResponse = _loginService.Login();
            if (_loginResponse != null)
            {
                _allStocks = _stockService.GetStocks(_loginResponse.portfolio.Select(p => p.fund_id), _loginResponse.distill_token);
            }
        }
    }
}