using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading;

namespace StockAnalysisWithSharesiesApp.Data
{
    public interface ILoginService
    {
        LoginResponse Login();
    }

    public class LoginService : ILoginService
    {
        private readonly LoginOptions _options;

        public LoginService(IOptions<LoginOptions> options)
        {
            _options = options.Value;
        }

        public LoginResponse Login()
        {
            var restClient = new RestSharp.RestClient("https://app.sharesies.nz/");
            RestSharp.RestRequest request = new RestSharp.RestRequest("api/identity/login", RestSharp.Method.POST);
            request.AddJsonBody(_options);
            var response = restClient.Execute<LoginResponse>(request);
            return response.Data;
        }
    }

    public class LoginOptions
    {
        public string email { get; set; }

        public string password { get; set; }

        public bool remember { get; set; }
    }

    public class LoginResponse
    {
        public string distill_token { get; set; }

        public List<PortfolioStock> portfolio { get; set; }
    }

    public class PortfolioStock
    {
        public string fund_id { get; set; }
    }
}
