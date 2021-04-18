using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Options;

namespace StockAnalysisWithSharesiesApp.Data
{
    public interface IAnalysisService
    {
        IEnumerable<AnalyzedStock> GetFastestGrowingStocks();

        IEnumerable<AnalyzedStock> GetStocksWithMostPositiveIndicators();

        IEnumerable<AnalyzedStock> GetStocksOnBargainThisWeek();

        IEnumerable<AnalyzedStock> GetStocksThatShouldBeSold();

        string GetAllSymbolsAnalyzed();

        int GetMaximumForPositiveIndicators();
    }

    public class AnalysisService : IAnalysisService
    {
        private const int NumberOfResults = 5;
        private readonly IStockService _stockService;
        private readonly ILoginService _loginService;
        private readonly IMapper _mapper;
        private LoginResponse _loginResponse;
        private IEnumerable<AnalyzedStock> _allStocks = new List<AnalyzedStock>();
        private AnalysisOptions _analysisOptions;

        public AnalysisService(IStockService stockService, ILoginService loginService, IMapper mapper, IOptions<AnalysisOptions> options)
        {
            _stockService = stockService;
            _loginService = loginService;
            _mapper = mapper;
            _analysisOptions = options.Value;
        }

        public IEnumerable<AnalyzedStock> GetFastestGrowingStocks()
        {
            GetAllStockData();

            return _allStocks
                .OrderByDescending(s => s.GrowthLastYear())
                .Take(NumberOfResults);
        }

        public IEnumerable<AnalyzedStock> GetStocksWithMostPositiveIndicators()
        {
            GetAllStockData();

            return _allStocks
                .OrderByDescending(s => s.PositiveIndicators())
                .ThenByDescending(s => s.PercentReturn)
                .Take(NumberOfResults);
        }

        public IEnumerable<AnalyzedStock> GetStocksOnBargainThisWeek()
        {
            GetAllStockData();

            return _allStocks.Where(s => s.PercentReturn > 0)
                .OrderByDescending(s => s.PercentReturn - s.GrowthLastWeek())
                .Take(NumberOfResults);
        }

        public IEnumerable<AnalyzedStock> GetStocksThatShouldBeSold()
        {
            GetAllStockData();

            return _allStocks
                .Where(s => s.PercentReturn > 0 && s.PositiveIndicators() < _analysisOptions.PositiveIndicatorsThreshold)
                .OrderBy(s => s.PercentReturn)
                .Take(NumberOfResults);
        }

        public string GetAllSymbolsAnalyzed()
        {
            return string.Join(", ", _allStocks.Select(s => s.symbol));
        }

        public int GetMaximumForPositiveIndicators()
        {
            return _analysisOptions.PositiveIndicatorsMaximum;
        }

        private void GetAllStockData()
        {
            if (_allStocks.Any()) return;

            _loginResponse = _loginService.Login();
            if (_loginResponse != null)
            {
                var portfolioStocks = _loginResponse.portfolio;
                var stocks = _stockService.GetStocks(_loginResponse.portfolio.Select(p => p.fund_id),
                    _loginResponse.distill_token);
                _allStocks = stocks.Join(portfolioStocks, s => s.id, ps => ps.fund_id,
                    (s, ps) => _mapper.Map<AnalyzedStock>(s).AddPercentReturn(ps.return_percent));
            }
        }
    }

    public class AnalysisOptions
    {
        public int PositiveIndicatorsThreshold { get; set; }

        public int PositiveIndicatorsMaximum { get; set; }
    }

    public class AnalyzedStock : Stock
    {
        public decimal PercentReturn { get; private set; }

        public AnalyzedStock AddPercentReturn(decimal percentReturn)
        {
            PercentReturn = Math.Round(percentReturn, 3);
            return this;
        }

        public class AutoMappingProfile : Profile
        {
            public AutoMappingProfile()
            {
                CreateMap<Stock, AnalyzedStock>()
                    .ForMember(ps => ps.PercentReturn, o => o.Ignore());
            }
        }
    }
}
