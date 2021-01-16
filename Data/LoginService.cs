using Microsoft.Extensions.Options;

namespace StockAnalysisWithSharesiesApp.Data
{
    public interface ILoginService
    {
        string Login();
    }

    public class LoginService : ILoginService
    {
        private readonly LoginOptions _options;

        public LoginService(IOptions<LoginOptions> options)
        {
            _options = options.Value;
        }

        public string Login()
        {
            var restClient = new RestSharp.RestClient("https://app.sharesies.nz/");
            RestSharp.RestRequest request = new RestSharp.RestRequest("api/identity/login", RestSharp.Method.POST);
            request.AddJsonBody(_options);
            return restClient.Execute<LoginResponse>(request).Data.distill_token;
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
    }
}
