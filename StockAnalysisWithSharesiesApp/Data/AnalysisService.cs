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
        private readonly AnalysisOptions _analysisOptions;
        private readonly IStockService _stockService;
        private readonly ILoginService _loginService;
        private string _token;
        private IEnumerable<Stock> _allStocks;

        public AnalysisService(IOptions<AnalysisOptions> options, IStockService stockService, ILoginService loginService)
        {
            _analysisOptions = options.Value;
            _stockService = stockService;
            _loginService = loginService;
        }

        public IEnumerable<Stock> GetFastestGrowingStocks()
        {
            _token = _token ?? _loginService.Login();
            _allStocks = _allStocks ?? _analysisOptions.Symbols.Select(s => _stockService.GetStock(s, _token));

            return _allStocks
                .OrderByDescending(s => s.GrowthLastyear())
                .Take(NumberOfResults);
        }

        public IEnumerable<Stock> GetStocksWithMostPositiveIndicators()
        {
            _token = _token ?? _loginService.Login();
            _allStocks = _allStocks ?? _analysisOptions.Symbols.Select(s => _stockService.GetStock(s, _token));

            return _allStocks
                .OrderByDescending(s => s.PositiveIndicators())
                .Take(NumberOfResults);
        }

        public IEnumerable<Stock> GetStocksOnBargainThisWeek()
        {
            _token = _token ?? _loginService.Login();
            _allStocks = _allStocks ?? _analysisOptions.Symbols.Select(s => _stockService.GetStock(s, _token));

            return _allStocks
                .OrderBy(s => s.GrowthLastWeek())
                .Take(NumberOfResults);
        }

        public string GetAllSymbolsAnalysed()
        {
            return string.Join(", ", _analysisOptions.Symbols);
        }
    }

    public class AnalysisOptions
    {
        public List<string> Symbols { get; set; }
    }
}